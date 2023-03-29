using System.Runtime.CompilerServices;

namespace UnityInjector.Providers {
    public interface IProvider {
        bool Tracked { get; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object GetInstance();
    }
}
