using Sonosthesia.Utils;

namespace Sonosthesia.Trigger
{
    public class PeakSignalTrigger : SignalTrigger<Peak>
    {
        protected override bool SkipFirst => true;
    }
}