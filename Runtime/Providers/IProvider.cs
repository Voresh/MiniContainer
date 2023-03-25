using System.Runtime.CompilerServices;

namespace EasyUnity.Providers {
    public interface IProvider {
        bool Tracked { get; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object GetInstance();
    }
}
