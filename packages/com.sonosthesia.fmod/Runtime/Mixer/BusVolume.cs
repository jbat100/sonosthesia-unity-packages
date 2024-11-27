using FMOD.Studio;

namespace Sonosthesia.FMOD
{
    public class BusVolume : Volume
    {
        private Bus _bus;

        protected override void SetupPath(string path) => _bus = FMODUnity.RuntimeManager.GetBus(path);

        protected override void ApplyLinearVolume(float volume) => _bus.setVolume(volume);
    }    
}


