using NUnit.Framework;
using Unity.PerformanceTesting;
using MiniContainer.InstanceConstructors;
using VContainer;
using Zenject;

namespace MiniContainer.Performance.Tests.Runtime
{
    public class ClassA
    {
        public ClassA() { }
    }

    public class ClassB
    {
        public ClassB(ClassA classA, ClassC classC) { }
    }

    public class ClassC
    {
        public ClassC(ClassD classD) { }
    }

    public class ClassD
    {
        public ClassD() { }
    }

    public class ContainerPerformanceTests
    {
        [Test, Performance]
        public void ResolveNonCachedTest()
        {
            var container = new Container();
            container.Register<ClassA>(false);
            container.SetInstanceConstructors(new ReflectionInstanceConstructor());
            Measure.Method(() =>
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        container.Resolve<ClassA>();
                    }
                })
                .SampleGroup("Reflection")
                .Run();
            container.SetInstanceConstructors(new MiniContainerTestsPerformance_GeneratedInstanceConstructor());
            Measure.Method(() =>
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        container.Resolve<ClassA>();
                    }
                })
                .SampleGroup("MiniContainer")
                .Run();
            var zenjectContainer = new DiContainer();
            zenjectContainer.Bind<ClassA>().AsTransient().Lazy();
            Measure.Method(() =>
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        zenjectContainer.Resolve<ClassA>();
                    }
                })
                .SampleGroup("Zenject")
                .Run();
            var builder = new ContainerBuilder();
            builder.Register<ClassA>(Lifetime.Transient);
            var vContainer = builder.Build();
            Measure.Method(() =>
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        vContainer.Resolve<ClassA>();
                    }
                })
                .SampleGroup("VContainer")
                .Run();
        }

        [Test, Performance]
        public void ResolveNonCachedComplexTest()
        {
            var container = new Container();
            container.Register<ClassA>(false);
            container.Register<ClassB>(false);
            container.Register<ClassC>(false);
            container.Register<ClassD>(false);
            container.SetInstanceConstructors(new ReflectionInstanceConstructor());
            Measure.Method(() =>
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        container.Resolve<ClassB>();
                    }
                })
                .SampleGroup("Reflection")
                .GC()
                .Run();
            container.SetInstanceConstructors(new MiniContainerTestsPerformance_GeneratedInstanceConstructor());
            Measure.Method(() =>
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        container.Resolve<ClassB>();
                    }
                })
                .SampleGroup("MiniContainer")
                .GC()
                .Run();
            var zenjectContainer = new DiContainer();
            zenjectContainer.Bind<ClassA>().AsTransient().Lazy();
            zenjectContainer.Bind<ClassB>().AsTransient().Lazy();
            zenjectContainer.Bind<ClassC>().AsTransient().Lazy();
            zenjectContainer.Bind<ClassD>().AsTransient().Lazy();
            Measure.Method(() =>
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        zenjectContainer.Resolve<ClassB>();
                    }
                })
                .SampleGroup("Zenject")
                .GC()
                .Run();
            var builder = new ContainerBuilder();
            builder.Register<ClassA>(Lifetime.Transient);
            builder.Register<ClassB>(Lifetime.Transient);
            builder.Register<ClassC>(Lifetime.Transient);
            builder.Register<ClassD>(Lifetime.Transient);
            var vContainer = builder.Build();
            Measure.Method(() =>
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        vContainer.Resolve<ClassB>();
                    }
                })
                .SampleGroup("VContainer")
                .GC()
                .Run();
        }

        [Test, Performance]
        public void RegisterComplexTest()
        {
            Measure.Method(() =>
                {
                    var container = new Container();
                    container.Register<ClassA>(false);
                    container.Register<ClassB>(false);
                    container.Register<ClassC>(false);
                    container.Register<ClassD>(false);
                })
                .SampleGroup("MiniContainer")
                .GC()
                .Run();
            Measure.Method(() =>
                {
                    var zenjectContainer = new DiContainer();
                    zenjectContainer.Bind<ClassA>().AsTransient().Lazy();
                    zenjectContainer.Bind<ClassB>().AsTransient().Lazy();
                    zenjectContainer.Bind<ClassC>().AsTransient().Lazy();
                    zenjectContainer.Bind<ClassD>().AsTransient().Lazy();
                })
                .SampleGroup("Zenject")
                .GC()
                .Run();
            Measure.Method(() =>
                {
                    var builder = new ContainerBuilder();
                    builder.Register<ClassA>(Lifetime.Transient);
                    builder.Register<ClassB>(Lifetime.Transient);
                    builder.Register<ClassC>(Lifetime.Transient);
                    builder.Register<ClassD>(Lifetime.Transient);
                    builder.Build();
                })
                .SampleGroup("VContainer")
                .GC()
                .Run();
        }
    }
}
