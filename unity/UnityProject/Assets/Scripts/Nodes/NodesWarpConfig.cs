using System;
using Unity.VisualScripting;

namespace Sonosthesia.Nodes
{
    [Serializable, Inspectable]
    public class NodesWarpConfig
    {
        [Inspectable] public float Scale;
        [Inspectable] public float Offset;
    }
}
