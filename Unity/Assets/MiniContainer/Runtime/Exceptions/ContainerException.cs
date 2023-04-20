using System;

namespace MiniContainer.Exceptions {
    public abstract class ContainerException : Exception {
        protected ContainerException(string message) : base(message) { }
    }
}
