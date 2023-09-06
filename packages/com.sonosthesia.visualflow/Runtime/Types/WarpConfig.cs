using System;
using Unity.VisualScripting;

namespace Sonosthesia.VisualFlow
{
    [Serializable, Inspectable]
    public class WarpConfig
    {
        [Inspectable] public float Scale;
        [Inspectable] public float Offset;
    }
}