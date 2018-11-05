using SSKJ.RoadManageSystem.IBusines.System;
using SSKJ.RoadManageSystem.IRepository.System;
using SSKJ.RoadManageSystem.Models.SystemModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SSKJ.RoadManageSystem.Busines.System
{
    public class AreaBusines : IAreaBusines
    {
        private readonly IAreaRepository areaRepository;
        public AreaBusines(IAreaRepository areaRepository)
        {
            this.areaRepository = areaRepository;
        }

        public async Task<bool> CreateAsync(Area entity, string dataBaseName = null)
        {
            return await areaRepository.CreateAsync(entity, dataBaseName);
        }

        public async Task<bool> CreateAsync(IEnumerable<Area> entityList, string dataBaseName = null)
        {
            return await areaRepository.CreateAsync(entityList, dataBaseName);
        }

        public async Task<bool> DeleteAsync(string keyValue, string dataBaseName = null)
        {
            return await areaRepository.DeleteAsync(keyValue, dataBaseName);
        }

        public async Task<bool> DeleteAsync(string[] keyValues, string dataBaseName = null)
        {
            return await areaRepository.DeleteAsync(keyValues, dataBaseName);
        }

        public async Task<bool> DeleteAsync(Area entity, string dataBaseName = null)
        {
            return await areaRepository.DeleteAsync(entity, dataBaseName);
        }

        public async Task<bool> DeleteAsync(IEnumerable<Area> entityList, string dataBaseName = null)
        {
            return await areaRepository.DeleteAsync(entityList, dataBaseName);
        }

        public async Task<Area> GetEntityAsync(Expression<Func<Area, bool>> where, string dataBaseName = null)
        {
            return await areaRepository.GetEntityAsync(where, dataBaseName);
        }

        public async Task<Area> GetEntityAsync(string keyValue, string dataBaseName = null)
        {
            return await areaRepository.GetEntityAsync(keyValue, dataBaseName);
        }

        public async Task<IEnumerable<Area>> GetListAsync(Expression<Func<Area, bool>> where, string dataBaseName = null)
        {
            return await areaRepository.GetListAsync(where, dataBaseName);
        }

        public async Task<Tuple<IEnumerable<Area>, int>> GetListAsync<Tkey>(Expression<Func<Area, bool>> where, Func<Area, Tkey> orderbyLambda, bool isAsc, int pageSize, int pageIndex, string dataBaseName = null)
        {
            return await areaRepository.GetListAsync(where, orderbyLambda, isAsc, pageSize, pageIndex, dataBaseName);
        }

        public async Task<IEnumerable<Area>> GetListAsync(string dataBaseName = null)
        {
            return await areaRepository.GetListAsync(dataBaseName);
        }

        public async Task<string> GetStrTreeAsync()
        {
            var data = await areaRepository.GetListAsync(a => a.Layer < 4);

            var temp=TreeData.AreaTreeJson(data.OrderBy(o => o.SortCode).ToList());
            return temp;
        }

        public async Task<bool> UpdateAsync(IEnumerable<Area> entityList, string dataBaseName = null)
        {
            return await areaRepository.UpdateAsync(entityList, dataBaseName);
        }

        public async Task<bool> UpdateAsync(Area entity, string dataBaseName = null)
        {
            return await areaRepository.UpdateAsync(entity, dataBaseName);
        }
    }
}
