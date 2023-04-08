using UnityInjector.Samples.OpenGeneric.Logger;

namespace UnityInjector.Samples.OpenGeneric.Services {
    public class Service {
        public Service(ILogger<Service> logger) {
            logger.Log("constructor call");
        }
    }    
}
