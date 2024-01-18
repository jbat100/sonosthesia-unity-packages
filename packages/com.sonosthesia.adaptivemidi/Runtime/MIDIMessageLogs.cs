using UniRx;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public class MIDIMessageLogs : MonoBehaviour
    {
        private MIDIMessageNode _broadcaster;
        private CompositeDisposable _subscriptions = new ();
        
        protected void Awake()
        {
            _broadcaster = GetComponent<MIDIMessageNode>();
        }

        protected void OnEnable()
        {
            _subscriptions = new CompositeDisposable
            {
                _broadcaster.ClockObservable.Subscribe(m => Debug.Log(m)),
                _broadcaster.NoteOnObservable.Subscribe(m => Debug.Log(m)),
                _broadcaster.NoteOffObservable.Subscribe(m => Debug.Log(m)),
                _broadcaster.ControlObservable.Subscribe(m => Debug.Log(m)),
                _broadcaster.PitchBendObservable.Subscribe(m => Debug.Log(m)),
                _broadcaster.ChannelAftertouchObservable.Subscribe(m => Debug.Log(m)),
                _broadcaster.PolyphonicAftertouchObservable.Subscribe(m => Debug.Log(m)),
                _broadcaster.SongPositionPointerObservable.Subscribe(m => Debug.Log(m)),
                _broadcaster.SyncObservable.Subscribe(m => Debug.Log(m)),
            };
        }

        protected void OnDisable()
        {
            _subscriptions?.Dispose();
        }
    }
}