using System;

namespace UnityInjector.Exceptions {
    public abstract class ContainerException : Exception {
        protected ContainerException(string message) : base(message) { }
    }
}
