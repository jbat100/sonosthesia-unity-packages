namespace Sonosthesia.Touch
{
    public abstract class TouchExtractorSessionProcessor<T> : ITouchExtractorSession<T> where T : struct
    {
        private readonly ITouchExtractorSession<T> _session;
        
        public TouchExtractorSessionProcessor(ITouchExtractorSession<T> session)
        {
            _session = session;
        }

        protected abstract T Process(TouchEvent touchEvent, T value);

        public bool Setup(TouchEvent touchEvent, out T value)
        {
            if (_session.Setup(touchEvent, out value))
            {
                value = Process(touchEvent, value);
                return true;
            }
            return false;
        }
        
        public bool Update(TouchEvent touchEvent, out T value)
        {
            if (_session.Update(touchEvent, out value))
            {
                value = Process(touchEvent, value);
                return true;
            }
            return false;
        }
    }
}