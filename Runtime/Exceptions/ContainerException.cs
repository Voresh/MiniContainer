using System;

namespace EasyUnity.Exceptions {
    public abstract class ContainerException : Exception {
        protected ContainerException(string message) : base(message) { }
    }
}
