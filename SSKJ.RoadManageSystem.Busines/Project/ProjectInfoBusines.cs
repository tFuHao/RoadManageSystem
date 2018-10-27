using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SSKJ.RoadManageSystem.IBusines.Project;
using SSKJ.RoadManageSystem.IRepository.Project;
using SSKJ.RoadManageSystem.Models.ProjectModel;

namespace SSKJ.RoadManageSystem.Busines.Project
{
   public class ProjectInfoBusines:IProjectInfoBusines
    {
        private readonly IProjectInfoRepository prjInfoRepository;

        public ProjectInfoBusines(IProjectInfoRepository prjInfoRepository)
        {
            this.prjInfoRepository = prjInfoRepository;
        }
        public async Task<bool> CreateAsync(ProjectInfo entity, string dataBaseName = null)
        {
            return await prjInfoRepository.CreateAsync(entity, dataBaseName);
        }

        public async Task<bool> CreateAsync(IEnumerable<ProjectInfo> entityList, string dataBaseName = null)
        {
            return await prjInfoRepository.CreateAsync(entityList, dataBaseName);
        }

        public async Task<bool> DeleteAsync(string keyValue, string dataBaseName = null)
        {
            return await prjInfoRepository.DeleteAsync(keyValue, dataBaseName);
        }

        public async Task<bool> DeleteAsync(string[] keyValues, string dataBaseName = null)
        {
            return await prjInfoRepository.DeleteAsync(keyValues, dataBaseName);
        }

        public async Task<bool> DeleteAsync(ProjectInfo entity, string dataBaseName = null)
        {
            return await prjInfoRepository.DeleteAsync(entity, dataBaseName);
        }

        public async Task<bool> DeleteAsync(IEnumerable<ProjectInfo> entityList, string dataBaseName = null)
        {
            return await prjInfoRepository.DeleteAsync(entityList, dataBaseName);
        }

        public async Task<ProjectInfo> GetEntityAsync(Expression<Func<ProjectInfo, bool>> where, string dataBaseName = null)
        {
            return await prjInfoRepository.GetEntityAsync(where, dataBaseName);
        }

        public async Task<ProjectInfo> GetEntityAsync(string keyValue, string dataBaseName = null)
        {
            return await prjInfoRepository.GetEntityAsync(keyValue, dataBaseName);
        }

        public async Task<IEnumerable<ProjectInfo>> GetListAsync(Expression<Func<ProjectInfo, bool>> where, string dataBaseName = null)
        {
            return await prjInfoRepository.GetListAsync(where, dataBaseName);
        }

        public async Task<Tuple<IEnumerable<ProjectInfo>, int>> GetListAsync<Tkey>(Expression<Func<ProjectInfo, bool>> where, Func<ProjectInfo, Tkey> orderbyLambda, bool isAsc, int pageSize, int pageIndex, string dataBaseName = null)
        {
            return await prjInfoRepository.GetListAsync(where, orderbyLambda, isAsc, pageSize, pageIndex, dataBaseName);
        }

        public async Task<IEnumerable<ProjectInfo>> GetListAsync(string dataBaseName = null)
        {
            return await prjInfoRepository.GetListAsync(dataBaseName);
        }

        public async Task<bool> UpdateAsync(ProjectInfo entity, string dataBaseName = null)
        {
            return await prjInfoRepository.UpdateAsync(entity, dataBaseName);
        }

        public async Task<bool> UpdateAsync(IEnumerable<ProjectInfo> entityList, string dataBaseName = null)
        {
            return await prjInfoRepository.UpdateAsync(entityList, dataBaseName);
        }
    }
}
