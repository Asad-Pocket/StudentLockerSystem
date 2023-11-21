using Autofac;
using StudentLockerSystem.Models;

namespace StudentLockerSystem
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegisterViewModel>().AsSelf();

            builder.RegisterType<LoginViewModel>().AsSelf();

            base.Load(builder);
        }
    }
}
