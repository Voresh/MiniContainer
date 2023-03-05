using System;
using System.Linq;
using System.Runtime.CompilerServices;

#if UNITY_2020_1_OR_NEWER && !DISABLE_EASY_UNITY_IL2CPP_OPTIONS
using Unity.IL2CPP.CompilerServices;
#endif

namespace EasyUnity.InstanceConstructors {
#if UNITY_2020_1_OR_NEWER && !DISABLE_EASY_UNITY_IL2CPP_OPTIONS
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class ReflectionInstanceConstructor : InstanceConstructor {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryGetInstance(Type type, Container container, out object instance) {
            var constructor = type
                .GetConstructors()
                .OrderByDescending(_ => _.GetParameters().Length)
                .First();
            var parameters = constructor
                .GetParameters()
                .Select(_ => container.Resolve(_.ParameterType))
                .ToArray(); //todo: remove allocation
            instance = Activator.CreateInstance(type, parameters);
            return true;
        }
    }
}
