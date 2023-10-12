using UniRx;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Sonosthesia.AdaptiveMIDI
{
    [RequireComponent(typeof(MIDIInput))]
    public class MIDIInputDebug : MonoBehaviour
    {
        private MIDIInput _input;
        private CompositeDisposable _subscriptions = new ();
        
        protected void Awake()
        {
            _input = GetComponent<MIDIInput>();
        }

        protected void OnEnable()
        {
            _subscriptions = new CompositeDisposable()
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