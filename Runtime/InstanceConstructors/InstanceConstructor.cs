using System;
using System.Runtime.CompilerServices;

namespace UnityInjector.InstanceConstructors {
    public abstract class InstanceConstructor {
        public abstract bool TryGetInstance(Type type, Container container, out object instance);
    }
}
