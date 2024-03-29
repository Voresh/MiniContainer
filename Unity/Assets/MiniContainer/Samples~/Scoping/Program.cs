﻿using DefaultNamespace;
using MiniContainer.InstanceConstructors;
using MiniContainer.Samples.Scoping.Services;
using UnityEngine;

namespace MiniContainer.Samples.Scoping
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
            container.SetInstanceConstructors(
                new AssemblyCSharp_GeneratedInstanceConstructor(),
                new ReflectionInstanceConstructor());
            container.Register<Service>();
            container.Register<AnotherService>();
            return container;
        }

        private static void Start(Container container)
        {
            using (var scopeContainer = new Container(container))
            {
                scopeContainer.Register<ScopedService>();

                scopeContainer.Resolve<ScopedService>();
            }
        }
    }
}
