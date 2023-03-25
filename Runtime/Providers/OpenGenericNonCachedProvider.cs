using System;
using System.Runtime.CompilerServices;

namespace EasyUnity.Providers {
    public class OpenGenericNonCachedProvider : IOpenGenericProvider {
        private readonly Container _Container;
        private readonly Type _Type;

        public bool Tracked => false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance(Type[] genericArguments) {
            var genericType = _Type.MakeGenericType(genericArguments);
            return _Container.CreateInstance(genericType);
        }

        public OpenGenericNonCachedProvider(Container container, Type type) {
            _Container = container;
            _Type = type;
        }
    }
}
