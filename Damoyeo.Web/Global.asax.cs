using Autofac;
using Autofac.Integration.Mvc;
using System.Web.Mvc;
using System.Web.Routing;
using Damoyeo.DataAccess.Repository.IRepository;
using Damoyeo.DataAccess.Repository;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Damoyeo.Util.Manager;
using System.Web;
using System;
using Serilog;


namespace Damoyeo.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Autofac MVC ���� ���̵�
            //https://autofac.readthedocs.io/en/latest/integration/mvc.html
            //Autofac MVC ���� ����
            //https://github.com/autofac/Autofac.Mvc
            var builder = new ContainerBuilder();
            //���ø����̼� ���� �� Autofac �����̳ʸ� �����ϴ� ���� MVC ��Ʈ�ѷ��� �ش� ���� �׸��� ����ؾ� �մϴ�.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);


            // Model Binder ���
            // * �� ���δ�(Model Binder)�� ASP.NET MVC���� ���Ǵ� �������,
            // HTTP ��û �����͸� ��Ʈ�ѷ��� �׼� �޼��忡�� ����ϴ� �� ��ü�� ��ȯ�ϴ� ������ �մϴ�.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            //������ �Ϲ������� ����ϴ� �߻�Ŭ������ ���Ӽ����� ��ϵ˴ϴ�.
            //HttpContextBase, HttpRequestBase, HttpResponseBase, HttpServerUtilityBase .. Autofac MVC ���� ���̵� ����
            builder.RegisterModule<AutofacWebTypesModule>();

            //ViewPage�� ������ ���� �����ϰ� ���ݴϴ�.
            //���̾ƿ������������� ����� �� �����ϴ�.
            builder.RegisterSource(new ViewRegistrationSource());

            //�۾����Ϳ����� �Ӽ������� Ȱ��ȭ�մϴ�.
            builder.RegisterFilterProvider();

            //����α� ���
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.AppSettings()
            .CreateLogger();
            builder.RegisterInstance(Log.Logger).As<ILogger>();

            builder.Register(c => new SqlConnection(ConfigurationManager.ConnectionStrings["DamoyeoConnectionString"].ConnectionString))
            .As<IDbConnection>()
            .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();


            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error()
        {
            
            Exception exception = Server.GetLastError();
            HttpException httpException = exception as HttpException;
            // ���� ���� �ڵ带 �����ɴϴ�.
            int statusCode = (httpException != null) ? httpException.GetHttpCode() : 500;

            //404�� ������ �����ڵ忡 �α׸� ����ϴ�.
            if (statusCode != 404)
            {
                string errorMessage = $@"RawUrl[ {Request.RawUrl} ]
                                     UrlReferrer[ {Request.UrlReferrer} ]
                                     USER IP[ {Request.UserHostAddress} ]";
                
                
                Log.Logger.Error(errorMessage);
                if (httpException != null)
                {
                    Log.Logger.Error(httpException.Message);
                    Log.Logger.Error(httpException.StackTrace);
                }
                else
                {
                    Log.Logger.Error(exception.Message);
                    Log.Logger.Error(exception.StackTrace);
                }
                
            }
            
#if DEBUG

#else
            Response.Clear();
            Server.ClearError();
            HttpContext.Current.Server.TransferRequest($"/fileimages/Error/index.asp?code={statusCode}");
#endif
        }
    }
}
