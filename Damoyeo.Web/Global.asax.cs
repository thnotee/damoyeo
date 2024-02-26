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
            //Autofac MVC 통합 가이드
            //https://autofac.readthedocs.io/en/latest/integration/mvc.html
            //Autofac MVC 빠른 시작
            //https://github.com/autofac/Autofac.Mvc
            var builder = new ContainerBuilder();
            //애플리케이션 시작 시 Autofac 컨테이너를 빌드하는 동안 MVC 컨트롤러와 해당 종속 항목을 등록해야 합니다.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Model Binder 등록
            // * 모델 바인더(Model Binder)는 ASP.NET MVC에서 사용되는 기능으로,
            // HTTP 요청 데이터를 컨트롤러의 액션 메서드에서 사용하는 모델 객체로 변환하는 역할을 합니다.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            //웹에서 일반적으로 사용하는 추상클래스가 종속성으로 등록됩니다.
            //HttpContextBase, HttpRequestBase, HttpResponseBase, HttpServerUtilityBase .. Autofac MVC 통합 가이드 참조
            builder.RegisterModule<AutofacWebTypesModule>();

            //ViewPage에 의존성 주입 가능하게 해줍니다.
            //레이아웃페이지에서는 사용할 수 없습니다.
            builder.RegisterSource(new ViewRegistrationSource());

            //작업필터에대한 속성주입을 활성화합니다.
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
