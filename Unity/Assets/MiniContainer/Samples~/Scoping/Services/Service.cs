using UnityEngine;

namespace MiniContainer.Samples.Scoping.Services {
    public class Service {
        public Service(AnotherService anotherService) {
            Debug.Log($"constructor call, got {anotherService}");
        }
    }    
}
