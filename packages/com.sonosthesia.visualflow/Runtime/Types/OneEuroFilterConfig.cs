using System;
using Unity.VisualScripting;

namespace Sonosthesia.VisualFlow
{
    [Serializable, Inspectable]
    public class OneEuroFilterConfig
    {
        [Inspectable] public float Beta;
        [Inspectable] public float MinCutoff;
    }
}