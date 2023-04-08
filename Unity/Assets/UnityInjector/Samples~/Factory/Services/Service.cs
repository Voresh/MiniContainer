using System;
using UnityInjector.Samples.Factory.Production;

namespace UnityInjector.Samples.Factory.Services {
    public class Service {
        public Service(Func<int, FactoryProduction> factoryMethod) {
            var production = factoryMethod(1);
            production.DebugFields();
            production = factoryMethod(2);
            production.DebugFields();
            production = factoryMethod(3);
            production.DebugFields();
        }
    }
}
