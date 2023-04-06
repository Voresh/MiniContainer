using UnityEngine;

namespace Samples.Basic.Logger {
    public class Logger<T> : ILogger<T> {
        private static string Tag => $"[{typeof(T).Name}]";
        public void Log(string message) => Debug.Log($"{Tag} {message}");
    }
}
