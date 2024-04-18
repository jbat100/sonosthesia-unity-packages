using UnityEngine;

namespace Sonosthesia.Timeline
{
    public class TestSignal : MonoBehaviour
    {
        public void Test(string test)
        {
            Debug.Log($"{this.name} {nameof(TestSignal)} received signal {test}");
        }
    }
}