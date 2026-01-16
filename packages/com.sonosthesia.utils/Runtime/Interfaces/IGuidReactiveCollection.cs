using System;
using UniRx;

namespace Sonosthesia.Utils
{
    public interface IGuidReactiveCollection
    {
        IReadOnlyReactiveCollection<Guid> Ids { get; }
    }
}