using MiniContainer.Samples.OpenGeneric.Logger;

namespace MiniContainer.Samples.OpenGeneric.Services {
    public class AnotherService {
        public AnotherService(ILogger<AnotherService> logger) {
            logger.Log("constructor call");
        }
    }
}
