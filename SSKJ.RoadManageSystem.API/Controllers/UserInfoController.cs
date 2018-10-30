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
    public class UserInfoController : BaseController
    {
        private readonly IAuthorizeBusines authorizeBll;
        private readonly IBusines.System.IUserBusines sysUserBll;
        private readonly IBusines.Project.IUserBusines prjUserBll;

        public UserInfoController(IAuthorizeBusines authorizeBll, IBusines.System.IUserBusines sysUserBll, IBusines.Project.IUserBusines prjUserBll)
        {
            this.authorizeBll = authorizeBll;
            this.sysUserBll = sysUserBll;
            this.prjUserBll = prjUserBll;
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
                else if (UserInfo.RoleId == "PrjManger")
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

                return Success(authorize);
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
                    user.RealName = "系统管理员";
                }
                else if (UserInfo.RoleId == "PrjManger")
                {
                    var _user = await sysUserBll.GetEntityAsync(UserInfo.UserId);
                    user = Utility.Tools.MapperUtils.MapTo<User, UserInfoModel>(_user);
                }
                else
                {
                    var _user = await prjUserBll.GetEntityAsync(UserInfo.UserId, UserInfo.DataBaseName);
                    user = Utility.Tools.MapperUtils.MapTo<RoadManageSystem.Models.ProjectModel.User, UserInfoModel>(_user);
                }
                return Success(user);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}