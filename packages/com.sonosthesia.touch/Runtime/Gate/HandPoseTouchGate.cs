namespace Sonosthesia.Touch
{
    public class HandPoseTouchGate : TouchGate
    {
        public override bool Check(TouchSource source, TouchActor actor)
        {
            return true;
        }
    }
}