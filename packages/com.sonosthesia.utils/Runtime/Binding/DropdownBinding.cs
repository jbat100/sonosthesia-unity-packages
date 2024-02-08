using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine.UI;

namespace Sonosthesia.Utils
{
    public class DropdownBinding<T> : IDisposable
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;
        private readonly Dropdown _dropdown;
        private readonly BijectionDictionary<T, string> _texts;
        private readonly IDisposable _changeSubscription;
        
        public DropdownBinding(Dropdown dropdown, BijectionDictionary<T, string> texts, IObservable<Unit> changeObservable, Func<T> getter, Action<T> setter)
        {
            _dropdown = dropdown;
            _texts = texts;
            _getter = getter;
            _setter = setter;
            _dropdown.options = texts.Values.Select(note => new Dropdown.OptionData(note)).ToList();
            _changeSubscription = changeObservable.StartWith(Unit.Default).Subscribe(_ =>
            {
                T current = _getter();
                T selected = _texts.Reverse[_dropdown.options[_dropdown.value].text];
                if (!EqualityComparer<T>.Default.Equals(selected, current))
                {
                    _dropdown.value = _dropdown.options.FindIndex(option => option.text == _texts[current]);
                }
            });
            _dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(int i)
        {
            _setter(_texts.Reverse[_dropdown.options[i].text]);
        }

        public void Dispose()
        {
            _changeSubscription?.Dispose();
            _dropdown.onValueChanged.RemoveListener(OnValueChanged);
        }
    }
}