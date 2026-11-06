using System.Collections.Generic;
using UnityEngine;

// Decorative cannon fire from the pirate ships (lands in the water, never hits the player).
public class ShipCannons : MonoBehaviour
{
    public Transform[] muzzles;
    public GameObject ballPrefab;
    public Vector2 interval = new Vector2(3f, 7f);

    Transform player;
    float t;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        player = p != null ? p.transform : null;
        t = Random.Range(interval.x, interval.y);
    }

    void Update()
    {
        t -= Time.deltaTime;
        if (t > 0f || muzzles == null || muzzles.Length == 0 || ballPrefab == null) return;
        t = Random.Range(interval.x, interval.y);
        var m = muzzles[Random.Range(0, muzzles.Length)];
        var ball = Instantiate(ballPrefab, m.position, Quaternion.identity);
        Vector3 target;
        if (player != null)
        {
            float side = Mathf.Sign(m.position.x);
            target = new Vector3(side * Random.Range(5f, 9f), -1.4f, player.position.z + Random.Range(6f, 28f));
        }
        else target = m.position + m.right * 14f * Mathf.Sign(m.localPosition.x) + new Vector3(0, -2.5f, Random.Range(-4f, 4f));
        ball.GetComponent<CannonBall>().Launch(target, 1.7f);
    }
}
