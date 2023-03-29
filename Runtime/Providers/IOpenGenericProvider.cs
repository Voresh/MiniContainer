using System;
using System.Runtime.CompilerServices;

namespace UnityInjector.Providers {
    public interface IOpenGenericProvider {
        bool Tracked { get; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object GetInstance(Type[] genericArguments);
    }
}
