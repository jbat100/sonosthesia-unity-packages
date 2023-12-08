using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI.Timeline
{
    // TODO : use metronome
    
    public class MIDITimelineSync : MonoBehaviour
    {
        [SerializeField] private float _tempo = 120;
        
        [SerializeField] private MIDIInput _input;

        [SerializeField] private PlayableDirector _director;

        private float _beatPeriod;
        private MIDISongPositionPointer? _pointer;
        private readonly CompositeDisposable _subscriptions = new();
        
        protected void OnEnable()
        {
            // beat is 16th note 15 = 60/4
            _beatPeriod = 15f / _tempo; 
            _subscriptions.Add(_input.SongPositionPointerObservable.Subscribe(pointer => _pointer = pointer));
            _subscriptions.Add(_input.SyncObservable.Subscribe(sync =>
            {
                switch (sync.Type)
                {
                    case MIDISyncType.Start:
                    case MIDISyncType.Continue:
                    {
                        if (_pointer.HasValue)
                        {
                            float time = _beatPeriod * _pointer.Value.Position;
                            _director.time = time;
                            _director.Play();
                            Debug.Log($"{this} started play at {time}");
                        }
                        else
                        {
                            Debug.LogError($"{this} expected pointer");
                        }
                        break;
                    }
                    case MIDISyncType.Stop:
                    {
                        Debug.Log($"{this} stop");
                        _director.Stop();
                        break;
                    }
                }
            }));
        }

        protected void OnDisable()
        {
            _pointer = null;
            _subscriptions.Clear();
        }
    }
}