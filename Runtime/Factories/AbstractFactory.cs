using System;

namespace UnityInjector.Factories {
    public class AbstractFactory {
        private readonly Container _Container;

        public AbstractFactory(Container container) {
            _Container = container;
        }

        public object ConstructType(Type type) {
            return _Container.CreateInstance(type);
        }
        
        public T ConstructType<T>() {
            return (T) _Container.CreateInstance(typeof(T));
        }
    }
}
