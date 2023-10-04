using System;

namespace Sonosthesia.Utils
{
    
    // Suggested by Chat GPT as a way to provide a Nullable concept which can handle both reference types
    // and value types 
    
    public readonly struct Option<T>
    {
        private readonly T _value;
        public bool HasValue { get; }
    
        private Option(T value, bool hasValue)
        {
            _value = value;
            HasValue = hasValue;
        }

        public static Option<T> Some(T value) => new Option<T>(value, true);

        public static Option<T> None => new Option<T>(default, false);

        public T Value
        {
            get
            {
                if (!HasValue) throw new InvalidOperationException("Option does not have a value");
                return _value;
            }
        }
    }
}