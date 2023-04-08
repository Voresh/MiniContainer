using System;
using UnityEngine;
using UnityInjector.InstanceConstructors;
using UnityInjector.Samples.Factory.Production;
using UnityInjector.Samples.Factory.Services;

namespace UnityInjector.Samples.Factory {
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
            container.RegisterInstance<Func<int, FactoryProduction>>(
                _ => new FactoryProduction(container.Resolve<AnotherService>(), _));
            container.RegisterInstance<Func<Type, object>>(container.CreateInstance); // abstract factory
            return container;
        }
        
        private static void Start(Container container) {
            container.Resolve<Service>();
        }
    }
}
