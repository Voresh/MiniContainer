using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EasyUnity.Registration {
    internal interface IRegistrationContextProviders {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Add(Type type, object provider);
    }

    internal class RegistrationContextProviders<TProvider> : IRegistrationContextProviders {
        private readonly Dictionary<Type, TProvider> _Dictionary;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Type type, object provider) => _Dictionary.Add(type, (TProvider) provider);

        public RegistrationContextProviders(Dictionary<Type, TProvider> dictionary) {
            _Dictionary = dictionary;
        }
    }
}
