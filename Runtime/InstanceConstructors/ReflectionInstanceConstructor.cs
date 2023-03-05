using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasyUnity.InstanceConstructors {
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
