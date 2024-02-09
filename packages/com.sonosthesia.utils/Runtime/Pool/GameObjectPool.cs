using UnityEngine;
using UnityEngine.Pool;

namespace Sonosthesia.Utils
{
    [CreateAssetMenu(fileName = "Pool", menuName = "Sonosthesia/Pool/GameObjectPool")]
    public class GameObjectPool : ScriptableRootedPool<GameObject>
    {
        [SerializeField] private GameObject _prefab;
        
        private ObjectPool<GameObject> _pool;

        public override IObjectPool<GameObject> Pool
        {
            get
            {
                if (_pool != null)
                {
                    return _pool;
                }

                _pool = new ObjectPool<GameObject>(
                    () =>
                    {
                        GameObject gameObject = Instantiate(_prefab, Root, false);
                        return gameObject;
                    }, 
                    instance => instance.SetActive(true), 
                    instance =>
                    {
                        instance.SetActive(false);
                        instance.transform.SetParent(Root, false);
                    }, 
                    instance => Destroy(instance));

                return _pool;
            }
        }
    }
}