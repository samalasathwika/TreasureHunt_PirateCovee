using UnityEngine;
using UnityEngine.Rendering;

// Gives every level its own look: 1 = Moonlit Night, 2 = Sunny Day, 3 = Golden Sunset, 4 = Storm at Skull Rock.
public class LevelTheme : MonoBehaviour
{
    public Material skyBase, moonMat, starMat, cloudMat, rainMat;
    public Renderer seaRenderer;
    public Light sun;

    Transform player, skyRoot;
    Transform[] drops;
    Light lampLight;
    bool storm;
    float nextBolt, boltEnd, baseSun, baseAmb;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p == null) return;
        player = p.transform;
        skyRoot = new GameObject("SkyDecor").transform;
        switch (Mathf.Clamp(Save.SelectedLevel, 1, Save.LevelCount))
        {
            case 1: Night(); break;
            case 2: Day(); break;
            case 3: Sunset(); break;
            default: Storm(); break;
        }
    }

    void Setup(Color skyTint, Color ground, float exposure, float sunSize, float thickness, Color fog, float fogDensity,
               Color lightColor, float lightIntensity, Vector3 lightRot, float ambient, Color seaColor)
    {
        if (skyBase != null)
        {
            var sky = new Material(skyBase);
            sky.SetColor("_SkyTint", skyTint); sky.SetColor("_GroundColor", ground);
            sky.SetFloat("_Exposure", exposure); sky.SetFloat("_SunSize", sunSize); sky.SetFloat("_AtmosphereThickness", thickness);
            RenderSettings.skybox = sky;
        }
        RenderSettings.fog = true; RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = fog; RenderSettings.fogDensity = fogDensity;
        if (sun != null)
        {
            sun.color = lightColor; sun.intensity = lightIntensity; sun.transform.rotation = Quaternion.Euler(lightRot);
            RenderSettings.sun = sun; baseSun = lightIntensity;
        }
        RenderSettings.ambientMode = AmbientMode.Skybox; RenderSettings.ambientIntensity = ambient; baseAmb = ambient;
        if (seaRenderer != null) seaRenderer.material.color = seaColor;
        DynamicGI.UpdateEnvironment();
    }

    void Night()
    {
        Setup(new Color(0.05f, 0.07f, 0.22f), new Color(0.02f, 0.03f, 0.08f), 0.35f, 0f, 0.6f, new Color(0.05f, 0.08f, 0.17f), 0.0045f,
              new Color(0.55f, 0.65f, 1f), 0.85f, new Vector3(40, 160, 0), 0.9f, new Color(0.03f, 0.14f, 0.28f));
        Stars(); Moon(); AddLamp();
    }

    void Day()
    {
        Setup(new Color(0.35f, 0.55f, 0.92f), new Color(0.5f, 0.55f, 0.6f), 1.3f, 0.04f, 0.9f, new Color(0.75f, 0.88f, 0.98f), 0.004f,
              new Color(1f, 0.97f, 0.9f), 1.6f, new Vector3(55, -30, 0), 1f, new Color(0.1f, 0.62f, 0.82f));
        Clouds(Color.white);
    }

    void Sunset()
    {
        Setup(new Color(1f, 0.5f, 0.25f), new Color(0.4f, 0.3f, 0.3f), 1.15f, 0.09f, 2f, new Color(0.95f, 0.6f, 0.42f), 0.005f,
              new Color(1f, 0.55f, 0.3f), 1.4f, new Vector3(12, 150, 0), 0.95f, new Color(0.45f, 0.35f, 0.45f));
        Clouds(new Color(1f, 0.7f, 0.5f));
    }

    void Storm()
    {
        Setup(new Color(0.25f, 0.27f, 0.3f), new Color(0.2f, 0.2f, 0.22f), 0.55f, 0f, 1.5f, new Color(0.35f, 0.4f, 0.45f), 0.011f,
              new Color(0.7f, 0.75f, 0.85f), 0.8f, new Vector3(50, -20, 0), 0.8f, new Color(0.05f, 0.2f, 0.25f));
        Clouds(new Color(0.3f, 0.32f, 0.36f)); Rain(); AddLamp();
        storm = true; nextBolt = Time.time + 4f;
    }

    GameObject Ball(Material m, Vector3 localPos, Vector3 scale)
    {
        var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(s.GetComponent<Collider>());
        s.transform.SetParent(skyRoot, false);
        s.transform.localPosition = localPos; s.transform.localScale = scale;
        s.GetComponent<Renderer>().sharedMaterial = m;
        return s;
    }

    void Stars()
    {
        for (int i = 0; i < 140; i++)
        {
            Vector3 dir = Quaternion.Euler(-Random.Range(2f, 16f), Random.Range(-55f, 55f), 0) * Vector3.forward;
            Ball(starMat, dir * 100f, Vector3.one * Random.Range(0.4f, 1.1f));
        }
    }

    void Moon() { Ball(moonMat, new Vector3(-40, 18, 100), Vector3.one * 10f); }

    void Clouds(Color tint)
    {
        var m = new Material(cloudMat); m.color = tint;
        for (int i = 0; i < 12; i++)
        {
            float x = (Random.value < 0.5f ? -1f : 1f) * Random.Range(20f, 80f);
            Vector3 c = new Vector3(x, Random.Range(10f, 22f), Random.Range(70f, 130f));
            for (int k = 0; k < 3; k++)
                Ball(m, c + new Vector3(Random.Range(-6f, 6f), Random.Range(-1f, 1f), Random.Range(-3f, 3f)),
                     new Vector3(Random.Range(10f, 18f), Random.Range(3f, 5f), Random.Range(6f, 9f)));
        }
    }

    void Rain()
    {
        drops = new Transform[140];
        for (int i = 0; i < drops.Length; i++)
        {
            var d = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(d.GetComponent<Collider>());
            d.transform.localScale = new Vector3(0.03f, 0.7f, 0.03f);
            d.GetComponent<Renderer>().sharedMaterial = rainMat;
            d.transform.position = new Vector3(Random.Range(-14f, 14f), Random.Range(-1f, 14f), player.position.z + Random.Range(-4f, 24f));
            drops[i] = d.transform;
        }
    }

    void AddLamp()
    {
        var go = new GameObject("PlayerLamp");
        lampLight = go.AddComponent<Light>();
        lampLight.type = LightType.Point; lampLight.color = new Color(1f, 0.75f, 0.45f); lampLight.range = 14f;
        lampLight.intensity = GraphicsSettings.currentRenderPipeline != null ? 8f : 2f;
    }

    void Update()
    {
        if (drops == null || player == null) return;
        foreach (var d in drops)
        {
            Vector3 p = d.position;
            p.y -= 30f * Time.deltaTime;
            if (p.y < -1f) p = new Vector3(Random.Range(-14f, 14f), Random.Range(8f, 14f), player.position.z + Random.Range(-4f, 24f));
            d.position = p;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;
        if (skyRoot != null) skyRoot.position = new Vector3(0, 0, player.position.z);
        if (lampLight != null) lampLight.transform.position = player.position + new Vector3(0, 3.5f, 3f);
        if (storm && sun != null)
        {
            if (Time.time >= nextBolt) { boltEnd = Time.time + 0.12f; nextBolt = Time.time + Random.Range(4f, 9f); }
            bool flash = Time.time < boltEnd;
            sun.intensity = flash ? baseSun * 3.5f : baseSun;
            RenderSettings.ambientIntensity = flash ? baseAmb * 2f : baseAmb;
        }
    }
}
