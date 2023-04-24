using System;
using NUnit.Framework;

namespace MiniContainer.Tests.Editor
{
    public class ContainerTests
    {
        public class ClassA : IClassA
        { }

        public class ClassB
        { }

        public class ClassC
        {
            public int Number { get; }

            public ClassC(int number)
            {
                Number = number;
            }
        }

        public interface IClassA
        { }

        public class GenericClassA<T>
        { }

        public class GenericClassAChild : GenericClassA<ClassA>
        { }

        public class TwoGenericClassA<T, T1>
        { }

        public class DisposableClass : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }

        [Test]
        public void RegisterInstanceTest()
        {
            var container = new Container();
            container.RegisterInstance(new ClassA(), typeof(ClassA));
            var dependencyA = container.Resolve<ClassA>();
            Assert.IsNotNull(dependencyA);
        }

        [Test]
        public void RegisterTypeTest()
        {
            var container = new Container();
            container.Register(typeof(ClassA), typeof(ClassA));
            var dependencyA = container.Resolve<ClassA>();
            Assert.IsNotNull(dependencyA);
        }

        [Test]
        public void RegisterTypeInterfaceTest()
        {
            var container = new Container();
            container.Register(typeof(ClassA), typeof(IClassA));
            var dependencyInterfaceA = container.Resolve<IClassA>();
            Assert.IsNotNull(dependencyInterfaceA);
        }

        [Test]
        public void RegisterTypeSelfAndInterfaceTest()
        {
            var container = new Container();
            container.Register(typeof(ClassA), typeof(ClassA))
                .As<IClassA>();
            var dependencyA = container.Resolve<ClassA>();
            Assert.IsNotNull(dependencyA);
            var dependencyInterfaceA = container.Resolve<IClassA>();
            Assert.IsNotNull(dependencyInterfaceA);
        }

        [Test]
        public void RegisterInstanceInterfaceTest()
        {
            var container = new Container();
            container.RegisterInstance(new ClassA(), typeof(IClassA));
            var dependencyA = container.Resolve<IClassA>();
            Assert.IsNotNull(dependencyA);
        }
        
        [Test]
        public void RegisterInstanceSelfAndInterfaceTest()
        {
            var container = new Container();
            container.RegisterInstance(new ClassA(), typeof(ClassA))
                .As<IClassA>();
            var dependencyA = container.Resolve<ClassA>();
            Assert.IsNotNull(dependencyA);
            var dependencyInterfaceA = container.Resolve<IClassA>();
            Assert.IsNotNull(dependencyInterfaceA);
        }

        [Test]
        public void RegisterOpenGenericTest()
        {
            var container = new Container();
            container.Register(typeof(GenericClassA<>));
            var dependencyA = container.Resolve<GenericClassA<ClassA>>();
            Assert.IsNotNull(dependencyA);
        }

        [Test]
        public void RegisterOpenGenericsTest()
        {
            var container = new Container();
            container.Register(typeof(GenericClassA<>));
            var dependencyA = container.Resolve<GenericClassA<ClassA>>();
            var dependencyB = container.Resolve<GenericClassA<ClassB>>();
            Assert.IsNotNull(dependencyA);
            Assert.IsNotNull(dependencyB);
        }

        [Test]
        public void RegisterOpenGenericOverrideTest()
        {
            var container = new Container();
            container.Register(typeof(GenericClassA<>));
            container.Register(typeof(GenericClassAChild), typeof(GenericClassA<ClassA>));
            var dependencyA = container.Resolve<GenericClassA<ClassA>>();
            Assert.IsNotNull(dependencyA);
            Assert.That(dependencyA is GenericClassAChild);
        }

        [Test]
        public void RegisterTwoOpenGenericsTest()
        {
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
        public void RegisterTwoOpenGenericsNonCachedTest()
        {
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
        public void CachedDisposedTest()
        {
            var container = new Container();
            container.Register<DisposableClass>(true);
            var disposable = container.Resolve<DisposableClass>();
            container.Dispose();
            Assert.IsTrue(disposable.Disposed);
        }

        [Test]
        public void NonCachedNotDisposedTest()
        {
            var container = new Container();
            container.Register<DisposableClass>(false);
            var disposable = container.Resolve<DisposableClass>();
            container.Dispose();
            Assert.IsFalse(disposable.Disposed);
        }

        [Test]
        public void InstanceNotDisposedTest()
        {
            var container = new Container();
            container.RegisterInstance(new DisposableClass());
            var disposable = container.Resolve<DisposableClass>();
            container.Dispose();
            Assert.IsFalse(disposable.Disposed);
        }

        [Test]
        public void FuncFactoryTest()
        {
            var container = new Container();
            container.RegisterInstance<Func<int, ClassC>>(number => new ClassC(number));
            var classC = container.Resolve<Func<int, ClassC>>().Invoke(1);
            Assert.IsNotNull(classC);
            Assert.AreEqual(1, classC.Number);
        }
        
        [Test]
        public void FuncInterfaceFactoryTest()
        {
            var container = new Container();
            container.RegisterInstance<Func<IClassA>>(() => new ClassA());
            var classA = container.Resolve<Func<IClassA>>().Invoke();
            Assert.IsNotNull(classA);
        }

        [Test]
        public void ScopeTest()
        {
            var container = new Container();
            container.Register<ClassA>();
            using (var scopedContainer = new Container(container))
            {
                var classA = scopedContainer.Resolve<ClassA>();
                Assert.IsNotNull(classA);
                
                scopedContainer.Register<ClassB>();
                var classB = scopedContainer.Resolve<ClassB>();
                Assert.IsNotNull(classB);
                
                classB = null;
                try
                {
                    classB = container.Resolve<ClassB>();
                }
                catch (Exception)
                {
                    // ignored
                }
                Assert.IsNull(classB);
            }
        }
    }
}
