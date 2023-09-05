using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.VisualFlow
{
    public class ScriptGraphTarget<T, B> : BlendTarget<T, B> where T : struct where B : struct, IBlender<T>
    {
        [SerializeField] private ScriptGraphDomain _domain;

        [SerializeField] private string _variableName;

        protected override T Reference => this.GetVariableDeclarations(_domain).Get<T>(_variableName);
        protected override void ApplyBlended(T value) => this.GetVariableDeclarations(_domain).Set(_variableName, value);
    }
}
