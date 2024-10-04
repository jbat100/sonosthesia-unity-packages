using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    public class EventTouchDriver : MonoBehaviour
    {
        [SerializeField] private CollisionTouchChannel _channel;

        private BehaviorSubject<CollisionTouch> _current;

        private readonly TouchPayloadBuilder _builder = new TouchPayloadBuilder();

        protected virtual CollisionTouch MakePayload(PointerEventData data)
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

        protected void Begin(CollisionTouch collisionTouch)
        {
            Clean();
            _current = new BehaviorSubject<CollisionTouch>(collisionTouch);
            _channel.Pipe(_current.AsObservable());
        }

        protected void Move(CollisionTouch collisionTouch)
        {
            if (_current != null)
            {
                _current.OnNext(collisionTouch);
            }
        }
        
        
    }    
}


