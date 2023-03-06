using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private Transform _target; 
    
    // Update is called once per frame
    void Update()
    {
        this.transform.position = _target.position;
        this.transform.rotation = _target.rotation;
    }
}
