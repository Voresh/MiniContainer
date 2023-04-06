using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityInjector.InstanceConstructors;

namespace UnityInjector.Tests.Runtime {
    public class DependencyA {
        public DependencyA() { }
    }

    public class DependencyB {
        public DependencyB(DependencyA dependencyA, DependencyC dependencyC) { }
    }

    public class DependencyC {
        public DependencyC(DependencyD dependencyD) { }
    }
    
    public class DependencyD {
        public DependencyD() { }
    }
    
    public class ContainerPerformanceTests {
        [Test, Performance]
        public void ResolveNonCachedTest() {
            var container = new Container();
            container.Register<DependencyA>(false);
            Container.SetInstanceConstructors(new ReflectionInstanceConstructor());
            Measure.Method(() => {
                    for (var i = 0; i < 10000; i++) {
                        container.Resolve<DependencyA>();
                    }
                })
                .SampleGroup("Reflection")
                .GC()
                .Run();
            Container.SetInstanceConstructors(new UnityInjectorTestsEditor_GeneratedInstanceConstructor());
            Measure.Method(() => {
                    for (var i = 0; i < 10000; i++) {
                        container.Resolve<DependencyA>();
                    }
                })
                .SampleGroup("CodeGen")
                .GC()
                .Run();
        }
        
        [Test, Performance]
        public void ResolveNonCachedComplexTest() {
            var container = new Container();
            container.Register<DependencyA>(false);
            container.Register<DependencyB>(false);
            container.Register<DependencyC>(false);
            container.Register<DependencyD>(false);
            Container.SetInstanceConstructors(new ReflectionInstanceConstructor());
            Measure.Method(() => {
                    for (var i = 0; i < 10000; i++) {
                        container.Resolve<DependencyB>();
                    }
                })
                .SampleGroup("Reflection")
                .GC()
                .Run();
            Container.SetInstanceConstructors(new UnityInjectorTestsEditor_GeneratedInstanceConstructor());
            Measure.Method(() => {
                    for (var i = 0; i < 10000; i++) {
                        container.Resolve<DependencyB>();
                    }
                })
                .SampleGroup("CodeGen")
                .GC()
                .Run();
        }
    }
}
