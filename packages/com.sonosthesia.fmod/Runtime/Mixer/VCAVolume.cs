using FMOD.Studio;

namespace Sonosthesia.FMOD
{
    public class VCAVolume : Volume
    {
        private VCA _vca;
        
        protected override void SetupPath(string path) => _vca = FMODUnity.RuntimeManager.GetVCA(path);

        protected override void ApplyLinearVolume(float volume) => _vca.setVolume(volume);

    }    
}


