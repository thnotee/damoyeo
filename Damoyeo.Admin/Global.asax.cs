using Autofac;
using Autofac.Integration.Mvc;
using Damoyeo.DataAccess.Repository;
using Damoyeo.DataAccess.Repository.IRepository;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Routing;

namespace Damoyeo.Admin
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

            builder.Register(c => new SqlConnection(ConfigurationManager.ConnectionStrings["DamoyeoConnectionString"].ConnectionString))
            .As<IDbConnection>()
            .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}