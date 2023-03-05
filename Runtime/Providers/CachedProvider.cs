using System;
using System.Runtime.CompilerServices;

#if UNITY_2020_1_OR_NEWER && !DISABLE_EASY_UNITY_IL2CPP_OPTIONS
using Unity.IL2CPP.CompilerServices;
#endif

namespace EasyUnity.Providers {
#if UNITY_2020_1_OR_NEWER && !DISABLE_EASY_UNITY_IL2CPP_OPTIONS
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class CachedProvider : IProvider {
        private readonly Container _Container;
        private readonly Type _Type;
        private bool _Created;
        private object _Instance;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance() {
            if (_Created)
                return _Instance;
            _Instance = _Container.CreateInstance(_Type);
            _Created = true;
            return _Instance;
        }

        public CachedProvider(Container container, Type type) {
            _Container = container;
            _Type = type;
        }
    }
}
