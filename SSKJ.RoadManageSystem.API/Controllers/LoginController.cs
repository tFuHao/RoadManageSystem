using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSKJ.RoadManageSystem.API.Models;
using SSKJ.RoadManageSystem.IBusines.Project;
using SSKJ.RoadManageSystem.IBusines.System;
using SSKJ.RoadManageSystem.Models.SystemModel;

namespace SSKJ.RoadManageSystem.API.Controllers
{
    [Route("api/Login/[action]")]
    public class LoginController : BaseController
    {
        private readonly IUserProjectBusines userProjectBll;
        private readonly IBusines.System.IUserBusines sysUserBll;
        private readonly IBusines.Project.IUserBusines prjUserBll;
        private readonly IAuthorizeBusines authorizeBll;

        public LoginController(IUserProjectBusines userProjectBll, IBusines.System.IUserBusines sysUserBll, IBusines.Project.IUserBusines prjUserBll, IAuthorizeBusines authorizeBll)
        {
            this.userProjectBll = userProjectBll;
            this.sysUserBll = sysUserBll;
            this.prjUserBll = prjUserBll;
            this.authorizeBll = authorizeBll;
        }

        [HttpPost]
        public async Task<IActionResult> LoginIn(LoginModel model)
        {
            try
            {
                var _user = new UserTokenInfoModel();

                if (string.IsNullOrEmpty(model.ProjectCode))
                {
                    if (model.UserName.ToLower() == "system")
                    {
                        if (model.Password == "123456")
                            _user.UserId = _user.RoleId = "System";
                        else
                            return BadRequest(new { type = 0, message = "密码错误，请重新输入!" });
                    }
                    else
                    {
                        var user = await sysUserBll.GetEntityAsync(u => u.Account == model.UserName);
                        if (user == null)
                            return BadRequest(new { type = 0, message = "用户名错误或用户名不存在，请重新输入!" });
                        else
                        {
                            var paw = Utility.Tools.MD5Utils.Sign(model.Password, user.Secretkey);
                            if (user.Password != paw)
                                return BadRequest(new { type = 0, message = "密码错误，请重新输入!" });
                        }
                        _user = Utility.Tools.MapperUtils.MapTo<User, UserTokenInfoModel>(user);
                        _user.RoleId = "PrjManager";
                    }
                }
                else
                {
                    var entity = await userProjectBll.GetEntityAsync(p => p.PrjIdentification == model.ProjectCode);

                    if (entity == null)
                        return BadRequest(new { type = 0, message = "项目代码有误或不存在，请重新输入!" });

                    if (string.IsNullOrEmpty(entity.PrjDataBase))
                        return BadRequest(new { type = 0, message = "出错了，请稍后重试!" });

                    var user = await prjUserBll.GetEntityAsync(u => u.Account == model.UserName, entity.PrjDataBase);

                    if (user == null)
                        return BadRequest(new { type = 0, message = "用户名错误或用户名不存在，请重新输入!" });
                    else
                    {
                        var paw = Utility.Tools.MD5Utils.Sign(model.Password, user.Secretkey);
                        if (user.Password != paw)
                            return BadRequest(new { type = 0, message = "密码错误，请重新输入!" });
                    }

                    if (user.EnabledMark == 0)
                        return BadRequest(new { type = 0, message = "该角色已被锁定，请联系管理员解锁" });

                    _user = Utility.Tools.MapperUtils.MapTo<RoadManageSystem.Models.ProjectModel.User, UserTokenInfoModel>(user);
                    _user.DataBaseName = entity.PrjDataBase;
                }
                _user.TokenExpiration = DateTime.Now.AddDays(1);

                string token = Utility.Tools.TokenUtils.ToToken(_user);

                return Ok(new { type = 1, token });
            }
            catch (Exception)
            {
                return BadRequest(new { type = 0, message = "出错了，请稍后重试!" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPermission()
        {
            try
            {
                var userInfo = GetUserInfo();

                var user = new UserInfoModel();

                if (userInfo.RoleId == "System")
                {
                    user.UserId = user.Account = user.RoleId = "System";
                    user.RealName = "系统管理员";
                }
                else if (userInfo.RoleId == "PrjManger")
                {
                    var _user = await sysUserBll.GetEntityAsync(userInfo.UserId);
                    user = Utility.Tools.MapperUtils.MapTo<User, UserInfoModel>(_user);
                }
                else
                {
                    var _user = await prjUserBll.GetEntityAsync(userInfo.UserId, userInfo.DataBaseName);
                    user = Utility.Tools.MapperUtils.MapTo<RoadManageSystem.Models.ProjectModel.User, UserInfoModel>(_user);
                }

                if (userInfo.TokenExpiration <= DateTime.Now)
                    return BadRequest(new { type = 0, message = "登录超时，请重新登录!" });

                var authorize = new AuthorizeModel
                {
                    UserInfo = user,
                    ModuleAuthorizes = await authorizeBll.GetModuleAuthorizes(2, userInfo.RoleId, userInfo.DataBaseName),
                    ButtonAuthorizes = await authorizeBll.GetButtonAuthorizes(2, userInfo.RoleId, userInfo.DataBaseName),
                    ColumnAuthorizes = await authorizeBll.GetColumnAuthorizes(2, userInfo.RoleId, userInfo.DataBaseName),
                    RouteAuthorizes = await authorizeBll.GetRouteAuthorizes(2, userInfo.RoleId, userInfo.DataBaseName)
                };

                return Ok(authorize);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "出错了，请稍后重试!" });
            }
        }
    }
}