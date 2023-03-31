using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityInjector.Exceptions;
using UnityInjector.InstanceConstructors;
using UnityInjector.Providers;
using UnityInjector.Registration;

namespace UnityInjector {
    public class Container : IDisposable {
        private readonly Container _Parent;
        private readonly Dictionary<Type, IProvider> _Providers
            = new Dictionary<Type, IProvider>();
        private readonly RegistrationContextProviders<IProvider> _RegistrationContextProviders;
        private readonly Dictionary<Type, IOpenGenericProvider> _OpenGenericProviders
            = new Dictionary<Type, IOpenGenericProvider>();
        private readonly RegistrationContextProviders<IOpenGenericProvider> _RegistrationContextOpenGenericProviders;
        private readonly HashSet<IDisposable> _Disposables 
            = new HashSet<IDisposable>();
        private static readonly HashSet<InstanceConstructor> _InstanceConstructors
            = new HashSet<InstanceConstructor> { new ReflectionInstanceConstructor() };

        public Container(Container parent = null) {
            _Parent = parent;
            _RegistrationContextProviders 
                = new RegistrationContextProviders<IProvider>(_Providers);
            _RegistrationContextOpenGenericProviders 
                = new RegistrationContextProviders<IOpenGenericProvider>(_OpenGenericProviders);
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
            _Providers.Add(interfaceType, provider);
            return new RegistrationContext(_RegistrationContextProviders, provider, instance.GetType());
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
            if (!interfaceType.IsAssignableFrom(type) && (!type.IsGenericTypeDefinition || !interfaceType.IsGenericTypeDefinition))
                throw new ArgumentException($"{interfaceType} not assignable from {type}");
#endif
            if (!interfaceType.IsGenericTypeDefinition) {
                IProvider provider = cached
                    ? new CachedProvider(this, type)
                    : new NonCachedProvider(this, type);
                _Providers.Add(interfaceType, provider);
                return new RegistrationContext(_RegistrationContextProviders, provider, type);
            }
            else {
                IOpenGenericProvider openGenericProvider = cached
                    ? new OpenGenericCachedProvider(this, type)
                    : new OpenGenericNonCachedProvider(this, type);
                _OpenGenericProviders.Add(interfaceType, openGenericProvider);
                return new RegistrationContext(_RegistrationContextOpenGenericProviders, openGenericProvider, type);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object CreateInstance(Type type) {
            foreach (var instanceConstructor in _InstanceConstructors)
                if (instanceConstructor.TryGetInstance(type, this, out var instance))
                    return instance;
#if !DISABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            throw new InstanceConstructorNotFoundException($"no instance constructor found for {type}");
#endif
#pragma warning disable CS0162
            return null;
#pragma warning restore CS0162
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Resolve<T>() {
            return (T) Resolve(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Resolve(Type type) {
            if (_Providers.TryGetValue(type, out var provider)) {
                var instance = provider.GetInstance();
                if (provider.Tracked && instance is IDisposable disposable)
                    _Disposables.Add(disposable);
                return instance;
            }
            if (type.IsGenericType 
                && _OpenGenericProviders.TryGetValue(type.GetGenericTypeDefinition(), out var openGenericProvider)) {
                var instance = openGenericProvider.GetInstance(type.GetGenericArguments());
                if (openGenericProvider.Tracked && instance is IDisposable disposable)
                    _Disposables.Add(disposable);
                return instance;
            }
#if !DISABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_Parent == null)
                throw new TypeNotRegisteredException($"{type} not registered in container");
#endif
            return _Parent.Resolve(type);
        }

        public void Dispose() {
            foreach (var disposable in _Disposables)
                disposable.Dispose();
            _Disposables.Clear();
        }
    }
}