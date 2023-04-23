using System;
using MiniContainer.InstanceConstructors;
using MiniContainer.Samples.Factory.Production;
using MiniContainer.Samples.Factory.Services;
using UnityEngine;

namespace MiniContainer.Samples.Factory
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
            container.RegisterInstance<Func<int, FactoryProduction>>(
                _ => new FactoryProduction(container.Resolve<AnotherService>(), _));
            container.RegisterInstance<Func<Type, object>>(container.CreateInstance); // abstract factory
            return container;
        }

        private static void Start(Container container)
        {
            container.Resolve<Service>();
        }
    }
}
