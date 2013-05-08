using System.Configuration;
using System.Web.Mvc;
using Frontend.Notifications;
using Model;
using Model.Repositories.interfaces;
using ServiceStack.CacheAccess;
using ServiceStack.Logging;
using ServiceStack.Mvc;
using ServiceStack.Redis;
using ServiceStack.WebHost.Endpoints;
using Services.Routes.impl;
using Services.Routes.interfaces;
using SolrNet;
using SolrNet.Impl;

[assembly: WebActivator.PreApplicationStartMethod(typeof(WebUI.App_Start.AppHost), "Start")]

//IMPORTANT: Add the line below to MvcApplication.RegisterRoutes(RouteCollection) in the Global.asax:
//routes.IgnoreRoute("api/{*pathInfo}"); 

/**
 * Entire ServiceStack Starter Template configured with a 'Hello' Web Service and a 'Todo' Rest Service.
 *
 * Auto-Generated Metadata API page at: /metadata
 * See other complete web service examples at: https://github.com/ServiceStack/ServiceStack.Examples
 */

namespace WebUI.App_Start
{
    
	public class AppHost
		: AppHostBase
	{		
		public AppHost() //Tell ServiceStack the name and where to find your web services
			: base("StarterTemplate ASP.NET Host", typeof(HelloService).Assembly) { }

		public override void Configure(Funq.Container container)
		{
			//Set JSON web services to return idiomatic JSON camelCase properties
			ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
		
			//Configure User Defined REST Paths
			Routes
			  .Add<Hello>("/hello")
			  .Add<Hello>("/hello/{Name*}");

			//Uncomment to change the default ServiceStack configuration
			//SetConfig(new EndpointHostConfig {
			//});

			//Enable Authentication
			//ConfigureAuth(container);

			//Register all your dependencies
			container.Register(new TodoRepository());

            // --------------------------- IoC - Infrastructure Configuration -----------------------------------------------
            container.Register<ILog>(c => LogManager.GetLogger("LogFile"));
            container.Register<IRedisClientsManager>(c => new PooledRedisClientManager(ConfigurationManager.AppSettings["RedisHostAddress"]));
		    container.Register<ICacheClient>(c => (ICacheClient)c.Resolve<IRedisClientsManager>().GetClient());

		    container.RegisterAutoWiredAs<UnitOfWork, IUnitOfWork>();

            // --------------------------- IoC - Business Objects Configuration -----------------------------------------------
            container.RegisterAutoWired<NotificationHub>();
		    container.RegisterAutoWiredAs<DefaultRoutesService, IRoutesService>();

           // container.RegisterFilterProvider();
            //Set MVC to use the same Funq IOC as ServiceStack
			ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
		}

		
		public static void Start()
		{
			new AppHost().Init();
		}
	}
}