using UnityEngine;
using UnityInjector.InstanceConstructors;
using UnityInjector.Samples.Basic.Logger;
using UnityInjector.Samples.Basic.Services;

namespace UnityInjector.Samples.Basic {
    public class Program {
        [RuntimeInitializeOnLoadMethod]
        private static void Main() {
            var container = Configure();
            Start(container);
        }

        private static Container Configure() {
            var container = new Container();
            Container.SetInstanceConstructors(
                new AssemblyCSharp_GeneratedInstanceConstructor(),
                new ReflectionInstanceConstructor());
            container.Register<Service>();
            container.Register<AnotherService, IAnotherService>();
            container.Register(typeof(Logger<>), typeof(ILogger<>), false);
            container.Register(typeof(AnotherLogger<AnotherService>), typeof(ILogger<AnotherService>));
            return container;
        }

        private static void Start(Container container) {
            container.Resolve<Service>();
        }
    }
}
