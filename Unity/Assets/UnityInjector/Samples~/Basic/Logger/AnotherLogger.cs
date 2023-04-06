using UnityEngine;

namespace UnityInjector.Samples.Basic.Logger {
    public class AnotherLogger<T> : ILogger<T> {
        private static string Tag => $"[{typeof(T).Name}]";
        public void Log(string message) => Debug.Log($"FromAnotherLogger: {Tag} {message}");
    }
}
