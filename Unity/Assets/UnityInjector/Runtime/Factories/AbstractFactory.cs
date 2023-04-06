using System;
using System.Runtime.CompilerServices;

namespace UnityInjector.Factories {
    public class AbstractFactory {
        private readonly Container _Container;

        public AbstractFactory(Container container) {
            _Container = container;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ConstructType(Type type) {
            return _Container.CreateInstance(type);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ConstructType<T>() {
            return (T) _Container.CreateInstance(typeof(T));
        }
    }
}
