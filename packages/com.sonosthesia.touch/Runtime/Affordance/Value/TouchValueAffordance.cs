using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchValueAffordance<TValue, TContainer> : ValueAffordance<TValue, TouchEvent, TContainer>
        where TValue : struct
        where TContainer : MonoBehaviour, IStreamContainer<ValueEvent<TValue, TouchEvent>>
    {
        
    }
}