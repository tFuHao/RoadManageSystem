using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSKJ.RoadManageSystem.API.Controllers;
using SSKJ.RoadManageSystem.IBusines.Project;
using SSKJ.RoadManageSystem.IBusines.System;
using SSKJ.RoadManageSystem.Models;
using SSKJ.RoadManageSystem.Models.ProjectModel;

namespace SSKJ.RoadManageSystem.API.Areas.AuthorizeManage.Controllers
{
    [Route("api/ProjectUser/[action]")]
    [Area("AuthorizeManage")]
    public class ProjectUserController : BaseController
    {
        public readonly IAuthorizeBusines authBll;
        private readonly IBusines.Project.IUserBusines userBll;
        private readonly IUserRelationBusines roleUserBll;
        private readonly IRouteBusines routeBll;
        private readonly IModuleBusines moduleBll;
        private readonly IButtonBusines buttonBll;
        private readonly IColumnBusines columnBll;
        private readonly IRoleBusines roleBll;

        public ProjectUserController(IAuthorizeBusines authBll, IBusines.Project.IUserBusines userBll, IUserRelationBusines roleUserBll, IRouteBusines routeBll, IModuleBusines moduleBll, IButtonBusines buttonBll, IColumnBusines columnBll, IRoleBusines roleBll)
        {
            this.authBll = authBll;
            this.userBll = userBll;
            this.roleUserBll = roleUserBll;
            this.routeBll = routeBll;
            this.moduleBll = moduleBll;
            this.buttonBll = buttonBll;
            this.columnBll = columnBll;
            this.roleBll = roleBll;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersData(int pageSize, int pageIndex, string keyword)
        {
            try
            {
                var users = new Tuple<IEnumerable<User>, int>(null, 0);
                if (!string.IsNullOrEmpty(keyword))
                    users = await userBll.GetListAsync(u => u.RealName.Contains(keyword) && u.RoleId != "PrjAdmin", u => u.CreateDate, true, pageSize, pageIndex, UserInfo.DataBaseName);
                else
                    users = await userBll.GetListAsync(u => u.RoleId != "PrjAdmin", u => u.CreateDate, true, pageSize, pageIndex, UserInfo.DataBaseName);

                return Success(new { data = users.Item1, count = users.Item2 });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetRoleData()
        {
            try
            {
                var roles = await roleBll.GetListAsync(UserInfo.DataBaseName);
                return Success(roles);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUserPermission(int category, string objectId)
        {
            try
            {
                var result = await authBll.GetModuleAndRoutePermission(category, objectId, UserInfo.DataBaseName);
                return Success(result);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public IActionResult GetButtonAndColumnTree(List<string> halfKeys, List<string> checkedKeys, string strAuthorizes, string strModules, string strButtons, string strColumns)
        {
            var result = authBll.GetButtonAndColumnPermission(halfKeys, checkedKeys, strAuthorizes, strModules, strButtons, strColumns);
            return Success(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEntity(User entity)
        {
            try
            {
                var result = false;
                if (string.IsNullOrEmpty(entity.UserId))
                {
                    entity.UserId = Guid.NewGuid().ToString();
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUserId = UserInfo.UserId;
                    entity.Gender = 0;
                    entity.EnabledMark = 1;
                    entity.HeadIcon = "/userAvatar/avatar.jpg";
                    entity.Secretkey = Guid.NewGuid().ToString();
                    entity.Password = Utility.Tools.MD5Utils.Sign(entity.Password, entity.Secretkey);
                    result = await userBll.CreateAsync(entity, UserInfo.DataBaseName);
                }
                else
                {
                    var user = await userBll.GetEntityAsync(entity.UserId, UserInfo.DataBaseName);
                    user.ModifyDate = DateTime.Now;
                    user.ModifyUserId = UserInfo.UserId;
                    user.Account = entity.Account;
                    user.RealName = entity.RealName;
                    user.RoleId = entity.RoleId;

                    result = await userBll.UpdateAsync(user, UserInfo.DataBaseName);
                }
                if (result)
                {
                    var roleUserEntity = await roleUserBll.GetEntityAsync(r => r.UserId == entity.UserId && r.IsDefault == 1, UserInfo.DataBaseName);
                    if (roleUserEntity != null)
                        await roleUserBll.DeleteAsync(roleUserEntity, UserInfo.DataBaseName);

                    var ruEntity = new UserRelation
                    {
                        UserRelationId = Guid.NewGuid().ToString(),
                        ObjectId = entity.RoleId,
                        UserId = entity.UserId,
                        IsDefault = 1
                    };
                    await roleUserBll.CreateAsync(ruEntity, UserInfo.DataBaseName);
                    return Success();

                }
                else
                    return Fail();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(List<User> list)
        {
            try
            {
                var userAuth = await authBll.GetListAsync(a => list.Any(u => u.UserId == a.ObjectId), UserInfo.DataBaseName);
                var roleUser = await roleUserBll.GetListAsync(r => list.Any(u => u.UserId == r.UserId), UserInfo.DataBaseName);
                var result = await userBll.DeleteAsync(list, UserInfo.DataBaseName);
                if (result)
                {
                    await authBll.DeleteAsync(userAuth, UserInfo.DataBaseName);
                    await roleUserBll.DeleteAsync(roleUser, UserInfo.DataBaseName);

                    return Success();
                }
                return Fail();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserPermission(string userId, List<AuthorizeIdType> modules, List<AuthorizeIdType> buttons, List<AuthorizeIdType> columns, List<AuthorizeIdType> routes)
        {
            try
            {
                var result = await authBll.SavePermission(userId, UserInfo.UserId, 1, modules, buttons, columns, routes, UserInfo.DataBaseName);
                if (result)
                    return Success();
                return Fail();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 重置用户密码为123456
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            try
            {
                var user = await userBll.GetEntityAsync(userId, UserInfo.DataBaseName);
                user.Secretkey = Guid.NewGuid().ToString();
                user.Password = Utility.Tools.MD5Utils.Sign("123456", user.Secretkey);

                var result = await userBll.UpdateAsync(user, UserInfo.DataBaseName);
                if (result)
                    return Success();
                return Fail();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="userId">主键值</param>
        /// <param name="state">状态：1-启动；0-禁用</param>
        [HttpPost]
        public async Task<IActionResult> UpdateState(string userId, int state)
        {
            try
            {
                var user = await userBll.GetEntityAsync(userId, UserInfo.DataBaseName);
                user.EnabledMark = state;

                var result = await userBll.UpdateAsync(user, UserInfo.DataBaseName);
                if (result)
                    return Success();
                return Fail();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}