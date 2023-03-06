using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EasyUnity.Exceptions;
using EasyUnity.InstanceConstructors;
using EasyUnity.Providers;
using EasyUnity.Registration;

namespace EasyUnity {
    public class Container : IDisposable {
        private readonly Container _Parent;
        private readonly Dictionary<Type, IProvider> _Container
            = new Dictionary<Type, IProvider>();
        private readonly HashSet<object> _ResolvedObjects 
            = new HashSet<object>();
        private static readonly HashSet<InstanceConstructor> _InstanceConstructors
            = new HashSet<InstanceConstructor> { new ReflectionInstanceConstructor() };

        public Container(Container parent = null) {
            _Parent = parent;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInstanceConstructors(params InstanceConstructor[] instanceConstructors) {
            _InstanceConstructors.Clear();
            foreach (var instanceConstructor in instanceConstructors)
                _InstanceConstructors.Add(instanceConstructor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext RegisterInstance<TType>(TType instance) {
            return RegisterInstance(instance, typeof(TType));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext RegisterInstance<TType, TInterface>(TType instance) where TType : TInterface {
            return RegisterInstance(instance, typeof(TInterface));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext RegisterInstance(object instance, Type interfaceType) {
#if !DISABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (!interfaceType.IsInstanceOfType(instance))
                throw new ArgumentException($"{interfaceType} not assignable from {instance.GetType()}");
#endif
            var provider = new InstanceProvider(instance);
            _Container.Add(interfaceType, provider);
            _ResolvedObjects.Add(instance);
            return new RegistrationContext(_Container, provider, instance.GetType());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext Register<TType, TInterface>(bool cached = true) where TType : TInterface {
            return Register(typeof(TType), typeof(TInterface), cached);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext Register<TType>(bool cached = true) {
            return Register(typeof(TType), cached);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext Register(Type type, bool cached = true) {
            return Register(type, type, cached);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext Register(Type type, Type interfaceType, bool cached = true) {
#if !DISABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (!interfaceType.IsAssignableFrom(type))
                throw new ArgumentException($"{interfaceType} not assignable from {type}");
#endif
            IProvider provider = cached
                ? new CachedProvider(this, type) 
                : new NonCachedProvider(this, type);
            _Container.Add(interfaceType, provider);
            return new RegistrationContext(_Container, provider, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object CreateInstance(Type type) {
            foreach (var instanceConstructor in _InstanceConstructors)
                if (instanceConstructor.TryGetInstance(type, this, out var instance))
                    return instance;
#if !DISABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            throw new InstanceConstructorNotFoundException($"no instance constructor found for {type}");
#endif
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Resolve<T>() {
            return (T) Resolve(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Resolve(Type type) {
            if (_Container.TryGetValue(type, out var provider)) {
                var instance = provider.GetInstance();
                _ResolvedObjects.Add(instance);
                return instance;
            }
#if !DISABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_Parent == null)
                throw new TypeNotRegisteredException($"{type} not registered in container");
#endif
            return _Parent.Resolve(type);
        }

        public void Dispose() {
            foreach (var resolvedObject in _ResolvedObjects)
                if (resolvedObject is IDisposable disposable)
                    disposable.Dispose();
            _ResolvedObjects.Clear();
        }
    }
}