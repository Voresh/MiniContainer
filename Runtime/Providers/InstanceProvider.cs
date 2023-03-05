using System.Runtime.CompilerServices;

namespace EasyUnity.Providers {
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
