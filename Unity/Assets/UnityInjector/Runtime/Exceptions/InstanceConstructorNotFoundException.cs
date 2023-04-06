namespace UnityInjector.Exceptions {
    public class InstanceConstructorNotFoundException : ContainerException {
        public InstanceConstructorNotFoundException(string message) : base(message) { }
    }
}
