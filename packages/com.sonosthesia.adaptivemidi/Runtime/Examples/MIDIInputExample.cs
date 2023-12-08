using UniRx;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI.Examples
{
    public class MIDIInputExample : MonoBehaviour
    {
        [SerializeField] private MIDIInput _input;
        
        private CompositeDisposable _subscriptions = new ();

        protected void OnEnable()
        {
            _subscriptions = new CompositeDisposable
            {
                _input.ClockObservable.Subscribe(m => Debug.Log(m)),
                _input.NoteOnObservable.Subscribe(m => Debug.Log(m)),
                _input.NoteOffObservable.Subscribe(m => Debug.Log(m)),
                _input.ControlObservable.Subscribe(m => Debug.Log(m)),
                _input.ChannelAftertouchObservable.Subscribe(m => Debug.Log(m)),
                _input.PolyphonicAftertouchObservable.Subscribe(m => Debug.Log(m)),
                _input.SongPositionPointerObservable.Subscribe(m => Debug.Log(m)),
                _input.SyncObservable.Subscribe(m => Debug.Log(m)),
            };
        }

        protected void OnDisable()
        {
            _subscriptions?.Dispose();
        }
    }
}