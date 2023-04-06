using Samples.Basic.Logger;

namespace Samples.Basic.Services {
    public class AnotherService : IAnotherService {
        public AnotherService(ILogger<AnotherService> logger) {
            logger.Log("constructor call");
        }
    }
}
