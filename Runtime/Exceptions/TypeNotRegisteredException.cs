namespace EasyUnity.Exceptions {
    public class TypeNotRegisteredException : ContainerException {
        public TypeNotRegisteredException(string message) : base(message) { }
    }
}
