using Microsoft.Extensions.DependencyInjection;
using SSKJ.RoadManageSystem.IRepository.Project;
using SSKJ.RoadManageSystem.IRepository.System;
using SSKJ.RoadManageSystem.Repository.MySQL.Project;
using SSKJ.RoadManageSystem.Repository.MySQL.System;

namespace SSKJ.RoadManageSystem.DependencyInjection
{
    /// <summary>
    /// 注入仓储层
    /// </summary>
    public static class RepositoryInjection
    {
        public static void ConfigureRepository(IServiceCollection services)
        {
            //system
            services.AddSingleton<IModuleRepository, ModuleRepository>();
            services.AddSingleton<IButtonRepository, ButtonRepository>();
            services.AddSingleton<IColumnRepository, ColumnRepository>();
            services.AddSingleton<IRepository.System.IUserRepository, Repository.MySQL.System.UserRepository>();
            services.AddSingleton<IUserProjectRepository, UserProjectRepository>();
            services.AddSingleton<IAreaRepository, AreaRepository>();

            //project
            services.AddSingleton<IAuthorizeRepository, AuthorizeRepository>();
            services.AddSingleton<IRepository.Project.IUserRepository, Repository.MySQL.Project.UserRepository>();
            services.AddSingleton<IUserRelationRepository, UserRelationRepository>();
            services.AddSingleton<IProjectInfoRepository, ProjectInfoRepository>();
            services.AddSingleton<IRouteRepository, RouteRepository>();
            services.AddSingleton<IRoleRepository, RoleRepository>();
        }
    }
}
