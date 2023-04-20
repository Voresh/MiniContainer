using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MiniContainer.Registration {
    public readonly struct RegistrationContext {
        private readonly Dictionary<Type,Registration> _Registrations;
        private readonly Registration _ImplementationRegistration;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext As<TInterface>()
            => As(typeof(TInterface));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext As(Type interfaceType) {
#if !DISABLE_UNITY_INJECTOR_CONTAINER_EXCEPTIONS
            if (!interfaceType.IsAssignableFrom(_ImplementationRegistration.ImplementationType))
                throw new ArgumentException($"{interfaceType} not assignable from {_ImplementationRegistration.ImplementationType}");
#endif
            _Registrations.Add(interfaceType, _ImplementationRegistration);
            return this;
        }
    
        internal RegistrationContext(
            Dictionary<Type, Registration> registrations,
            Registration implementationRegistration) {
            _Registrations = registrations;
            _ImplementationRegistration = implementationRegistration;
        }
    }
}
