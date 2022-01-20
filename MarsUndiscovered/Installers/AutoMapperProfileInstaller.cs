using AutoMapper;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace MarsUndiscovered.Installers
{
    public class AutoMapperProfileInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Register all mapper profiles
            container.Register(
                Classes.FromAssemblyInThisApplication(GetType().Assembly)
                    .BasedOn<Profile>().WithServiceBase());
        }
    }
}
