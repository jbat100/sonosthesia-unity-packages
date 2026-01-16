using UnityEngine;
using UnityEngine.InputSystem;

namespace Sonosthesia.Utils
{
    public class RigidbodyLauncher : MonoBehaviour
    {
        [SerializeField] private Rigidbody _prefab;
        [SerializeField] private Camera _camera;
        [SerializeField] private float _launchSpeed = 20f;
        [SerializeField] private float _lifeTime = 5f;
        [SerializeField] private float _spawnOffsetAlongRay = 0.25f;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        private void Update()
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                LaunchAtMouse();
            }
        }

        private void LaunchAtMouse()
        {
            if (!_prefab || !_camera)
            {
                return;
            }

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(mousePos);
            Vector3 spawnPos =  ray.origin + ray.direction * Mathf.Max(0f, _spawnOffsetAlongRay);
            Quaternion spawnRot = Quaternion.LookRotation(ray.direction, Vector3.up);

            Rigidbody instance = Instantiate(_prefab, spawnPos, spawnRot);

            instance.SetLinearVelocity(ray.direction * _launchSpeed);

            if (_lifeTime > 0f)
            {
                Destroy(instance.gameObject, _lifeTime);
            }
        }
    }
}