using System;
using System.Runtime.CompilerServices;

namespace EasyUnity.Registration {
    public readonly struct RegistrationContext {
        private readonly IRegistrationContextProviders _Providers;
        private readonly object _Provider;
        private readonly Type _Type;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext As<TInterface>() {
            return As(typeof(TInterface));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext As(Type interfaceType) {
            if (!interfaceType.IsAssignableFrom(_Type))
                throw new ArgumentException($"{interfaceType} not assignable from {_Type}");
            _Providers.Add(interfaceType, _Provider);
            return this;
        }
    
        internal RegistrationContext(IRegistrationContextProviders providers, object provider, Type type) {
            _Providers = providers;
            _Provider = provider;
            _Type = type;
        }
    }
}
