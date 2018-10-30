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
    [Route("api/ProjectRole/[action]")]
    [Area("AuthorizeManage")]
    public class ProjectRoleController : BaseController
    {
        private readonly IRoleBusines RoleBus;
        private readonly IUserRelationBusines roleUserBll;

        private readonly IBusines.Project.IUserBusines UserBus;

        private readonly IModuleBusines ModuleBus;

        private readonly IButtonBusines ButtonBus;

        private readonly IColumnBusines ColumnBus;

        private readonly IAuthorizeBusines AuthorizeBus;

        public ProjectRoleController(IRoleBusines roleBus, IUserRelationBusines roleUserBll, IBusines.Project.IUserBusines userBus, IModuleBusines moduleBus, IButtonBusines buttonBus, IColumnBusines columnBus, IAuthorizeBusines authorizeBus)
        {
            RoleBus = roleBus;
            this.roleUserBll = roleUserBll;
            UserBus = userBus;
            ModuleBus = moduleBus;
            ButtonBus = buttonBus;
            ColumnBus = columnBus;
            AuthorizeBus = authorizeBus;
        }

        public async Task<IActionResult> GetRoles(int pageSize, int pageIndex)
        {
            var result = await RoleBus.GetListAsync(e => true, e => e.SortCode, true, pageSize, pageIndex, UserInfo.DataBaseName);
            var users = await roleUserBll.GetListAsync(UserInfo.DataBaseName);
            return Success(new
            {
                data = result.Item1.Select(role => new
                {
                    role.RoleId,
                    role.FullName,
                    role.Description,
                    UserNumber = users.ToList().FindAll(r => r.ObjectId == role.RoleId).Count()
                }),
                count = result.Item2
            });
        }

        public async Task<IActionResult> Insert(Role input)
        {
            try
            {
                if (input.RoleId == null)
                {
                    input.RoleId = Guid.NewGuid().ToString();
                    input.DeleteMark = 0;
                    input.CreateDate = DateTime.Now;
                    input.CreateUserId = UserInfo.UserId;
                    var result = await RoleBus.CreateAsync(input, UserInfo.DataBaseName);
                    if (result)
                        return Success();
                    return Fail();

                }
                else
                {
                    var entity = await RoleBus.GetEntityAsync(e => e.RoleId == input.RoleId, UserInfo.DataBaseName);
                    if (entity == null)
                        return null;
                    entity.FullName = input.FullName;
                    entity.Description = input.Description;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserId = UserInfo.UserId;
                    var result = await RoleBus.UpdateAsync(entity, UserInfo.DataBaseName);
                    if (result)
                        return Success();
                    return Fail();
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public async Task<IActionResult> Delete(List<Role> list)
        {
            try
            {
                var result = await RoleBus.DeleteAsync(list, UserInfo.DataBaseName);
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
        /// 更改角色成员时获取项目成员
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetUsers(string roleId)
        {
            try
            {
                var roleUsers = await roleUserBll.GetListAsync(r => r.ObjectId == roleId, UserInfo.DataBaseName);
                var allUsers = await UserBus.GetListAsync(e => e.EnabledMark == 1 && e.RoleId != "PrjAdmin", UserInfo.DataBaseName);
                return Success(new { checkeds = roleUsers.Select(r => r.UserId), users = allUsers });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        public async Task<IActionResult> SaveRoleUsers(string roleId, List<string> userIds)
        {
            try
            {
                var users = await UserBus.GetListAsync(u => u.RoleId == roleId, UserInfo.DataBaseName);
                var delList = await roleUserBll.GetListAsync(r => r.ObjectId == roleId, UserInfo.DataBaseName);
                await roleUserBll.DeleteAsync(delList, UserInfo.DataBaseName);

                var list = new List<UserRelation>();
                if (userIds.Count() > 0)
                {
                    if (users.Count() > 0)
                    {
                        var _users = users.ToList().FindAll(u => !(userIds.Any(id => u.UserId == id)));
                        if (_users.Count > 0)
                        {
                            _users.ForEach(user =>
                            {
                                var entity = new UserRelation
                                {
                                    UserRelationId = Guid.NewGuid().ToString(),
                                    ObjectId = roleId,
                                    UserId = user.UserId,
                                    IsDefault = 1
                                };
                                list.Add(entity);
                            });
                        }
                    }
                    userIds.ForEach(id =>
                    {
                        var isDefault = 0;
                        if (users.Count() > 0 && users.Any(u => u.UserId == id))
                            isDefault = 1;
                        var entity = new UserRelation
                        {
                            UserRelationId = Guid.NewGuid().ToString(),
                            ObjectId = roleId,
                            UserId = id,
                            IsDefault = isDefault
                        };
                        list.Add(entity);
                    });
                }
                else
                {
                    if (users.Count() > 0)
                    {
                        users.ToList().ForEach(user =>
                        {
                            var entity = new UserRelation
                            {
                                UserRelationId = Guid.NewGuid().ToString(),
                                ObjectId = roleId,
                                UserId = user.UserId,
                                IsDefault = 1
                            };
                            list.Add(entity);
                        });
                    }
                }

                var result = await roleUserBll.CreateAsync(list, UserInfo.DataBaseName);
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
        /// 授权
        /// </summary>
        /// <param name="roleId">将要授权的角色ID</param>
        /// <param name="moduleList">可以看到的module</param>
        /// <param name="buttonList">module对应的按钮</param>
        /// <param name="columnList">页面的视图</param>
        /// <returns></returns>
        public async Task<IActionResult> SetAuthorize(string objectId, List<AuthorizeIdType> modules, List<AuthorizeIdType> buttons, List<AuthorizeIdType> columns, List<AuthorizeIdType> routes)
        {
            try
            {
                var result = await AuthorizeBus.SavePermission(objectId, UserInfo.UserId, 2, modules, buttons, columns, routes, UserInfo.DataBaseName);
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