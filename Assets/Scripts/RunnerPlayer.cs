using UnityEngine;

// 3-lane runner. Needs a kinematic Rigidbody + Capsule Collider. Tag "Player".
[RequireComponent(typeof(Rigidbody))]
public class RunnerPlayer : MonoBehaviour
{
    public float laneWidth = 2f, laneSpeed = 14f, jumpVelocity = 9.5f, gravity = -26f;

    int lane;
    float vy, groundY;
    Renderer[] rends;

    void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; rb.useGravity = false;
        groundY = transform.position.y;
        rends = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        var g = RunnerGame.I;
        if (g == null || g.state != RunnerGame.State.Playing) return;

        if (GameInput.LeftPressed())  lane = Mathf.Max(-1, lane - 1);
        if (GameInput.RightPressed()) lane = Mathf.Min(1, lane + 1);

        Vector3 p = transform.position;
        p.x = Mathf.MoveTowards(p.x, lane * laneWidth, laneSpeed * Time.deltaTime);
        p.z = g.Distance;

        bool grounded = p.y <= groundY + 0.001f && vy <= 0f;
        if (grounded && GameInput.JumpPressed())
        {
            vy = jumpVelocity;
            if (AudioManager.I != null) AudioManager.I.Play("jump");
        }
        vy += gravity * Time.deltaTime;
        p.y += vy * Time.deltaTime;
        if (p.y <= groundY) { p.y = groundY; vy = 0f; }
        transform.position = p;

        bool visible = !g.IsInvulnerable || ((int)(Time.time * 12f) % 2 == 0);
        foreach (var r in rends) r.enabled = visible;
    }

    void OnTriggerEnter(Collider other)
    {
        var g = RunnerGame.I;
        if (g == null || g.state != RunnerGame.State.Playing) return;

        if (other.CompareTag("Collectible"))
        {
            var pk = other.GetComponent<Pickup>();
            if (pk != null)
            {
                if (pk.kind == Pickup.Kind.Coin) g.AddCoin(pk.value);
                else if (pk.kind == Pickup.Kind.Chest) g.AddChest(pk.value);
                else g.AddStar();
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Hazard")) g.Hit();
    }
}
