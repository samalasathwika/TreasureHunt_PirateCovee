using UnityEngine;

// Generates simple sound effects and a looping tune in code (no audio files needed).
public class AudioManager : MonoBehaviour
{
    public static AudioManager I;
    AudioSource music, sfx;
    AudioClip coin, chest, hit, jump, star;

    public static void Ensure()
    {
        if (I != null) return;
        new GameObject("AudioManager").AddComponent<AudioManager>();
    }

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        music = gameObject.AddComponent<AudioSource>(); music.loop = true;
        sfx = gameObject.AddComponent<AudioSource>();
        coin = Tone(new[] { 988f, 1319f }, 0.18f, 0.5f);
        star = Tone(new[] { 784f, 1047f, 1568f }, 0.3f, 0.5f);
        chest = Tone(new[] { 523f, 659f, 784f, 1047f }, 0.45f, 0.5f);
        jump = Tone(new[] { 330f, 440f }, 0.14f, 0.4f);
        hit = Noise(0.35f);
        music.clip = Tune();
        ApplyVolumes();
        music.Play();
    }

    public void ApplyVolumes() { music.volume = Save.Music * 0.3f; sfx.volume = Save.Sfx; }

    public void Play(string name)
    {
        AudioClip c = name == "coin" ? coin : name == "chest" ? chest : name == "star" ? star : name == "jump" ? jump : hit;
        sfx.PlayOneShot(c);
    }

    AudioClip Tone(float[] notes, float dur, float vol)
    {
        int sr = 44100, n = (int)(sr * dur);
        var d = new float[n];
        for (int i = 0; i < n; i++)
        {
            int ni = Mathf.Min(notes.Length - 1, (int)((float)i / n * notes.Length));
            d[i] = Mathf.Sin(2f * Mathf.PI * notes[ni] * i / sr) * (1f - (float)i / n) * vol;
        }
        var c = AudioClip.Create("tone", n, 1, sr, false); c.SetData(d, 0); return c;
    }

    AudioClip Noise(float dur)
    {
        int sr = 44100, n = (int)(sr * dur);
        var d = new float[n];
        for (int i = 0; i < n; i++) d[i] = (Random.value * 2f - 1f) * Mathf.Pow(1f - (float)i / n, 2f) * 0.6f;
        var c = AudioClip.Create("hit", n, 1, sr, false); c.SetData(d, 0); return c;
    }

    AudioClip Tune()
    {
        float[] scale = { 261.6f, 329.6f, 392f, 440f, 523.3f, 440f, 392f, 329.6f, 293.7f, 349.2f, 440f, 392f, 349.2f, 329.6f, 293.7f, 261.6f };
        int sr = 22050, noteLen = sr / 4, n = noteLen * scale.Length;
        var d = new float[n];
        for (int i = 0; i < n; i++)
        {
            int k = i / noteLen; float t = (float)(i % noteLen) / noteLen;
            d[i] = Mathf.Sin(2f * Mathf.PI * scale[k] * i / sr) * Mathf.Pow(1f - t, 1.5f) * 0.5f;
        }
        var c = AudioClip.Create("tune", n, 1, sr, false); c.SetData(d, 0); return c;
    }
}
