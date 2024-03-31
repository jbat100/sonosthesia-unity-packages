using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Utils;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Scaffold.Editor
{
    /// <summary>
    /// Splines.Changed does not fire in Editor, using this as a workaround
    /// </summary>
    
    [ExecuteInEditMode]
    public class EditorSplineChangeBroadcaster : MonoBehaviour
    {
        [SerializeField] private List<SplineContainer> _splines;
        
        [SerializeField] private List<ObservableBehaviour> _targets;

        protected virtual void OnEnable()
        {
            EditorSplineUtility.AfterSplineWasModified += SplineOnChanged;
        }

        protected virtual void OnDisable()
        {
            EditorSplineUtility.AfterSplineWasModified -= SplineOnChanged;
        }

        protected virtual void SplineOnChanged(Spline spline)
        {
            // Debug.Log($"{this} {nameof(SplineOnChanged)} {spline}");
            
            if (!_splines.SelectMany(s => s.Splines).Contains(spline))
            {
                return;
            }
            
            foreach (ObservableBehaviour target in _targets)
            {
                target.BroadcastChange();
            }
        }
    }    
}


