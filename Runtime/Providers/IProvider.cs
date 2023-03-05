using System.Runtime.CompilerServices;

namespace EasyUnity.Providers
{
	public interface IProvider
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		object GetInstance();
	}
}