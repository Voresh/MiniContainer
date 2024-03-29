﻿using MiniContainer.Samples.Factory.Services;
using UnityEngine;

namespace MiniContainer.Samples.Factory.Production
{
    public class FactoryProduction
    {
        private readonly int _Param;
        private readonly AnotherService _AnotherService;

        public FactoryProduction(AnotherService anotherService, int param)
        {
            _AnotherService = anotherService;
            _Param = param;
        }

        public void DebugFields()
        {
            Debug.Log($"{nameof(_Param)}:{_Param} {nameof(_AnotherService)}:{_AnotherService}");
        }
    }
}
