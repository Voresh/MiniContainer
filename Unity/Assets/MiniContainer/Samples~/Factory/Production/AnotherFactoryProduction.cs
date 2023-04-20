using MiniContainer.Samples.Factory.Services;
using UnityEngine;

namespace MiniContainer.Samples.Factory.Production
{
    public class AnotherFactoryProduction
    {
        private readonly AnotherService _AnotherService;

        public AnotherFactoryProduction(AnotherService anotherService)
        {
            _AnotherService = anotherService;
        }

        public void DebugFields()
        {
            Debug.Log($"{nameof(_AnotherService)}:{_AnotherService}");
        }
    }
}
