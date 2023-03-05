using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EasyUnity.Providers;

namespace EasyUnity.Registration {
    public readonly struct RegistrationContext {
        private readonly Dictionary<Type, IProvider> _ContainerDictionary;
        private readonly IProvider _Provider;
        private readonly Type _Type;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void As<TInterface>() {
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_ContainerDictionary.ContainsKey(typeof(TInterface)))
                throw new Exception($"{typeof(TInterface)} already registered in container");
#endif
            if (!typeof(TInterface).IsAssignableFrom(_Type))
                throw new ArgumentException($"{typeof(TInterface)} not assignable from {_Type}");
            _ContainerDictionary.Add(typeof(TInterface), _Provider);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void As(Type @interface) {
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_ContainerDictionary.ContainsKey(@interface))
                throw new Exception($"{@interface} already registered in container");
#endif
            if (!@interface.IsAssignableFrom(_Type))
                throw new ArgumentException($"{@interface} not assignable from {_Type}");
            _ContainerDictionary.Add(@interface, _Provider);
        }
    
        internal RegistrationContext(Dictionary<Type, IProvider> containerDictionary, IProvider provider, Type type) {
            _ContainerDictionary = containerDictionary;
            _Provider = provider;
            _Type = type;
        }
    }
    
    public readonly struct RegistrationContext<TType> where TType : class {
        private readonly Dictionary<Type, IProvider> _ContainerDictionary;
        private readonly IProvider _Provider;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void As<TInterface>() {
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_ContainerDictionary.ContainsKey(typeof(TInterface)))
                throw new Exception($"{typeof(TInterface)} already registered in container");
#endif
            if (!typeof(TInterface).IsAssignableFrom(typeof(TType)))
                throw new ArgumentException($"{typeof(TInterface)} not assignable from {typeof(TType)}");
            _ContainerDictionary.Add(typeof(TInterface), _Provider);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void As(Type @interface) {
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_ContainerDictionary.ContainsKey(@interface))
                throw new Exception($"{@interface} already registered in container");
#endif
            if (!@interface.IsAssignableFrom(typeof(TType)))
                throw new ArgumentException($"{@interface} not assignable from {typeof(TType)}");
            _ContainerDictionary.Add(@interface, _Provider);
        }

        internal RegistrationContext(Dictionary<Type, IProvider> containerDictionary, IProvider provider) {
            _ContainerDictionary = containerDictionary;
            _Provider = provider;
        }
    }
}
