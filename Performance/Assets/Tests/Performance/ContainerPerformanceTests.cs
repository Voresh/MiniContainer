using NUnit.Framework;
using Unity.PerformanceTesting;
using MiniContainer.InstanceConstructors;

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
            Container.SetInstanceConstructors(new ReflectionInstanceConstructor());
            Measure.Method(() =>
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        container.Resolve<ClassA>();
                    }
                })
                .SampleGroup("Reflection")
                .GC()
                .Run();
            Container.SetInstanceConstructors(new MiniContainerTestsPerformance_GeneratedInstanceConstructor());
            Measure.Method(() =>
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        container.Resolve<ClassA>();
                    }
                })
                .SampleGroup("CodeGen")
                .GC()
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
            Container.SetInstanceConstructors(new ReflectionInstanceConstructor());
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
            Container.SetInstanceConstructors(new MiniContainerTestsPerformance_GeneratedInstanceConstructor());
            Measure.Method(() =>
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        container.Resolve<ClassB>();
                    }
                })
                .SampleGroup("CodeGen")
                .GC()
                .Run();
        }
    }
}
