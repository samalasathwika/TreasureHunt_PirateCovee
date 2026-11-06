using UnityEngine;

// Pirates run behind the hero. Every hit lets them close in; at 0 lives they catch you.
public class PirateChasers : MonoBehaviour
{
    public static PirateChasers I;
    public Transform[] chasers;
    public float[] laneOffsets = { -1.6f, 1.6f, 0f };

    Transform player;
    float gap = 9f;
    bool caught;

    void Awake() { I = this; }
    void Start() { player = GameObject.FindWithTag("Player").transform; PlaceAll(0f); }

    void LateUpdate()
    {
        var g = RunnerGame.I;
        if (player == null || g == null) return;
        float want = caught ? 1.4f : (g.Lives >= 3 ? 9f : (g.Lives == 2 ? 6f : 3.8f));
        gap = Mathf.MoveTowards(gap, want, 3f * Time.deltaTime);
        PlaceAll(Time.deltaTime);
    }

    public void SnapClose() { caught = true; gap = 1.4f; PlaceAll(10f); }

    void PlaceAll(float dt)
    {
        if (player == null) return;
        float speed = RunnerGame.I != null ? RunnerGame.I.Speed : 8f;
        for (int i = 0; i < chasers.Length; i++)
        {
            var c = chasers[i];
            Vector3 p = c.position;
            float tx = player.position.x * 0.8f + laneOffsets[i % laneOffsets.Length];
            p.x = dt >= 1f ? tx : Mathf.Lerp(p.x, tx, 4f * dt);
            p.z = player.position.z - gap - i * 2.4f;
            p.y = Mathf.Abs(Mathf.Sin(Time.time * speed * 0.9f + i)) * 0.1f;
            c.position = p;
        }
    }
}
