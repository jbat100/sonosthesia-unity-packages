using System;
using System.Collections.Generic;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Instrument
{
    public class MIDIPointerControlChange : MonoBehaviour, 
        IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler
    {
        [Serializable]
        private class AxisSettings
        {
            [SerializeField] private bool _active;
            public bool Active => _active;

            [SerializeField] private int _channel;
            public int Channel => _channel;
            
            [SerializeField] private int _controller;
            public int Controller => _controller;

            [SerializeField] private float _sensitivity;
            public float Sensitivity => _sensitivity;

            [SerializeField] private float _initialValue;
            public float InitialValue => _initialValue;
        }
        
        [SerializeField] private MIDIOutput _output;

        [SerializeField] private AxisSettings _xSettings;
        
        [SerializeField] private AxisSettings _ySettings;

        [SerializeField] private AxisSettings _zSettings;
        
        private class DragSession
        {
            public Vector3 PreviousTarget { get; private set; }
            public float PreviousTime { get; private set; }
            public GameObject DragObject { get; private set; }

            public DragSession(Vector3 target, float time, GameObject dragObject)
            {
                PreviousTarget = target;
                PreviousTime = time;
                DragObject = dragObject;
            }

            public Vector3 Push(Vector3 target, float time)
            {
                Vector3 result = target - PreviousTarget;
                PreviousTarget = target;
                PreviousTime = time;
                return result;
            }
        }

        private Dictionary<int, DragSession> _dragSessions = new();

        private class AxisState
        {
            private float _currentValue;
            private int _lastSent = -1;

            private int MIDIValue => Mathf.Clamp(Mathf.RoundToInt(_currentValue), 0, 127);

            private void SendMessage(AxisSettings settings, MIDIOutput output, bool force)
            {
                int midiValue = MIDIValue;
                if (force || midiValue != _lastSent)
                {
                    _lastSent = midiValue;
                    output.BroadcastControl(new MIDIControl(settings.Channel, settings.Controller, midiValue));
                }
            }

            public AxisState(AxisSettings setting)
            {
                _currentValue = MIDIUtils.ClampTo7Bit(setting.InitialValue);
                _lastSent = MIDIValue;
            }

            public void ProcessDrag(float drag, AxisSettings settings, MIDIOutput output)
            {
                if (settings.Active)
                {
                    _currentValue = MIDIUtils.ClampTo7Bit(_currentValue + drag * settings.Sensitivity);
                    SendMessage(settings, output, false);    
                }
            }

            public void Flush(AxisSettings settings, MIDIOutput output)
            {
                SendMessage(settings, output, true);
            }
        }

        private AxisState _xState;
        private AxisState _yState;
        private AxisState _zState;

        protected virtual void OnEnable()
        {
            _dragSessions.Clear();
            _xState = new AxisState(_xSettings);
            _yState = new AxisState(_ySettings);
            _zState = new AxisState(_zSettings);
        }
        
        protected virtual void OnDisable()
        {
            _dragSessions.Clear();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector3 target = transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
            _dragSessions[eventData.pointerId] = new DragSession(target, Time.time, eventData.pointerCurrentRaycast.gameObject);
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _dragSessions.Remove(eventData.pointerId);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (_dragSessions.TryGetValue(eventData.pointerId, out DragSession session))
            {
                // this leads to silly jumps
                if (eventData.pointerCurrentRaycast.gameObject != session.DragObject)
                {
                    return;
                }
                Vector3 target = transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
                Vector3 drag = session.Push(target, Time.time);
                _xState.ProcessDrag(drag.x, _xSettings, _output);
                _yState.ProcessDrag(drag.y, _ySettings, _output);
                _zState.ProcessDrag(drag.z, _zSettings, _output);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _dragSessions.Remove(eventData.pointerId);
        }
    }
}