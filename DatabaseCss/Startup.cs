using DatabaseCss.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DatabaseCss
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<DataBaseContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DatabaseCss")));
            var tempServiceProvider = services.BuildServiceProvider();
            services.AddWebOptimizer(pipeline =>
            {
                var provider = ActivatorUtilities.CreateInstance<DatabaseFileProvider>(tempServiceProvider);
                pipeline.AddCssBundle("/css/bundle.css", "/database/a.css", "database/b.css").UseFileProvider(provider);
                pipeline.AddCssBundle("/css/static.css", "/css/site.css");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseWebOptimizer();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);
        }

        private void OnStarted()
        {
            DataBaseWatcher.GetInstance().Initialization(Configuration.GetConnectionString("DatabaseCss"));
        }

        private void OnStopping()
        {
            DataBaseWatcher.GetInstance().Termination();
        }

        private void OnStopped()
        {
        }
    }
}