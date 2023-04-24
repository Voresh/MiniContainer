using System;
using System.Runtime.CompilerServices;
using MiniContainer.Registration;

namespace MiniContainer
{
    public static class ContainerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RegistrationContext RegisterInstance<TType>(this Container container, TType instance) 
            => container.RegisterInstance(instance, typeof(TType));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RegistrationContext RegisterInstance<TType, TInterface>(this Container container, TType instance) where TType : TInterface
            => container.RegisterInstance(instance, typeof(TInterface));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RegistrationContext Register<TType, TInterface>(this Container container, bool cached = true) where TType : TInterface
            => container.Register(typeof(TType), typeof(TInterface), cached);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RegistrationContext Register<TType>(this Container container, bool cached = true)
            => container.Register(typeof(TType), cached);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RegistrationContext Register(this Container container, Type type, bool cached = true)
            => container.Register(type, type, cached);
    }
}
