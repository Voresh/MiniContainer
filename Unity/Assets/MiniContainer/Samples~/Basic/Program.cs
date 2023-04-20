using MiniContainer.InstanceConstructors;
using MiniContainer.Samples.Basic.Services;
using UnityEngine;

namespace MiniContainer.Samples.Basic
{
    public class Program
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Main()
        {
            var container = Configure();
            Start(container);
        }

        private static Container Configure()
        {
            var container = new Container();
            Container.SetInstanceConstructors(
                new AssemblyCSharp_GeneratedInstanceConstructor(),
                new ReflectionInstanceConstructor());
            container.Register<Service>();
            container.Register<AnotherService, IAnotherService>();
            return container;
        }

        private static void Start(Container container)
        {
            container.Resolve<Service>();
        }
    }
}
