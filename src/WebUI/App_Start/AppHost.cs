using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Frontend.Notifications;
using Model;
using Model.Repositories.interfaces;
using ServiceStack.CacheAccess;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.Logging.Log4Net;
using ServiceStack.Mvc;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using Services.Routes.impl;
using Services.Routes.interfaces;
using WebUI.Areas.Solr.Controllers;
using log4net.Config;

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

          
            container.Register<ILog>(c => LogManager.GetLogger("LogFile"));
            container.Register<IRedisClientsManager>(c => new PooledRedisClientManager(ConfigurationManager.AppSettings["RedisHostAddress"]));
		    container.Register<ICacheClient>(c => (ICacheClient)c.Resolve<IRedisClientsManager>().GetClient());

		    container.RegisterAutoWiredAs<DefaultRoutesService, IRoutesService>();

            container.RegisterAutoWiredAs<UnitOfWork, IUnitOfWork>();
            container.RegisterAutoWired<NotificationHub>();

             container.RegisterAutoWiredAs<Test, ITest>();
           //  container.Register(new SolrNetModule("http://localhost:8983/solr"));
           
           // container.RegisterFilterProvider();
            //Set MVC to use the same Funq IOC as ServiceStack
			ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
		}

		/* Uncomment to enable ServiceStack Authentication and CustomUserSession
		private void ConfigureAuth(Funq.Container container)
		{
			var appSettings = new AppSettings();

			//Default route: /auth/{provider}
			Plugins.Add(new AuthFeature(() => new CustomUserSession(),
				new IAuthProvider[] {
					new CredentialsAuthProvider(appSettings), 
					new FacebookAuthProvider(appSettings), 
					new TwitterAuthProvider(appSettings), 
					new BasicAuthProvider(appSettings), 
				})); 

			//Default route: /register
			Plugins.Add(new RegistrationFeature()); 

			//Requires ConnectionString configured in Web.Config
            var connectionString = ConfigurationManager.ConnectionStrings["AppDbContext"].ConnectionString;
			container.Register<IDbConnectionFactory>(c =>
				new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider));

			container.Register<IUserAuthRepository>(c =>
				new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));

			var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>();
			authRepo.CreateMissingTables();
		}*/
		

		public static void Start()
		{
			new AppHost().Init();
		}
	}
}