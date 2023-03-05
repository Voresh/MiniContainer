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
    internal class NonCachedProvider : IProvider {
        private readonly Container _Container;
        private readonly Type _Type;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance() {
            return _Container.CreateInstance(_Type);
        }

        public NonCachedProvider(Container container, Type type) {
            _Container = container;
            _Type = type;
        }
    }
}