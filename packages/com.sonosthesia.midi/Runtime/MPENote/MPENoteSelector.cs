using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public class MPENoteSelector : Selector<MPENote>
    {
        private enum Selection
        {
            None,
            Unit,
            Note,
            Velocity,
            Pressure,
            Slide,
            Bend
        }

        [SerializeField] private Selection _selection;
        
        [SerializeField] private bool _normalize;

        protected override float InternalSelect(MPENote value)
        {
            return _selection switch
            {
                Selection.Unit => 1f,
                Selection.Note => _normalize ? value.Note / 127f : value.Note,
                Selection.Velocity => _normalize ? value.Velocity  / 127f : value.Velocity,
                Selection.Pressure => _normalize ? value.Pressure  / 127f : value.Pressure,
                Selection.Slide => _normalize ? value.Slide  / 127f : value.Slide,
                Selection.Bend => _normalize ? value.Bend  / 127f : value.Bend,
                _ => 0
            };
        }
    }
}