using UnityEngine;

public class SplashFx : MonoBehaviour
{
    float age;
    void Update()
    {
        age += Time.deltaTime;
        float k = age / 0.6f;
        transform.localScale = new Vector3(Mathf.Lerp(0.4f, 3.2f, k), Mathf.Lerp(0.6f, 0.05f, k), Mathf.Lerp(0.4f, 3.2f, k));
        if (k >= 1f) Destroy(gameObject);
    }
}
