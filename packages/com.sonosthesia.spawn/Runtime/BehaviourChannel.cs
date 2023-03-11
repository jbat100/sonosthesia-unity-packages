using Sonosthesia.Flow;

namespace Sonosthesia.Spawn
{
    public readonly struct BehaviourPayload
    {
        public readonly float Excitation;
        public readonly float Size;
        public readonly float Hue;
        public readonly float Opacity;

        public BehaviourPayload(float excitation, float size, float hue, float opacity)
        {
            Excitation = excitation;
            Size = size;
            Hue = hue;
            Opacity = opacity;
        }
    }
    
    public class BehaviourChannel : Channel<BehaviourPayload>
    {
        
    }
}