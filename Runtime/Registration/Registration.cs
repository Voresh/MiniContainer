using System;

namespace UnityInjector.Registration {
    public readonly struct Registration {
        public readonly Type ImplementationType;
        public readonly bool Cached;

        public Registration(Type implementationType, bool cached) {
            ImplementationType = implementationType;
            Cached = cached;
        }
    }
}
