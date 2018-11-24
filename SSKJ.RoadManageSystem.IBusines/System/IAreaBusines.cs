using SSKJ.RoadManageSystem.Models.SystemModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSKJ.RoadManageSystem.IBusines.System

{
   public interface IAreaBusines : IBaseBusines<Area>
    {
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="dataBaseName">数据库名称</param>
        /// <returns></returns>
        Task<string> GetStrTreeAsync();
    }
}
