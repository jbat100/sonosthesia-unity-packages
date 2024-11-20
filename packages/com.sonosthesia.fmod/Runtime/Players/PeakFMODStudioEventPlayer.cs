using Sonosthesia.Utils;

namespace Sonosthesia.FMOD
{
    public class PeakFMODStudioEventPlayer : FMODStudioEventPlayer<Peak>
    {
        protected override bool SkipFirst => true;
    }
}