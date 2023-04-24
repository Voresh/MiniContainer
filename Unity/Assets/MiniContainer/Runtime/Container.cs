using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MiniContainer.Exceptions;
using MiniContainer.InstanceConstructors;
using MiniContainer.Registration;

namespace MiniContainer
{
    public class Container : IDisposable
    {
        private readonly Container _parent;
        private readonly Dictionary<Type, Registration.Registration> _registrations
            = new Dictionary<Type, Registration.Registration>();
        private readonly Dictionary<Type, object> _instances
            = new Dictionary<Type, object>();
        private readonly HashSet<IDisposable> _disposables
            = new HashSet<IDisposable>();
        private readonly HashSet<InstanceConstructor> _instanceConstructors
            = new HashSet<InstanceConstructor>(1) { new ReflectionInstanceConstructor() };

        public Container(Container parent = null)
        {
            _parent = parent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetInstanceConstructors(params InstanceConstructor[] instanceConstructors)
        {
            _instanceConstructors.EnsureCapacity(instanceConstructors.Length);
            _instanceConstructors.Clear();
            foreach (var instanceConstructor in instanceConstructors)
                _instanceConstructors.Add(instanceConstructor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext RegisterInstance(object instance, Type interfaceType)
        {
#if !DISABLE_UNITY_INJECTOR_CONTAINER_EXCEPTIONS
            if (!interfaceType.IsInstanceOfType(instance))
                throw new ArgumentException($"{interfaceType} not assignable from {instance.GetType()}");
#endif
            var implementationRegistration = new Registration.Registration(instance.GetType(), true);
            _registrations.Add(interfaceType, implementationRegistration);
            _instances.Add(interfaceType, instance);
            return new RegistrationContext(_registrations, implementationRegistration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext Register(Type type, Type interfaceType, bool cached = true)
        {
#if !DISABLE_UNITY_INJECTOR_CONTAINER_EXCEPTIONS
            if (!interfaceType.IsAssignableFrom(type) && (!type.IsGenericTypeDefinition || !interfaceType.IsGenericTypeDefinition))
                throw new ArgumentException($"{interfaceType} not assignable from {type}");
            if (type.IsAbstract || type.IsInterface)
                throw new ArgumentException($"{type} is interface or abstract class");
#endif
            var implementationRegistration = new Registration.Registration(type, cached);
            _registrations.Add(interfaceType, implementationRegistration);
            return new RegistrationContext(_registrations, implementationRegistration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object CreateInstance(Type type)
        {
            foreach (var instanceConstructor in _instanceConstructors)
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
        public T Resolve<T>() => (T)Resolve(typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Resolve(Type type)
        {
            if (_instances.TryGetValue(type, out var instance))
                return instance;
            if (_registrations.TryGetValue(type, out var registration))
            {
                instance = CreateInstance(registration.ImplementationType);
                if (registration.Cached)
                {
                    _instances.Add(type, instance);
                    if (instance is IDisposable disposable)
                        _disposables.Add(disposable);
                }
                return instance;
            }
            if (type.IsGenericType
                && _registrations.TryGetValue(type.GetGenericTypeDefinition(), out registration))
            {
                var genericArguments = type.GetGenericArguments();
                var genericImplementationType = registration.ImplementationType.MakeGenericType(genericArguments);
                instance = CreateInstance(genericImplementationType);
                if (registration.Cached)
                {
                    _instances.Add(type, instance);
                    if (instance is IDisposable disposable)
                        _disposables.Add(disposable);
                }
                return instance;
            }
#if !DISABLE_UNITY_INJECTOR_CONTAINER_EXCEPTIONS
            if (_parent == null)
                throw new TypeNotRegisteredException($"{type} not registered in container");
#endif
            return _parent.Resolve(type);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
            _disposables.Clear();
        }
    }
}
