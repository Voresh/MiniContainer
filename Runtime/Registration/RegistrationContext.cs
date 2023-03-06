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
        public RegistrationContext As<TInterface>() {
            if (!typeof(TInterface).IsAssignableFrom(_Type))
                throw new ArgumentException($"{typeof(TInterface)} not assignable from {_Type}");
            _ContainerDictionary.Add(typeof(TInterface), _Provider);
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext As(Type interfaceType) {
            if (!interfaceType.IsAssignableFrom(_Type))
                throw new ArgumentException($"{interfaceType} not assignable from {_Type}");
            _ContainerDictionary.Add(interfaceType, _Provider);
            return this;
        }
    
        internal RegistrationContext(Dictionary<Type, IProvider> containerDictionary, IProvider provider, Type type) {
            _ContainerDictionary = containerDictionary;
            _Provider = provider;
            _Type = type;
        }
    }
}
