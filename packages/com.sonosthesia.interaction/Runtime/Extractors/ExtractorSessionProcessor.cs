namespace Sonosthesia.Interaction
{
    public abstract class ExtractorSessionProcessor<TEvent, TValue> : IExtractorSession<TEvent, TValue> 
        where TEvent : IInteractionEvent where TValue : struct
    {
        private readonly IExtractorSession<TEvent, TValue> _session;
        
        public ExtractorSessionProcessor(IExtractorSession<TEvent, TValue> session)
        {
            _session = session;
        }

        protected abstract TValue Process(TEvent touchEvent, TValue value);

        public bool Setup(TEvent touchEvent, out TValue value)
        {
            if (_session.Setup(touchEvent, out value))
            {
                value = Process(touchEvent, value);
                return true;
            }
            return false;
        }
        
        public bool Update(TEvent touchEvent, out TValue value)
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