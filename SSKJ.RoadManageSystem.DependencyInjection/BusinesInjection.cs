using Microsoft.Extensions.DependencyInjection;
using SSKJ.RoadManageSystem.Busines.Project;
using SSKJ.RoadManageSystem.Busines.System;
using SSKJ.RoadManageSystem.IBusines.Project;
using SSKJ.RoadManageSystem.IBusines.System;

namespace SSKJ.RoadManageSystem.DependencyInjection
{
    /// <summary>
    /// 注入业务逻辑层
    /// </summary>
    public static class BusinesInjection
    {
        public static void ConfigureBusiness(IServiceCollection services)
        {
            //system
            services.AddSingleton<IBusines.System.IUserBusines, Busines.System.UserBusines>();
            services.AddSingleton<IModuleBusines, ModuleBusines>();
            services.AddSingleton<IButtonBusines, ButtonBusines>();
            services.AddSingleton<IColumnBusines, ColumnBusines>();
            services.AddSingleton<IUserProjectBusines, UserProjectBusines>();
            services.AddSingleton<IAreaBusines, AreaBusines>();

            //project
            services.AddSingleton<IAuthorizeBusines, AuthorizeBusines>();
            services.AddSingleton<IBusines.Project.IUserBusines, Busines.Project.UserBusines>();
            services.AddSingleton<IUserRelationBusines, UserRelationBusines>();
            services.AddSingleton<IProjectInfoBusines, ProjectInfoBusines>();
            services.AddSingleton<IRouteBusines, RouteBusines>();
            services.AddSingleton<IRoleBusines, RoleBusines>();
        }
    }
}
