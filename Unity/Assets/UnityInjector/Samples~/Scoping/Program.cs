using DefaultNamespace;
using UnityEngine;
using UnityInjector.InstanceConstructors;
using UnityInjector.Samples.Scoping.Services;

namespace UnityInjector.Samples.Scoping {
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
            container.Register<AnotherService>();
            return container;
        }

        private static void Start(Container container) {
            using (var scopeContainer = new Container(container)) {
                scopeContainer.Register<ScopedService>();
                
                scopeContainer.Resolve<ScopedService>();
            }
        }
    }
}
