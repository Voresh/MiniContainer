using System;
using System.Runtime.CompilerServices;

namespace MiniContainer.InstanceConstructors
{
    public class ReflectionInstanceConstructor : InstanceConstructor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryGetInstance(Type type, Container container, out object instance)
        {
            var constructors = type.GetConstructors();
            var parameters = constructors[0].GetParameters();
            for (var index = 1; index < constructors.Length; index++)
            {
                var nextParameters = constructors[index].GetParameters();
                if (parameters.Length < nextParameters.Length)
                    parameters = nextParameters;
            }
            if (parameters.Length > 0)
            {
                var resolvedParameters = new object[parameters.Length];
                for (var index = 0; index < parameters.Length; index++)
                    resolvedParameters[index] = container.Resolve(parameters[index].ParameterType);
                instance = Activator.CreateInstance(type, resolvedParameters);
            }
            else
            {
                instance = Activator.CreateInstance(type);
            }
            return true;
        }
    }
}
