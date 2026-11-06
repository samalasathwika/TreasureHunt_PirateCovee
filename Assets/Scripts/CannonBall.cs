using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject splashPrefab;
    Vector3 v;
    const float G = -9.8f;

    public void Launch(Vector3 target, float time)
    {
        v = (target - transform.position - 0.5f * new Vector3(0, G, 0) * time * time) / time;
    }

    void Update()
    {
        v.y += G * Time.deltaTime;
        transform.position += v * Time.deltaTime;
        if (transform.position.y < -1.4f)
        {
            if (splashPrefab != null) Instantiate(splashPrefab, new Vector3(transform.position.x, -1.4f, transform.position.z), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
