using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MiniContainer.Registration
{
    public readonly struct RegistrationContext
    {
        private readonly Dictionary<Type, Registration> _registrations;
        private readonly Registration _implementationRegistration;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext As<TInterface>() => As(typeof(TInterface));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext As(Type interfaceType)
        {
#if !DISABLE_UNITY_INJECTOR_CONTAINER_EXCEPTIONS
            if (!interfaceType.IsAssignableFrom(_implementationRegistration.ImplementationType))
                throw new ArgumentException($"{interfaceType} not assignable from {_implementationRegistration.ImplementationType}");
#endif
            _registrations.Add(interfaceType, _implementationRegistration);
            return this;
        }

        internal RegistrationContext(
            Dictionary<Type, Registration> registrations,
            Registration implementationRegistration)
        {
            _registrations = registrations;
            _implementationRegistration = implementationRegistration;
        }
    }
}
