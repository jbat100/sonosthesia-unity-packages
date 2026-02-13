using System.Collections.Generic;
using Sonosthesia.Utils;

namespace Sonosthesia.Scaffold
{
    public abstract class GroupTransformerConfiguration : ObservableScriptableObject
    {
        public abstract void Apply<T>(IReadOnlyList<T> targets) where T : IGroupTransformerElement;
    }
}