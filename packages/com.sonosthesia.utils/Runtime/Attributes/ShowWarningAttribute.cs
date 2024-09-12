using UnityEngine;

namespace Sonosthesia.Utils
{
    public class ShowWarningAttribute : PropertyAttribute
    {
        public string Message { get; private set; }

        public ShowWarningAttribute(string message)
        {
            Message = message;
        }
    }
}