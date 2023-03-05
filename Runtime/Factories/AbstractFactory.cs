using System;

#if UNITY_2020_1_OR_NEWER && !DISABLE_EASY_UNITY_IL2CPP_OPTIONS
using Unity.IL2CPP.CompilerServices;
#endif

namespace EasyUnity.Factories {
#if UNITY_2020_1_OR_NEWER && !DISABLE_EASY_UNITY_IL2CPP_OPTIONS
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
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
