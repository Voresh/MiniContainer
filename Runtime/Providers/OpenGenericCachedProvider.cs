using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EasyUnity.Providers {
    public class OpenGenericCachedProvider : IOpenGenericProvider {
        private class TypesArrayEqualityComparer : IEqualityComparer<Type[]> {
            public bool Equals(Type[] x, Type[] y) {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;
                if (x.Length != y.Length)
                    return false;
                for (var i = 0; i < x.Length; i++)
                    if (x[i] != y[i])
                        return false;
                return true;
            }

            public int GetHashCode(Type[] types) {
                var hashcode = new HashCode();
                foreach (var type in types)
                    hashcode.Add(type);
                return hashcode.ToHashCode();
            }
        }
        private readonly Container _Container;
        private readonly Type _Type;
        private readonly Dictionary<Type[], object> _GenericInstances 
            = new Dictionary<Type[], object>(new TypesArrayEqualityComparer());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance(Type[] genericArguments) {
            if (_GenericInstances.TryGetValue(genericArguments, out var instance))
                return instance;
            instance = _Container.CreateInstance(_Type.MakeGenericType(genericArguments));
            _GenericInstances.Add(genericArguments, instance);
            return instance;
        }

        public OpenGenericCachedProvider(Container container, Type type) {
            _Container = container;
            _Type = type;
        }
    }
}
