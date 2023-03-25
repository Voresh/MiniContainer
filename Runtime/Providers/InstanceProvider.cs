using System.Runtime.CompilerServices;

namespace EasyUnity.Providers {
    public class InstanceProvider : IProvider {
        private readonly object _Instance;

        public bool Tracked => true;
        
        public InstanceProvider(object instance) {
            _Instance = instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance() {
            return _Instance;
        }
    }
}
