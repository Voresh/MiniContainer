using System;
using NUnit.Framework;
using UnityInjector.InstanceConstructors;

namespace UnityInjector.Tests.Editor {
    public class ContainerTests {
        public class ClassA : IClassA { }
        public class ClassB { }
        public interface IClassA { }
        public class GenericClassA<T> { }
        public class GenericClassAChild : GenericClassA<ClassA> { }
        public class TwoGenericClassA<T, T1> { }
        public class DisposableClass : IDisposable {
            public bool Disposed { get; private set; }

            public void Dispose() {
                Disposed = true;
            }
        }
        
        [SetUp]
        public void SetUpTest()
        {
            Container.SetInstanceConstructors(new ReflectionInstanceConstructor());
        }
        
        [Test]
        public void RegisterInstanceTest() {
            var container = new Container();
            container.RegisterInstance(new ClassA());
            var dependencyA = container.Resolve<ClassA>();
            Assert.IsNotNull(dependencyA);
        }

        [Test]
        public void RegisterTypeTest() {
            var container = new Container();
            container.Register<ClassA>();
            var dependencyA = container.Resolve<ClassA>();
            Assert.IsNotNull(dependencyA);
        }
        
        [Test]
        public void RegisterTypeInterfaceTest() {
            var container = new Container();
            container.Register<ClassA, IClassA>();
            var dependencyInterfaceA = container.Resolve<IClassA>();
            Assert.IsNotNull(dependencyInterfaceA);
        }
        
        [Test]
        public void RegisterTypeSelfAndInterfaceTest() {
            var container = new Container();
            container.Register<ClassA>()
                .As<IClassA>();
            var dependencyA = container.Resolve<ClassA>();
            Assert.IsNotNull(dependencyA);
            var dependencyInterfaceA = container.Resolve<IClassA>();
            Assert.IsNotNull(dependencyInterfaceA);
        }
        
        [Test]
        public void RegisterInstanceInterfaceTest() {
            var container = new Container();
            container.RegisterInstance(new ClassA())
                .As<IClassA>();
            var dependencyA = container.Resolve<IClassA>();
            Assert.IsNotNull(dependencyA);
        }
        
        [Test]
        public void RegisterOpenGenericTest() {
            var container = new Container();
            container.Register(typeof(GenericClassA<>));
            var dependencyA = container.Resolve<GenericClassA<ClassA>>();
            Assert.IsNotNull(dependencyA);
        }
        
        [Test]
        public void RegisterOpenGenericsTest() {
            var container = new Container();
            container.Register(typeof(GenericClassA<>));
            var dependencyA = container.Resolve<GenericClassA<ClassA>>();
            var dependencyB = container.Resolve<GenericClassA<ClassB>>();
            Assert.IsNotNull(dependencyA);
            Assert.IsNotNull(dependencyB);
        }
        
        [Test]
        public void RegisterOpenGenericOverrideTest() {
            var container = new Container();
            container.Register(typeof(GenericClassA<>));
            container.Register(typeof(GenericClassAChild), typeof(GenericClassA<ClassA>));
            var dependencyA = container.Resolve<GenericClassA<ClassA>>();
            Assert.IsNotNull(dependencyA);
            Assert.That(dependencyA is GenericClassAChild);
        }
        
        [Test]
        public void RegisterTwoOpenGenericsTest() {
            var container = new Container();
            container.Register(typeof(TwoGenericClassA<,>));
            var dependencyAB = container.Resolve<TwoGenericClassA<ClassA, ClassB>>();
            Assert.IsNotNull(dependencyAB);
            var dependencyBA = container.Resolve<TwoGenericClassA<ClassB, ClassA>>();
            Assert.IsNotNull(dependencyBA);
            var dependencyAB2 = container.Resolve<TwoGenericClassA<ClassA, ClassB>>();
            Assert.That(dependencyAB == dependencyAB2);
            var dependencyBA2 = container.Resolve<TwoGenericClassA<ClassB, ClassA>>();
            Assert.That(dependencyBA == dependencyBA2);
        }
        
        [Test]
        public void RegisterTwoOpenGenericsNonCachedTest() {
            var container = new Container();
            container.Register(typeof(TwoGenericClassA<,>), false);
            var dependencyAB = container.Resolve<TwoGenericClassA<ClassA, ClassB>>();
            Assert.IsNotNull(dependencyAB);
            var dependencyBA = container.Resolve<TwoGenericClassA<ClassB, ClassA>>();
            Assert.IsNotNull(dependencyBA);
            var dependencyAB2 = container.Resolve<TwoGenericClassA<ClassA, ClassB>>();
            Assert.That(dependencyAB != dependencyAB2);
            var dependencyBA2 = container.Resolve<TwoGenericClassA<ClassB, ClassA>>();
            Assert.That(dependencyBA != dependencyBA2);
        }
        
        [Test]
        public void CachedDisposedTest() {
            var container = new Container();
            container.Register<DisposableClass>(true);
            var disposable = container.Resolve<DisposableClass>();
            container.Dispose();
            Assert.IsTrue(disposable.Disposed);
        }
        
        [Test]
        public void NonCachedNotDisposedTest() {
            var container = new Container();
            container.Register<DisposableClass>(false);
            var disposable = container.Resolve<DisposableClass>();
            container.Dispose();
            Assert.IsFalse(disposable.Disposed);
        }
        
        [Test]
        public void InstanceNotDisposedTest() {
            var container = new Container();
            container.RegisterInstance(new DisposableClass());
            var disposable = container.Resolve<DisposableClass>();
            container.Dispose();
            Assert.IsFalse(disposable.Disposed);
        }
    }
}
