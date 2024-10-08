namespace Sonosthesia.Touch
{
    public abstract class TouchExtractorSessionProcessor<T, TProcessor> : ITouchExtractorSession<T> 
        where T : struct 
        where TProcessor : class
    {
        private readonly ITouchExtractorSession<T> _session;
        private readonly TProcessor _processor;
        
        public TouchExtractorSessionProcessor(ITouchExtractorSession<T> session, TProcessor processor)
        {
            _session = session;
            _processor = processor;
        }

        protected abstract T Process(TProcessor processor, T value);

        public bool Setup(TouchEvent touchEvent, out T value)
        {
            if (_session.Setup(touchEvent, out value))
            {
                value = Process(_processor, value);
                return true;
            }
            return false;
        }
        
        public bool Update(TouchEvent touchEvent, out T value)
        {
            if (_session.Update(touchEvent, out value))
            {
                value = Process(_processor, value);
                return true;
            }
            return false;
        }
    }
}