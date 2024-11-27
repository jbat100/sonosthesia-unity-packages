using UniRx;

namespace Sonosthesia.Touch
{
    // Useful when we only need affordances, without driving a channel
    // Its becoming clearer that a given trigger source should be able to drive multiple different processes
    // and templating on a single type is limiting. Prefer using agnostic affordances as they can be combined
    // to drive heterogeneous processes 
    
    public class UnitTouchSource : ValueTouchSource<Unit>
    {
        protected override bool Extract(bool initial, ITouchData touchData, out Unit value)
        {
            value = Unit.Default;
            return true;
        }
    }
}