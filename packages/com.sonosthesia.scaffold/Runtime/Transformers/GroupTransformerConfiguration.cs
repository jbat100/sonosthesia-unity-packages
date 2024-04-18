using System.Collections.Generic;
using Sonosthesia.Utils;

namespace Sonosthesia.Scaffold
{
    public abstract class GroupTransformerConfiguration : ObservableScriptableObject
    {
        public abstract void Apply<T>(IEnumerable<T> targets) where T : IGroupTransformerElement;
    }
}