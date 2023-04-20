using MiniContainer.Samples.OpenGeneric.Logger;

namespace MiniContainer.Samples.OpenGeneric.Services {
    public class Service {
        public Service(ILogger<Service> logger) {
            logger.Log("constructor call");
        }
    }    
}
