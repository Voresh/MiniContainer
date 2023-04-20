namespace MiniContainer.Samples.OpenGeneric.Logger
{
    public interface ILogger<T>
    {
        void Log(string message);
    }
}
