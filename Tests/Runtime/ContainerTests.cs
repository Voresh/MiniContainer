using EasyUnity;
using EasyUnity.InstanceConstructors;
using NUnit.Framework;

namespace Tests.Editor {
    public class ContainerTests {
        public class ClassA : IClassA { }
        public interface IClassA { }
        
        // [Test]
        // public void GeneratedInstanceConstructorTest() {
        //     var container = new Container();
        //     Container.SetInstanceConstructors(new EasyUnityTestsEditor_GeneratedInstanceConstructor());
        //     container.Register<ClassA>()
        //         .As<IClassA>();
        //     var dependencyA = container.Resolve<IClassA>();
        //     Assert.IsNotNull(dependencyA);
        // }
        
        [Test]
        public void RegisterInstanceTest() {
            var container = new Container();
            container.RegisterInstance(new ClassA());
            var dependencyA = container.Resolve<ClassA>();
            Assert.IsNotNull(dependencyA);
        }
        
        [Test]
        public void RegisterInstanceInterfaceTest() {
            var container = new Container();
            container.RegisterInstance(new ClassA())
                .As<IClassA>();
            var dependencyA = container.Resolve<IClassA>();
            Assert.IsNotNull(dependencyA);
        }
        
        //todo:
    }
}
