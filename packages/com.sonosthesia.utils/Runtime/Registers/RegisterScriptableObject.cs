using System.Collections.Generic;

namespace Sonosthesia.Utils
{
    public class RegisterScriptableObject<T> : ObservableScriptableObject
    {
        private readonly List<T> _items = new();
        public IReadOnlyList<T> Items => _items.AsReadOnly();

        public void Register(T item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
                BroadcastChange();
            }
        }
        
        public void Unregister(T multiSwitch)
        {
            if (_items.Contains(multiSwitch))
            {
                _items.Remove(multiSwitch);
                BroadcastChange();
            }
        }
    }
}