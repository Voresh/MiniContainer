using System;

namespace UnityInjector.InstanceConstructors {
    public abstract class InstanceConstructor {
        public abstract bool TryGetInstance(Type type, Container container, out object instance);
    }
}
