using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.VisualFlow
{
    public class ScriptGraphSignal<T> : Signal<T> where T : struct
    {
        [SerializeField] private ScriptGraphDomain _domain;

        [SerializeField] private string _variableName;

        protected void Update()
        {
            T current = this.GetVariableDeclarations(_domain).Get<T>(_variableName);
            Broadcast(current);
        }
    }
}