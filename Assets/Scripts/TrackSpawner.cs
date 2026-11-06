using System.Collections.Generic;
using UnityEngine;

// Endless pier: spawns track segments, obstacles, gold trails (mostly down the middle), chests, stars and treasure islands.
public class TrackSpawner : MonoBehaviour
{
    public GameObject segmentPrefab, islandPrefab;
    public GameObject[] obstaclePrefabs;
    public GameObject coinPrefab, chestPrefab, starPrefab;
    public float segmentLength = 20f, laneWidth = 2f;
    public int segmentsAhead = 7;

    readonly Queue<GameObject> segs = new Queue<GameObject>();
    float nextZ = -20f, nextStarAt, starStep;
    int starsPlaced, level;
    Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        level = Mathf.Clamp(Save.SelectedLevel, 1, Save.LevelCount);
        starStep = Save.TargetDistance(level) / 6f;
        nextStarAt = starStep;
    }

    void Update()
    {
        while (nextZ < player.position.z + segmentsAhead * segmentLength) SpawnSegment();
        while (segs.Count > 0 && segs.Peek().transform.position.z + segmentLength < player.position.z - 20f) Destroy(segs.Dequeue());
    }

    void SpawnSegment()
    {
        var seg = Instantiate(segmentPrefab, new Vector3(0, 0, nextZ), Quaternion.identity);
        segs.Enqueue(seg);
        if (nextZ >= 20f) Populate(seg.transform, nextZ);
        if (islandPrefab != null && Random.value < 0.6f)
        {
            int side = Random.value < 0.5f ? -1 : 1;
            var isl = Instantiate(islandPrefab, seg.transform);
            isl.transform.localPosition = new Vector3(side * Random.Range(14f, 21f), -1f, Random.Range(2f, 18f));
            isl.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
        nextZ += segmentLength;
    }

    int PickLane(List<int> free)
    {
        if (free.Contains(0) && Random.value < 0.55f) return 0;     // gold likes the middle lane
        return free[Random.Range(0, free.Count)];
    }

    void Populate(Transform seg, float z)
    {
        float chance = Save.ObstacleChance(level);
        for (int row = 0; row < 4; row++)
        {
            float lz = row * 5f + 2.5f;
            var free = new List<int> { -1, 0, 1 };

            if (Random.value < chance)
            {
                int n = Random.value < 0.35f ? 2 : 1;      // never block all three lanes
                for (int i = 0; i < n; i++)
                {
                    int idx = Random.Range(0, free.Count);
                    int lane = free[idx]; free.RemoveAt(idx);
                    var prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                    Place(prefab, seg, lane, lz);
                    if (!prefab.name.StartsWith("CrateStack") && Random.value < 0.4f)     // coin arc to jump through
                        for (int k = 0; k < 5; k++)
                        {
                            float t = (k - 2) / 2f;
                            Place(coinPrefab, seg, lane, lz - 2f + k, 1.0f + 1.2f * (1f - t * t));
                        }
                }
            }

            bool starDue = row == 2 && starsPlaced < 5 && nextStarAt >= z && nextStarAt < z + segmentLength;
            if (starDue && free.Count > 0)
            {
                Place(starPrefab, seg, PickLane(free), lz);
                starsPlaced++; nextStarAt += starStep;
            }
            else if (free.Count > 0 && Random.value < 0.14f)
                Place(chestPrefab, seg, PickLane(free), lz);
            else if (free.Count > 0 && Random.value < 0.6f)
            {
                int lane = PickLane(free);
                int count = Random.Range(4, 7);
                for (int k = 0; k < count; k++) Place(coinPrefab, seg, lane, lz - 2.5f + k * 1.1f);
            }
        }
    }

    void Place(GameObject prefab, Transform parent, int lane, float localZ, float y = -999f)
    {
        var o = Instantiate(prefab, parent);
        o.transform.localPosition = new Vector3(lane * laneWidth, y > -900f ? y : prefab.transform.position.y, localZ);
        o.transform.localRotation = prefab.transform.rotation;
    }
}
