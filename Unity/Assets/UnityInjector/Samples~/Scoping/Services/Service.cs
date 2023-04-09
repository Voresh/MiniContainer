using UnityEngine;

namespace UnityInjector.Samples.Scoping.Services {
    public class Service {
        public Service(AnotherService anotherService) {
            Debug.Log($"constructor call, got {anotherService}");
        }
    }    
}
