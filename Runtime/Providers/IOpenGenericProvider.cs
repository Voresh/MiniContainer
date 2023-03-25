using System;
using System.Runtime.CompilerServices;

namespace EasyUnity.Providers {
    public interface IOpenGenericProvider {
        bool Tracked { get; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object GetInstance(Type[] genericArguments);
    }
}
