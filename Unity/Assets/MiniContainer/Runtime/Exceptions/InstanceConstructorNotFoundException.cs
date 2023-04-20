namespace MiniContainer.Exceptions
{
    public class InstanceConstructorNotFoundException : ContainerException
    {
        public InstanceConstructorNotFoundException(string message) : base(message) { }
    }
}
