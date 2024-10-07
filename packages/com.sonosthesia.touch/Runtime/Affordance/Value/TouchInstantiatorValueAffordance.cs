using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchInstantiatorValueAffordance<TValue, TContainer> : 
        InstantiatorValueAffordance<TValue, TouchEvent, TContainer>
        where TValue : struct
        where TContainer : MonoBehaviour, IStreamContainer<ValueEvent<TValue, TouchEvent>>
    {
        
    }
}