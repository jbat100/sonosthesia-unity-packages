namespace Sonosthesia.Mapping
{
    public class ConfigurableFader<T> : Fader<T> where T : struct
    {
        
        
        public override T Fade(float fade)
        {
            throw new System.NotImplementedException();
        }
    }
}