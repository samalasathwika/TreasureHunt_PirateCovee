using UnityEngine;

// Keeps the sea plane under the player as they run.
public class FollowZ : MonoBehaviour
{
    public Transform target;
    void LateUpdate()
    {
        if (target == null) return;
        var p = transform.position; p.z = target.position.z; transform.position = p;
    }
}
