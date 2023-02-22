using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MikroFramework.Architecture
{
    public interface ICanGetSystem : IBelongToArchitecture
    {
    }

    public static class CanGetSystemExtension
    {
        public static T GetSystem<T>(this ICanGetSystem self, Action<T> delayedResult = null) where T : class, ISystem
        {
            return self.GetArchitecture().GetSystem<T>(delayedResult);
        }
    }
    
}
