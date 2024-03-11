using System.Collections.Generic;
using Sonosthesia.Utils;

namespace Sonosthesia.Scaffold
{
    public abstract class GroupTransformerConfiguration : DynamicScriptableObject
    {
        public abstract void Apply<T>(IEnumerable<T> targets) where T : IGroupTransformerElement;
    }
}