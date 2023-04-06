using UnityInjector.Samples.Basic.Logger;

namespace UnityInjector.Samples.Basic.Services {
    public class AnotherService : IAnotherService {
        public AnotherService(ILogger<AnotherService> logger) {
            logger.Log("constructor call");
        }
    }
}
