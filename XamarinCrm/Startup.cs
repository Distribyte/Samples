using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.Tracing;
using Owin;
using System;
using System.Configuration;
using System.IO;
using System.Web.Http;
using XamarinCRMAppService.Models;

namespace XamarinCRMAppService
{
    public partial class Startup
    {
        public static void Main()
        {
            MobileServiceInitializer initializer = new MobileServiceInitializer();

            var context = new MobileServiceContext();
            initializer.Seed(context);
            Controllers.BaseController.Context = context;

            string url = new Uri(new Uri(ConfigurationManager.AppSettings["ListenAddress"]), "xcrm/tables/").ToString();
            StartOptions startOptions = new StartOptions(url);
            startOptions.Settings.Add(typeof(ITraceOutputFactory).FullName, typeof(NullTraceOutputFactory).AssemblyQualifiedName);

            var host = WebApp.Start(startOptions, Configuration);

            Console.ReadKey();
        }

        public static void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
        }
    }

    public class NullTraceOutputFactory : ITraceOutputFactory
    {
        public TextWriter Create(string outputFile)
        {
            return StreamWriter.Null;
        }
    }
}