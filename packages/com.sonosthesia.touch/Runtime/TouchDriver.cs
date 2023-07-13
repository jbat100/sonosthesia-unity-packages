using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public class TouchDriver : MonoBehaviour
    {
        [SerializeField] private TouchChannel _channel;

        private BehaviorSubject<TouchPayload> _current;

        private readonly TouchPayloadBuilder _builder = new TouchPayloadBuilder();

        protected virtual TouchPayload MakePayload(PointerEventData data)
        {
            float3 position = data.pointerCurrentRaycast.worldPosition;
            _builder.Contact = position;
            _builder.Source = new RigidTransform(quaternion.identity, position);
            _builder.Target = new RigidTransform(quaternion.identity, position);
            return _builder.ToTouchPayload();
        }

        protected virtual bool ShouldMove(PointerEventData eventData)
        {
            return eventData.pointerCurrentRaycast.gameObject == eventData.pointerPressRaycast.gameObject;
        }
        
        protected void Clean()
        {
            if (_current != null)
            {
                _current.OnCompleted();
                _current.Dispose();
                _current = null;
            }
        }

        protected void Begin(TouchPayload touchPayload)
        {
            Clean();
            _current = new BehaviorSubject<TouchPayload>(touchPayload);
            _channel.Pipe(_current.AsObservable());
        }

        protected void Move(TouchPayload touchPayload)
        {
            if (_current != null)
            {
                _current.OnNext(touchPayload);
            }
        }
        
        
    }    
}


