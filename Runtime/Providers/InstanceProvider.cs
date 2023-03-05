using System.Runtime.CompilerServices;

#if UNITY_2020_1_OR_NEWER && !DISABLE_EASY_UNITY_IL2CPP_OPTIONS
using Unity.IL2CPP.CompilerServices;
#endif

namespace EasyUnity.Providers {
#if UNITY_2020_1_OR_NEWER && !DISABLE_EASY_UNITY_IL2CPP_OPTIONS
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class InstanceProvider : IProvider {
        private readonly object _Instance;

        public InstanceProvider(object instance) {
            _Instance = instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance() {
            return _Instance;
        }
    }
}
