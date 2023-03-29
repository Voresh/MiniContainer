using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace UnityInjector.InstanceConstructors {
    public class ReflectionInstanceConstructor : InstanceConstructor {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryGetInstance(Type type, Container container, out object instance) {
            var constructors = type.GetConstructors();
            var parameters = constructors[0].GetParameters();
            for (var index = 1; index < constructors.Length; index++) {
                var nextParameters = constructors[index].GetParameters();
                if (parameters.Length < nextParameters.Length)
                    parameters = nextParameters;
            }
            var sharedPool = ArrayPool<object>.Shared;
            var resolvedParameters = sharedPool.Rent(parameters.Length);
            for (var index = 0; index < parameters.Length; index++)
                resolvedParameters[index] = container.Resolve(parameters[index].ParameterType);
            instance = Activator.CreateInstance(type, resolvedParameters);
            sharedPool.Return(resolvedParameters);
            return true;
        }
    }
}
