using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityInjector.Exceptions;
using UnityInjector.InstanceConstructors;
using UnityInjector.Registration;

namespace UnityInjector {
    public class Container : IDisposable {
        private readonly Container _Parent;
        private readonly Dictionary<Type, Registration.Registration> _Registrations
            = new Dictionary<Type, Registration.Registration>();
        private readonly Dictionary<Type, object> _Instances
            = new Dictionary<Type, object>();
        private readonly HashSet<IDisposable> _Disposables 
            = new HashSet<IDisposable>();
        private static readonly HashSet<InstanceConstructor> _InstanceConstructors
            = new HashSet<InstanceConstructor>(1) { new ReflectionInstanceConstructor() };

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
#if !DISABLE_UNITY_INJECTOR_CONTAINER_EXCEPTIONS
            if (!interfaceType.IsInstanceOfType(instance))
                throw new ArgumentException($"{interfaceType} not assignable from {instance.GetType()}");
#endif
            var implementationRegistration = new Registration.Registration(instance.GetType(), true);
            _Registrations.Add(interfaceType, implementationRegistration);
            _Instances.Add(interfaceType, instance);
            return new RegistrationContext(_Registrations, implementationRegistration);
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
#if !DISABLE_UNITY_INJECTOR_CONTAINER_EXCEPTIONS
            if (!interfaceType.IsAssignableFrom(type) && (!type.IsGenericTypeDefinition || !interfaceType.IsGenericTypeDefinition))
                throw new ArgumentException($"{interfaceType} not assignable from {type}");
            if (type.IsAbstract || type.IsInterface)
                throw new ArgumentException($"{type} is interface or abstract class");
#endif
            var implementationRegistration = new Registration.Registration(type, cached);
            _Registrations.Add(interfaceType, implementationRegistration);
            return new RegistrationContext(_Registrations, implementationRegistration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object CreateInstance(Type type) {
            foreach (var instanceConstructor in _InstanceConstructors)
                if (instanceConstructor.TryGetInstance(type, this, out var instance))
                    return instance;
#if !DISABLE_UNITY_INJECTOR_CONTAINER_EXCEPTIONS
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
            if (_Instances.TryGetValue(type, out var instance))
                return instance;
            if (_Registrations.TryGetValue(type, out var registration)) {
                instance = CreateInstance(registration.ImplementationType);
                if (registration.Cached) {
                    _Instances.Add(type, instance);
                    if (instance is IDisposable disposable)
                        _Disposables.Add(disposable);
                }
                return instance;
            }
            if (type.IsGenericType 
                && _Registrations.TryGetValue(type.GetGenericTypeDefinition(), out registration)) {
                var genericArguments = type.GetGenericArguments();
                var genericImplementationType = registration.ImplementationType.MakeGenericType(genericArguments);
                instance = CreateInstance(genericImplementationType);
                if (registration.Cached) {
                    _Instances.Add(type, instance);
                    if (instance is IDisposable disposable)
                        _Disposables.Add(disposable);
                }
                return instance;
            }
#if !DISABLE_UNITY_INJECTOR_CONTAINER_EXCEPTIONS
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