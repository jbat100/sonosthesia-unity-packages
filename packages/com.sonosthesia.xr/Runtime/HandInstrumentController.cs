using UnityEngine;
using UnityEngine.XR.Hands;

namespace Sonosthesia.XR
{
    public class HandInstrumentController : MonoBehaviour
    {
        [SerializeField] private XRHandSkeletonDriver _driver;

        [SerializeField] private GameObject _jointPrefab;
    }
}
