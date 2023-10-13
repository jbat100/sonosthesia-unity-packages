using System;

namespace Sonosthesia.Utils
{
    [Flags]
    public enum Axes
    {
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2
    }
}