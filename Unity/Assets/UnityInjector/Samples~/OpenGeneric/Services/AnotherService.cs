using UnityInjector.Samples.OpenGeneric.Logger;

namespace UnityInjector.Samples.OpenGeneric.Services {
    public class AnotherService {
        public AnotherService(ILogger<AnotherService> logger) {
            logger.Log("constructor call");
        }
    }
}
