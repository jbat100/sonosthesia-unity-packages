using System;
using Unity.VisualScripting;

namespace Sonosthesia.Root
{
    [Serializable, Inspectable]
    public class RootWarpConfig
    {
        [Inspectable] public float Scale;
        [Inspectable] public float Offset;
    }
}
