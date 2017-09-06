using Nancy;
using Nancy.TinyIoc;
using DemoApi.data;
using Nancy.Bootstrapper;
using Nancy.Json;

namespace DemoApi
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<ILoadDemoData<HockeyStat>, DemoData>();
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            JsonSettings.MaxJsonLength = int.MaxValue;
        }
    }
}