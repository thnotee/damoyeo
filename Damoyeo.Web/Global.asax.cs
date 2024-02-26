using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

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

            builder.RegisterAssemblyTypes(Assembly.Load("Damoyeo.Data"))
                .Where(t=>t.Namespace.Contains("DataAccess"))
                .As(t=>t.GetInterfaces().FirstOrDefault(i=>i.Name == "I" + t.Name));

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
