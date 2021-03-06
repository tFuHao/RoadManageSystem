﻿using SSKJ.RoadManageSystem.Models.ProjectModel;
using SSKJ.RoadManageSystem.Models.SystemModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSKJ.RoadManageSystem.IBusines.Project
{
   public interface IAuthorizeBusines : IBaseBusines<Authorize>
    {
        // <summary>
        /// 获取功能权限
        /// </summary>
        /// <param name="category">1用户权限 2角色权限</param>
        /// <param name="objectId">用户ID或角色ID</param>
        /// <param name="dataBaseName"></param>
        /// <returns></returns>
        Task<IEnumerable<Module>> GetModuleAuthorizes(int category, string objectId, string dataBaseName);

        /// <summary>
        /// 获取功能按钮权限
        /// </summary>
        /// <param name="category">1用户权限 2角色权限</param>
        /// <param name="objectId">用户ID或角色ID</param>
        /// <param name="dataBaseName"></param>
        /// <returns></returns>
        Task<IEnumerable<ModuleButton>> GetButtonAuthorizes(int category, string objectId, string dataBaseName);

        /// <summary>
        /// 获取功能视图权限
        /// </summary>
        /// <param name="category">1用户权限 2角色权限</param>
        /// <param name="objectId">用户ID或角色ID</param>
        /// <param name="dataBaseName"></param>
        /// <returns></returns>
        Task<IEnumerable<ModuleColumn>> GetColumnAuthorizes(int category, string objectId, string dataBaseName);
        Task<string> GetRouteAuthorizes(int category, string objectId, string dataBaseName);

        Task<Models.PermissionModel> GetModuleAndRoutePermission(int category, string objectId, string dataBaseName);
        Models.PermissionModel GetButtonAndColumnPermission(List<string> halfKeys, List<string> checkedKeys, string strAuthorizes, string strModules, string strButtons, string strColumns);
        Task<bool> SavePermission(string objectId, string currentUserId, int category, List<Models.AuthorizeIdType> modules, List<Models.AuthorizeIdType> buttons, List<Models.AuthorizeIdType> columns, List<Models.AuthorizeIdType> routes, string dataBaseName);
    }
}
