using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SSKJ.RoadManageSystem.Models.SystemModel;

namespace SSKJ.RoadManageSystem.IBusines.System
{
    /// <summary>
    /// 模块业务接口定义
    /// </summary>
    public interface IModuleBusines:IBaseBusines<Module>
    {
        /// <summary>
        /// 根据条件where获取数据
        /// </summary>
        /// <param name="where">条件where</param>
        /// <param name="dataBaseName">数据库名称</param>
        /// <returns></returns>
        Task<string> GetTreeListAsync(Expression<Func<Module, bool>> where, string dataBaseName = null);

        string ModuleListToTree(List<Module> list);
    }
}
