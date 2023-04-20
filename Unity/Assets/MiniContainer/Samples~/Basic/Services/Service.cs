using UnityEngine;

namespace MiniContainer.Samples.Basic.Services
{
    public class Service
    {
        public Service(IAnotherService anotherService)
        {
            Debug.Log($"constructor call, got {anotherService}");
        }
    }
}
