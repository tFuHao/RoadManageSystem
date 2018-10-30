using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using SSKJ.RoadManageSystem.DependencyInjection;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Viyi.Util.Linq;

namespace SSKJ.RoadManageSystem.API
{
    public class Startup
    {
        IMvcBuilder mvcBuilder;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            mvcBuilder = services.AddMvc();

            RepositoryInjection.ConfigureRepository(services);
            BusinesInjection.ConfigureBusiness(services);

            RoadDesignCenter.DependencyInjection.RepositoryInjection.ConfigureRepository(services);
            RoadDesignCenter.DependencyInjection.BusinesInjection.ConfigureBusiness(services);

            services.AddHttpContextAccessor();
            services.AddCors();
            services.AddMvc().AddJsonOptions(config =>
            {
                config.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddMemoryCache();

            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            LoadPlugins(env);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSession();
            app.UseStaticFiles();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void LoadPlugins(IHostingEnvironment env)
        {
            var plugins = env.ContentRootFileProvider.GetFileInfo("Plugins");
            if (!Directory.Exists(plugins.PhysicalPath))
            {
                return;
            }

            var target = Path.Combine(env.ContentRootPath, "app_data", "plugins-cache");
            Directory.CreateDirectory(target);

            Directory.EnumerateDirectories(plugins.PhysicalPath)
                .Select(path => Path.Combine(path, "bin"))
                .Where(Directory.Exists)
                .SelectMany(bin => Directory.EnumerateFiles(bin, "*.dll"))
                .ForEach(dll => File.Copy(dll, Path.Combine(target, Path.GetFileName(dll)), true));

            Directory.EnumerateFiles(target, "*.dll")
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                .ForEach(mvcBuilder.AddApplicationPart);
        }
    }
}
