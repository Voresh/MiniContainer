﻿using UnityEngine;
using MiniContainer.Samples.Scoping.Services;

namespace DefaultNamespace
{
    public class ScopedService
    {
        public ScopedService(Service service)
        {
            Debug.Log($"Scoped service constructor call, got {service}");
        }
    }
}
