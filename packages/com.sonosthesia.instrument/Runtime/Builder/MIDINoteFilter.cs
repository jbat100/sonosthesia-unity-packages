using Sonosthesia.Utils;

namespace Sonosthesia.Instrument
{
    public abstract class MIDINoteFilter : ObservableBehaviour
    {
        public abstract bool Allow(int note);
    }
}