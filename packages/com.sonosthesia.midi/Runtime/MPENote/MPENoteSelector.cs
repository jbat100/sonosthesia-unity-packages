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

        protected override float InternalSelect(MPENote value)
        {
            return _selection switch
            {
                Selection.Unit => 1f,
                Selection.Note => value.Note / 127f,
                Selection.Velocity => value.Velocity  / 127f,
                Selection.Pressure => value.Pressure  / 127f,
                Selection.Slide => value.Slide  / 127f,
                Selection.Bend => value.Bend  / 127f,
                _ => 0
            };
        }
    }
}