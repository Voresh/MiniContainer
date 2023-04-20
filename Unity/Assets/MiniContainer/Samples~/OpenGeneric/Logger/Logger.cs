using UnityEngine;

namespace MiniContainer.Samples.OpenGeneric.Logger {
    public class Logger<T> : ILogger<T> {
        private static string Tag => $"[{typeof(T).Name}]";
        public void Log(string message) => Debug.Log($"{Tag} {message}");
    }
}
