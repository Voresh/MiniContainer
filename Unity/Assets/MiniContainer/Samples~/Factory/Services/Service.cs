using System;
using MiniContainer.Samples.Factory.Production;

namespace MiniContainer.Samples.Factory.Services {
    public class Service {
        public Service(Func<int, FactoryProduction> productionFactory, Func<Type, object> abstractFactory) {
            var production = productionFactory(1);
            production.DebugFields();
            production = productionFactory(2);
            production.DebugFields();
            production = productionFactory(3);
            production.DebugFields();
            var anotherProduction = (AnotherFactoryProduction) abstractFactory(typeof(AnotherFactoryProduction));
            anotherProduction.DebugFields();
        }
    }
}
