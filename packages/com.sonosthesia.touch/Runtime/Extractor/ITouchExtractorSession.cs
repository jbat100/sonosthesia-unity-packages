namespace Sonosthesia.Touch
{
    public interface ITouchExtractorSession<T> where T : struct
    {
        public bool Setup(TouchEvent touchEvent, out T value);
        public bool Update(TouchEvent touchEvent, out T value);
    }
    
}