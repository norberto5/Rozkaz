using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Client.TokenCacheProviders;
using Rozkaz.Models;
using Rozkaz.Services;
using System;
using System.Globalization;
using WebApp_OpenIDConnect_DotNet.Infrastructure;
using WebApp_OpenIDConnect_DotNet.Services.GraphOperations;

namespace Rozkaz
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<CookiePolicyOptions>(options =>
      {
        options.CheckConsentNeeded = context => false;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

      services.AddSession(options =>
      {
        options.Cookie.IsEssential = true;
        options.IdleTimeout = TimeSpan.FromHours(24);
      });

      services.AddEntityFrameworkSqlServer()
          .AddDbContext<RozkazDatabaseContext>(options => options.UseMySql(Configuration["Database:Connection"]));

      services.AddAzureAdV2Authentication(Configuration)
          .AddMsal(new string[] { Constants.ScopeUserRead })
          .AddInMemoryTokenCaches();

      services.AddGraphService(Configuration);

      services.AddSingleton<OrderPdfService>();
      services.AddScoped<UserResolver>();

      services.AddHttpContextAccessor();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
      }

      app.UseAuthentication();
      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseCookiePolicy();

      app.UseSession();

      var cultureInfo = new CultureInfo("pl-PL");
      CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
      CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
