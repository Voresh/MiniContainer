using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MiniContainer.InstanceConstructors
{
    public class DictionaryInstanceConstructor : InstanceConstructor
    {
        public Dictionary<Type, Func<Container, object>> Constructors { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryGetInstance(Type type, Container container, out object instance)
        {
            if (Constructors.TryGetValue(type, out var constructor))
            {
                instance = constructor.Invoke(container);
                return true;
            }
            instance = null;
            return false;
        }

        public DictionaryInstanceConstructor(params Dictionary<Type, Func<Container, object>>[] constructorDictionaries)
        {
            var capacity = constructorDictionaries?.Sum(_ => _.Count) ?? 0;
            Constructors = new Dictionary<Type, Func<Container, object>>(capacity);
            if (constructorDictionaries != null)
                foreach (var dictionary in constructorDictionaries)
                    foreach (var kvp in dictionary)
                        Constructors.Add(kvp.Key, kvp.Value);
        }
    }
}
