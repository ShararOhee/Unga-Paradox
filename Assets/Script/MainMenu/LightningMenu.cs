using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// AI was used to create the lightning menu script.
public class LightningMenu : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Seconds between lightning events (random in this range).")]
    public Vector2 intervalRange = new Vector2(2.5f, 6f);
    [Tooltip("How long a single bolt is visible, seconds.")]
    public float boltLifetime = 0.12f;

    [Header("Bolt Shape")]
    [Tooltip("How many jagged segments (higher = more detailed).")]
    [Range(6, 64)] public int segments = 24;
    [Tooltip("Horizontal/vertical jitter amount in screen % (0..1).")]
    [Range(0f, 0.25f)] public float jitter = 0.08f;
    [Tooltip("Chance for a small fork to appear.")]
    [Range(0f, 1f)] public float forkChance = 0.45f;
    [Tooltip("How long fork branches are, relative to main bolt length.")]
    [Range(0.1f, 0.6f)] public float forkLengthFactor = 0.35f;

    [Header("Visuals")]
    [Tooltip("Material with additive glow (e.g. Particles/Additive).")]
    public Material lineMaterial;
    [Tooltip("Start width of the bolt line.")]
    public float lineWidth = 0.06f;
    [Tooltip("Color & alpha of the bolt.")]
    public Color boltColor = new Color(1f, 1f, 1f, 1f);

    [Header("Flash")]
    [Tooltip("Optional full-screen Image on an Overlay Canvas to flash white.")]
    public Image flashImage;
    [Tooltip("Max flash alpha.")]
    [Range(0f, 1f)] public float flashAlpha = 0.35f;
    [Tooltip("How fast the flash fades.")]
    public float flashFadeSpeed = 3.5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] thunderClips;
    [Tooltip("Random delay before thunder (seconds).")]
    public Vector2 thunderDelayRange = new Vector2(0.1f, 0.6f);

    [Header("Camera")]
    [Tooltip("Camera used for ScreenToWorldPoint. Usually your UI/Main Camera.")]
    public Camera targetCamera;

    // Internal pool so we don’t GC every time
    private readonly List<GameObject> _activeBolts = new();

    void Awake()
    {
        if (!targetCamera) targetCamera = Camera.main;
    }

    void OnEnable()
    {
        StartCoroutine(LightningLoop());
    }

    IEnumerator LightningLoop()
    {
        while (true)
        {
            float wait = Random.Range(intervalRange.x, intervalRange.y);
            yield return new WaitForSeconds(wait);

            // 1–3 bolts in a “burst” feels nice on a menu
            int burst = Random.Range(1, 4);
            for (int i = 0; i < burst; i++)
            {
                SpawnBoltEvent();
                // small stagger within the burst
                yield return new WaitForSeconds(Random.Range(0.03f, 0.12f));
            }
        }
    }

    void SpawnBoltEvent()
    {
        // Choose random start/end across the screen; bias top->bottom or left->right
        bool vertical = Random.value > 0.5f;
        Vector2 startScreen, endScreen;

        if (vertical)
        {
            float x = Random.Range(0.05f, 0.95f) * Screen.width;
            startScreen = new Vector2(x, Screen.height * 1.05f);     // slightly off-screen
            endScreen   = new Vector2(x + Random.Range(-0.2f, 0.2f) * Screen.width, Screen.height * -0.05f);
        }
        else
        {
            float y = Random.Range(0.15f, 0.85f) * Screen.height;
            startScreen = new Vector2(-0.05f * Screen.width, y);
            endScreen   = new Vector2(1.05f * Screen.width,  y + Random.Range(-0.2f, 0.2f) * Screen.height);
        }

        var worldStart = ScreenToWorldNearPlane(startScreen);
        var worldEnd   = ScreenToWorldNearPlane(endScreen);

        // main bolt
        var mainBolt = CreateBoltObject("LightningBolt_Main");
        var mainLR = mainBolt.GetComponent<LineRenderer>();
        var mainPath = GenerateJaggedPath(worldStart, worldEnd, segments, jitter);
        ApplyPath(mainLR, mainPath);
        StartCoroutine(FadeAndDestroy(mainBolt, boltLifetime));

        // fork (optional)
        if (Random.value < forkChance)
        {
            var forkBolt = CreateBoltObject("LightningBolt_Fork");
            var forkLR = forkBolt.GetComponent<LineRenderer>();

            // pick a point along the main path to branch from
            int idx = Random.Range(2, Mathf.Max(3, mainPath.Count - 3));
            Vector3 forkStart = mainPath[idx];
            // fork direction is slightly off the main segment
            Vector3 dir = (mainPath[Mathf.Min(idx + 1, mainPath.Count - 1)] - mainPath[idx]).normalized;
            Vector3 perp = Vector3.Cross(dir, Vector3.forward).normalized;

            float forkLength = Vector3.Distance(worldStart, worldEnd) * forkLengthFactor;
            Vector3 forkEnd = forkStart + (dir + perp * Random.Range(-0.7f, 0.7f)).normalized * forkLength;

            var forkPath = GenerateJaggedPath(forkStart, forkEnd, Mathf.Max(6, segments / 2), jitter * 0.7f);
            ApplyPath(forkLR, forkPath);
            StartCoroutine(FadeAndDestroy(forkBolt, boltLifetime * 0.9f));
        }

        // screen flash
        if (flashImage)
            StartCoroutine(FlashRoutine());

        // thunder
        if (audioSource && thunderClips != null && thunderClips.Length > 0)
            StartCoroutine(PlayThunderAfterDelay(Random.Range(thunderDelayRange.x, thunderDelayRange.y)));
    }

    GameObject CreateBoltObject(string name)
    {
        var go = new GameObject(name);
        _activeBolts.Add(go);
        var lr = go.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.positionCount = 0;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth * 0.85f;
        lr.textureMode = LineTextureMode.Stretch;
        lr.alignment = LineAlignment.View;
        lr.useWorldSpace = true;
        lr.numCornerVertices = 2;
        lr.numCapVertices = 2;
        lr.startColor = boltColor;
        lr.endColor = boltColor;
        return go;
    }

    IEnumerator FadeAndDestroy(GameObject bolt, float life)
    {
        var lr = bolt.GetComponent<LineRenderer>();
        float t = 0f;
        Color c0 = lr.startColor;
        Color c1 = lr.endColor;

        // Quick hold then fade
        float hold = life * 0.35f;
        float fade = life - hold;

        yield return new WaitForSeconds(hold);
        while (t < fade)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fade);
            lr.startColor = new Color(c0.r, c0.g, c0.b, a);
            lr.endColor   = new Color(c1.r, c1.g, c1.b, a);
            yield return null;
        }

        _activeBolts.Remove(bolt);
        Destroy(bolt);
    }

    IEnumerator FlashRoutine()
    {
        // brief flash then fade out
        float a = flashAlpha;
        SetFlashAlpha(a);
        while (a > 0f)
        {
            a -= Time.deltaTime * flashFadeSpeed;
            SetFlashAlpha(Mathf.Max(0f, a));
            yield return null;
        }
    }

    void SetFlashAlpha(float a)
    {
        var c = flashImage.color;
        c.a = a;
        flashImage.color = c;
    }

    IEnumerator PlayThunderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        var clip = thunderClips[Random.Range(0, thunderClips.Length)];
        audioSource.PlayOneShot(clip);
    }

    Vector3 ScreenToWorldNearPlane(Vector2 screenPos)
    {
        // Draw at a tiny offset from the near clip so it overlays the scene
        float z = Mathf.Max(0.5f, targetCamera.nearClipPlane + 0.5f);
        var v = targetCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));
        // Lock Z so bolts don’t sort behind UI (for 2D setups, set to 0 as needed)
        v.z = 0f;
        return v;
    }

    List<Vector3> GenerateJaggedPath(Vector3 start, Vector3 end, int count, float jitterPercent)
    {
        var pts = new List<Vector3>(count);
        Vector3 dir = (end - start);
        float length = dir.magnitude;
        dir.Normalize();

        Vector3 perp = Vector3.Cross(dir, Vector3.forward).normalized;
        float jitterMag = length * jitterPercent;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / (count - 1);
            // base along the line
            Vector3 p = Vector3.Lerp(start, end, t);
            // falloff so ends are less jagged
            float falloff = Mathf.Sin(Mathf.PI * t);
            // random offset both perpendicular and slightly along the line
            Vector3 offset = perp * Random.Range(-jitterMag, jitterMag) * falloff
                           + dir  * Random.Range(-jitterMag * 0.15f, jitterMag * 0.15f) * falloff;
            pts.Add(p + offset);
        }
        return pts;
    }

    void ApplyPath(LineRenderer lr, List<Vector3> path)
    {
        lr.positionCount = path.Count;
        lr.SetPositions(path.ToArray());
    }
}
