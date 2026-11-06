using UnityEngine;

// Swings arms and legs while running (used by the hero and the chasing pirates).
public class RunAnimator : MonoBehaviour
{
    public Transform legL, legR, armL, armR;
    public float swing = 50f;

    void Update()
    {
        float speed = RunnerGame.I != null ? RunnerGame.I.Speed : 8f;
        float a = Mathf.Sin(Time.time * speed * 0.9f) * swing;
        if (legL) legL.localRotation = Quaternion.Euler(a, 0, 0);
        if (legR) legR.localRotation = Quaternion.Euler(-a, 0, 0);
        if (armL) armL.localRotation = Quaternion.Euler(-a * 0.8f, 0, 0);
        if (armR) armR.localRotation = Quaternion.Euler(a * 0.8f, 0, 0);
    }
}
