using UnityEngine;

// Coins, treasure chests and stars. Tag "Collectible", collider Is Trigger.
public class Pickup : MonoBehaviour
{
    public enum Kind { Coin, Chest, Star }
    public Kind kind = Kind.Coin;
    public int value = 10;
    public bool spin = true;
    public float bobHeight = 0.12f, bobSpeed = 3f;

    float startY;
    void Start() { startY = transform.localPosition.y; }

    void Update()
    {
        if (spin) transform.Rotate(0f, 140f * Time.deltaTime, 0f, Space.World);
        var p = transform.localPosition;
        p.y = startY + Mathf.Sin(Time.time * bobSpeed + transform.position.z) * bobHeight;
        transform.localPosition = p;
    }
}
