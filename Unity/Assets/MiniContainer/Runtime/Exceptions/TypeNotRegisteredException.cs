namespace MiniContainer.Exceptions {
    public class TypeNotRegisteredException : ContainerException {
        public TypeNotRegisteredException(string message) : base(message) { }
    }
}
