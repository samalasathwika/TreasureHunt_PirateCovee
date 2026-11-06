using UnityEngine;

// Keeps scenery (ships, skull rock) at a fixed distance from the runner, with gentle drifting and bobbing.
public class FollowPlayer : MonoBehaviour
{
    public Vector3 offset;
    public float driftAmplitude = 0f, driftSpeed = 0.15f, bobAmplitude = 0f, bobSpeed = 1f, rollDegrees = 0f;

    Transform target;
    float phase, baseY;
    Vector3 basePos;
    Quaternion baseRot;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        target = p != null ? p.transform : null;
        phase = Random.value * 6.28f;
        basePos = transform.position; baseY = basePos.y; baseRot = transform.rotation;
    }

    void LateUpdate()
    {
        Vector3 pos = basePos;
        if (target != null) pos = new Vector3(offset.x, baseY, target.position.z + offset.z + Mathf.Sin(Time.time * driftSpeed + phase) * driftAmplitude);
        pos.y = baseY + Mathf.Sin(Time.time * bobSpeed + phase) * bobAmplitude;
        transform.position = pos;
        transform.rotation = baseRot * Quaternion.Euler(0, 0, Mathf.Sin(Time.time * bobSpeed * 0.8f + phase) * rollDegrees);
    }
}
