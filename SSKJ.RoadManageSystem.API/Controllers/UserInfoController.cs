using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using SSKJ.RoadManageSystem.API.Models;
using SSKJ.RoadManageSystem.IBusines.Project;
using SSKJ.RoadManageSystem.IBusines.System;
using SSKJ.RoadManageSystem.Models.SystemModel;

namespace SSKJ.RoadManageSystem.API.Controllers
{
    [Route("api/UserInfo/[action]")]
    public class UserInfoController : BaseController
    {
        private readonly IAuthorizeBusines authorizeBll;
        private readonly IRoleBusines roleBll;
        private readonly IBusines.System.IUserBusines sysUserBll;
        private readonly IBusines.Project.IUserBusines prjUserBll;
        private readonly IAreaBusines areaBll;
        private readonly HostingEnvironment he;

        public UserInfoController(IRoleBusines roleBll, IAuthorizeBusines authorizeBll, IBusines.System.IUserBusines sysUserBll, IBusines.Project.IUserBusines prjUserBll, IAreaBusines areaBll, HostingEnvironment he)
        {
            this.authorizeBll = authorizeBll;
            this.sysUserBll = sysUserBll;
            this.prjUserBll = prjUserBll;
            this.roleBll = roleBll;
            this.areaBll = areaBll;
            this.he = he;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPermission(string moduleId)
        {
            try
            {
                var user = new UserInfoModel();

                if (UserInfo.RoleId == "System")
                {
                    user.UserId = user.Account = user.RoleId = "System";
                    user.RealName = "系统管理员";
                }
                else if (UserInfo.RoleId == "PrjManager")
                {
                    var _user = await sysUserBll.GetEntityAsync(UserInfo.UserId);
                    user = Utility.Tools.MapperUtils.MapTo<User, UserInfoModel>(_user);
                }
                else
                {
                    var _user = await prjUserBll.GetEntityAsync(UserInfo.UserId, UserInfo.DataBaseName);
                    user = Utility.Tools.MapperUtils.MapTo<RoadManageSystem.Models.ProjectModel.User, UserInfoModel>(_user);
                }

                var authorize = new AuthorizeModel
                {
                    UserInfo = user,
                    ModuleAuthorizes = await authorizeBll.GetModuleAuthorizes(moduleId, 2, UserInfo.RoleId, UserInfo.DataBaseName),
                    ButtonAuthorizes = await authorizeBll.GetButtonAuthorizes(moduleId, 2, UserInfo.RoleId, UserInfo.DataBaseName),
                    ColumnAuthorizes = await authorizeBll.GetColumnAuthorizes(moduleId, 2, UserInfo.RoleId, UserInfo.DataBaseName)
                };
                if (!string.IsNullOrEmpty(moduleId))
                    authorize.RouteAuthorizes = await authorizeBll.GetRouteAuthorizes(2, UserInfo.RoleId, UserInfo.DataBaseName);

                return SuccessData(authorize);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonalInfo()
        {
            try
            {
                var user = new UserInfoModel();

                if (UserInfo.RoleId == "System")
                {
                    user.UserId = user.Account = user.RoleId = "System";
                    user.RealName = user.RoleName = "系统管理员";
                }
                else if (UserInfo.RoleId == "PrjManger")
                {
                    var _user = await sysUserBll.GetEntityAsync(UserInfo.UserId);
                    user = Utility.Tools.MapperUtils.MapTo<User, UserInfoModel>(_user);
                    user.RoleName = "项目管理员";
                }
                else
                {
                    var _user = await prjUserBll.GetEntityAsync(UserInfo.UserId, UserInfo.DataBaseName);
                    user = Utility.Tools.MapperUtils.MapTo<RoadManageSystem.Models.ProjectModel.User, UserInfoModel>(_user);

                    var role = await roleBll.GetEntityAsync(r => r.RoleId == user.RoleId, UserInfo.DataBaseName);
                    if (user.RoleId == "PrjAdmin")
                        user.RoleName = "项目管理员";
                    else
                        user.RoleName = role?.FullName;
                }

                var area = await areaBll.GetListAsync();
                var sheng = area.SingleOrDefault(a => a.AreaId == user.ProvinceId);
                var shi = area.SingleOrDefault(a => a.AreaId == user.CityId);
                var qu = area.SingleOrDefault(a => a.AreaId == user.CountyId);

                if (sheng != null && shi != null && qu != null)
                    user.Area = sheng.AreaName + "-" + shi.AreaName + "-" + qu.AreaName;

                return SuccessData(user);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ValidatePawssward(string pawssward)
        {
            try
            {
                if (string.IsNullOrEmpty(pawssward))
                {
                    return Error("密码不能为空!");
                }
                else
                {
                    if (UserInfo.RoleId == "System")
                    {
                        if (pawssward == "123456")
                            return SuccessMes();
                        return Fail();
                    }
                    else if (UserInfo.RoleId == "PrjManager")
                    {
                        var user = await sysUserBll.GetEntityAsync(UserInfo.UserId);
                        var paw = Utility.Tools.MD5Utils.Sign(pawssward, user.Secretkey);
                        if (paw == user.Password)
                            return SuccessMes();
                        return Fail();
                    }
                    else
                    {
                        var user = await prjUserBll.GetEntityAsync(UserInfo.UserId, UserInfo.DataBaseName);
                        var paw = Utility.Tools.MD5Utils.Sign(pawssward, user.Secretkey);
                        if (paw == user.Password)
                            return SuccessMes();
                        return Fail();
                    }
                }
            }
            catch (Exception)
            {
                return Error("出错了，请稍后再试!");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ValidateFullName(int type, string fullName)
        {
            try
            {
                if (string.IsNullOrEmpty(fullName))
                {
                    if (type == 1)
                        return Error("邮箱不能为空!");
                    else if (type == 2)
                        return Error("手机不能为空!");
                    else if (type == 3)
                        return Error("用户名不能为空!");
                    else
                        return Error("姓名不能为空!");
                }
                else
                {
                    if (UserInfo.RoleId == "PrjManager")
                    {
                        var users = await sysUserBll.GetListAsync(u => u.UserId != UserInfo.UserId);
                        int count = 0;
                        if (type == 1)
                            count = users.ToList().FindAll(u => u.Email == fullName).Count();
                        else if (type == 2)
                            count = users.ToList().FindAll(u => u.Mobile == fullName).Count();
                        else if (type == 3)
                            count = users.ToList().FindAll(u => u.Account == fullName).Count();
                        else
                            count = users.ToList().FindAll(u => u.RealName == fullName).Count();

                        return SuccessData(count);
                    }
                    else
                    {
                        var users = await prjUserBll.GetListAsync(u => u.UserId != UserInfo.UserId, UserInfo.DataBaseName);
                        int count = 0;
                        if (type == 1)
                            count = users.ToList().FindAll(u => u.Email == fullName).Count();
                        else if (type == 2)
                            count = users.ToList().FindAll(u => u.Mobile == fullName).Count();
                        else if (type == 3)
                            count = users.ToList().FindAll(u => u.Account == fullName).Count();
                        else
                            count = users.ToList().FindAll(u => u.RealName == fullName).Count();

                        return SuccessData(count);
                    }
                }
            }
            catch (Exception)
            {
                return Error("出错了，请稍后再试!");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditPawssward(string newPawssward)
        {
            try
            {
                if (string.IsNullOrEmpty(newPawssward))
                {
                    return Error("密码不能为空!");
                }
                else
                {
                    if (UserInfo.RoleId == "System")
                    {
                        return SuccessMes();
                    }
                    else if (UserInfo.RoleId == "PrjManager")
                    {
                        var user = await sysUserBll.GetEntityAsync(UserInfo.UserId);
                        user.Secretkey = Guid.NewGuid().ToString();
                        user.Password = Utility.Tools.MD5Utils.Sign(newPawssward, user.Secretkey);

                        var result = await sysUserBll.UpdateAsync(user);
                        if (result)
                            return SuccessMes();
                        return Fail();
                    }
                    else
                    {
                        var user = await prjUserBll.GetEntityAsync(UserInfo.UserId, UserInfo.DataBaseName);
                        user.Secretkey = Guid.NewGuid().ToString();
                        user.Password = Utility.Tools.MD5Utils.Sign(newPawssward, user.Secretkey);

                        var result = await prjUserBll.UpdateAsync(user, UserInfo.DataBaseName);
                        if (result)
                            return SuccessMes();
                        return Fail();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetArea()
        {
            try
            {
                var area = await areaBll.GetStrTreeAsync();
                return SuccessData(area);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserIfno(string avatar, UserInfoModel entity)
        {
            try
            {
                var head = avatar == entity.HeadIcon && !string.IsNullOrEmpty(avatar);

                if (UserInfo.RoleId == "PrjManager")
                {
                    var user = await sysUserBll.GetEntityAsync(UserInfo.UserId);
                    user.Account = entity.Account;
                    user.RealName = entity.RealName;
                    user.Gender = entity.Gender;
                    user.Birthday = entity.Birthday;
                    user.Mobile = entity.Mobile;
                    user.Email = entity.Email;
                    user.ProvinceId = entity.ProvinceId;
                    user.CityId = entity.CityId;
                    user.CountyId = entity.CountyId;
                    if (!head)
                        user.HeadIcon = SaveHead(avatar);

                    var result = await sysUserBll.UpdateAsync(user);
                    if (result)
                        return SuccessMes();
                    return Fail();
                }
                else
                {
                    var user = await prjUserBll.GetEntityAsync(UserInfo.UserId, UserInfo.DataBaseName);
                    user.Account = entity.Account;
                    user.RealName = entity.RealName;
                    user.Gender = entity.Gender;
                    user.Birthday = entity.Birthday;
                    user.Mobile = entity.Mobile;
                    user.Email = entity.Email;
                    user.ProvinceId = entity.ProvinceId;
                    user.CityId = entity.CityId;
                    user.CountyId = entity.CountyId;
                    if (!head)
                        user.HeadIcon = SaveHead(avatar);

                    var result = await prjUserBll.UpdateAsync(user, UserInfo.DataBaseName);
                    if (result)
                        return SuccessMes();
                    return Fail();
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// base64代码转图片并保存，返回保存的路径
        /// </summary>
        /// <param name="base64Img">base64编码</param>
        /// <returns></returns>
        public string SaveHead(string base64Img)
        {
            var base64 = "";
            base64 = base64Img.Replace("data:image/png;base64,", "png").Replace("data:image/jgp;base64,", "png").Replace("data:image/jpg;base64,", "png").Replace("data:image/jpeg;base64,", "png").Replace("data:image/gif;base64,", "gif");
            var ext = "." + base64.Substring(0, 3);
            base64 = base64.Remove(0, 3);
            byte[] bytes = Convert.FromBase64String(base64);
            var path = he.WebRootPath + "\\userAvatar\\" + UserInfo.UserId + "\\";
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            Directory.CreateDirectory(path);

            var fileName = Guid.NewGuid().ToString() + ext;
            var filePath = path + fileName;
            System.IO.File.WriteAllBytes(filePath, bytes);
            return ("/userAvatar/" + UserInfo.UserId + "/" + fileName).Replace("\\", "/");
        }
    }
}