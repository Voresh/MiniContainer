using MiniContainer.InstanceConstructors;
using MiniContainer.Samples.OpenGeneric.Logger;
using MiniContainer.Samples.OpenGeneric.Services;
using UnityEngine;

namespace MiniContainer.Samples.OpenGeneric
{
    public class Program
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Main()
        {
            var container = Build();
            Start(container);
        }

        private static Container Build()
        {
            var container = new Container();
            Container.SetInstanceConstructors(
                new AssemblyCSharp_GeneratedInstanceConstructor(),
                new ReflectionInstanceConstructor());
            container.Register<Service>();
            container.Register<AnotherService>();
            container.Register(typeof(Logger<>), typeof(ILogger<>));
            container.Register(typeof(AnotherLogger<AnotherService>), typeof(ILogger<AnotherService>));
            return container;
        }

        private static void Start(Container container)
        {
            container.Resolve<Service>();
            container.Resolve<AnotherService>();
        }
    }
}
