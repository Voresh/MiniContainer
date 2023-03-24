using System;
using System.Runtime.CompilerServices;

namespace EasyUnity.Providers {
    public interface IOpenGenericProvider {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object GetInstance(Type[] genericArguments);
    }
}
