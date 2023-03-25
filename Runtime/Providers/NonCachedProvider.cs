using System;
using System.Runtime.CompilerServices;

namespace EasyUnity.Providers {
    internal class NonCachedProvider : IProvider {
        private readonly Container _Container;
        private readonly Type _Type;

        public bool Tracked => false;
        
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
