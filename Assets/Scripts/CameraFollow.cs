using UnityEngine;

// Chase camera behind and above the runner.
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 4.5f, -7f);
    public float smooth = 8f;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 want = new Vector3(target.position.x * 0.5f, offset.y, target.position.z + offset.z);
        transform.position = Vector3.Lerp(transform.position, want, smooth * Time.deltaTime);
        transform.LookAt(new Vector3(target.position.x * 0.5f, 1f, target.position.z + 6f));
    }
}
