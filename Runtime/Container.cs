using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EasyUnity.InstanceConstructors;
using EasyUnity.Providers;
using EasyUnity.Registration;

#if UNITY_2020_1_OR_NEWER && !DISABLE_EASY_UNITY_IL2CPP_OPTIONS
using Unity.IL2CPP.CompilerServices;
#endif

namespace EasyUnity {
#if UNITY_2020_1_OR_NEWER && !DISABLE_EASY_UNITY_IL2CPP_OPTIONS
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class Container {
        private readonly Container _Parent;
        private readonly Dictionary<Type, IProvider> _Container
            = new Dictionary<Type, IProvider>();
        private static readonly HashSet<InstanceConstructor> _InstanceConstructors
            = new HashSet<InstanceConstructor> { new ReflectionInstanceConstructor() };

        public Container(Container parent = null) {
            _Parent = parent;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInstanceConstructors(params InstanceConstructor[] instanceConstructors) {
            _InstanceConstructors.Clear();
            foreach (var instanceConstructor in instanceConstructors)
                _InstanceConstructors.Add(instanceConstructor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext<TType> RegisterInstance<TType>(TType instance) where TType : class {
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_Container.ContainsKey(typeof(TType)))
                throw new Exception($"{typeof(TType)} already registered in container");
#endif
            var provider = new InstanceProvider(instance);
            _Container.Add(typeof(TType), provider);
            return new RegistrationContext<TType>(_Container, provider);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext<TType> RegisterInstance<TType, TInterface>(TType instance) where TType : class, TInterface {
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_Container.ContainsKey(typeof(TType)))
                throw new Exception($"{typeof(TType)} already registered in container");
#endif
            var provider = new InstanceProvider(instance);
            _Container.Add(typeof(TInterface), provider);
            return new RegistrationContext<TType>(_Container, provider);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext RegisterInstance(object instance, Type interfaceType) {
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_Container.ContainsKey(type))
                throw new Exception($"{type} already registered in container");
#endif
            var provider = new InstanceProvider(instance);
            _Container.Add(interfaceType, provider);
            return new RegistrationContext(_Container, provider, instance.GetType());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext<TType> Register<TType, TInterface>(bool cached = true) where TInterface : class where TType : class, TInterface {
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_Container.ContainsKey(typeof(TType)))
                throw new Exception($"{typeof(TType)} already registered in container");
#endif
            IProvider provider = cached
                ? new CachedProvider(this, typeof(TType)) 
                : new NonCachedProvider(this, typeof(TType));
            _Container.Add(typeof(TInterface), provider);
            return new RegistrationContext<TType>(_Container, provider);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext<TType> Register<TType>(bool cached = true) where TType : class {
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_Container.ContainsKey(typeof(TType)))
                throw new Exception($"{typeof(TType)} already registered in container");
#endif
            IProvider provider = cached 
                ? new CachedProvider(this, typeof(TType))
                : new NonCachedProvider(this, typeof(TType));
            _Container.Add(typeof(TType), provider);
            return new RegistrationContext<TType>(_Container, provider);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RegistrationContext Register(Type type, bool cached = true) {
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_Container.ContainsKey(type))
                throw new Exception($"{type} already registered in container");
#endif
            IProvider provider = cached
                ? new CachedProvider(this, type) 
                : new NonCachedProvider(this, type);
            _Container.Add(type, provider);
            return new RegistrationContext(_Container, provider, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object CreateInstance(Type type) {
            foreach (var instanceConstructor in _InstanceConstructors)
                if (instanceConstructor.TryGetInstance(type, this, out var instance))
                    return instance;
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            throw new Exception($"no instance constructor found for {type}");
#endif
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Resolve<T>() {
            return (T) Resolve(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Resolve(Type type) {
            if (_Container.TryGetValue(type, out var provider))
                return provider.GetInstance();
#if ENABLE_EASY_UNITY_CONTAINER_EXCEPTIONS
            if (_Parent == null)
                throw new Exception($"{type} not registered in container");
#endif
            return _Parent.Resolve(type);
        }
    }
}