using SSKJ.RoadManageSystem.Models.SystemModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SSKJ.RoadManageSystem.IBusines.System
{
    public interface IColumnBusines : IBaseBusines<ModuleColumn>
    {
        /// <summary>
        /// 根据条件where获取数据
        /// </summary>
        /// <param name="where">条件where</param>
        /// <param name="dataBaseName">数据库名称</param>
        /// <returns></returns>
        Task<string> GetTreeListAsync(Expression<Func<ModuleColumn, bool>> where, string dataBaseName = null);

        string ColumnListToTree(List<ModuleColumn> list);
    }
}
