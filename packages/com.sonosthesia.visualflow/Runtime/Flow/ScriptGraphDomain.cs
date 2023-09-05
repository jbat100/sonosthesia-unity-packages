using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Sonosthesia.VisualFlow
{
    public enum ScriptGraphDomain
    {
        Object,
        Scene,
        Application,
        Saved
    }

    public static class ScriptGraphDomainExtensions
    {
        public static VariableDeclarations GetVariableDeclarations(this MonoBehaviour monoBehaviour, ScriptGraphDomain domain)
        {
            return domain switch
            {
                ScriptGraphDomain.Object => Variables.Object(monoBehaviour),
                ScriptGraphDomain.Scene => Variables.Scene(monoBehaviour),
                ScriptGraphDomain.Application => Variables.Application,
                ScriptGraphDomain.Saved => Variables.Saved,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}