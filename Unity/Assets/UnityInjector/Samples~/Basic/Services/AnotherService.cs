using UnityEngine;

namespace UnityInjector.Samples.Basic.Services {
    public class AnotherService : IAnotherService {
        public AnotherService() {
            Debug.Log("constructor call");
        }
    }
}
