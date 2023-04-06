using UnityInjector.Samples.Basic.Logger;

namespace UnityInjector.Samples.Basic.Services {
    public class Service {
        public Service(IAnotherService anotherService, ILogger<Service> logger) {
            logger.Log("constructor call");
        }
    }    
}
