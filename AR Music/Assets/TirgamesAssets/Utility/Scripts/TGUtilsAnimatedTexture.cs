using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Renderer))]
public class TGUtilsAnimatedTexture : MonoBehaviour
{
    public enum TMap { Main, Emission }

    [System.Serializable]
    public class TAnimation
    {
        public Texture AnimTexture;
        public int PlayCount = 1;
        public int XTiles = 1;
        public int YTiles = 1;
        public float FPS = 25f;
    }

    [Header("Basic Configuration")]
    public bool PlayOnStart = true;
    public bool Loop = true;
    public TMap Map = TMap.Main;
    public List<TAnimation> Animations = new List<TAnimation>();

    [Header("Beat Driven Configuration")]
    [Tooltip("Lowest Intensity")]
    public float BeatThreshold = 0.1f;
    [Tooltip("Whole Intensity")]
    public float BeatIntensity = 1f;
    [Tooltip("Samples/Bands")]
    public int BeatBandsToAverage = 8;

    // Dynamic Threshold //
    float beatCutoff = 0f;
    float beatDecayRate = 0.97f;
    int beatHoldTime = 60;
    int beatHoldCounter = 0;

    Material mat;
    int animID = -1;
    TAnimation currentAnim;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        beatCutoff = BeatThreshold;

        if (Animations == null || Animations.Count == 0)
        {
            Debug.LogWarning("It needs at leat one animation");
            enabled = false;
            return;
        }

        if (PlayOnStart) NextAnimation();
    }

    void Update()
    {
        BeatDetectAdvance();
    }

    // Beat Driven Logic //
    void BeatDetectAdvance()
    {
        var buf = AudioPeer._audioBandBuffer;
        if (buf == null || buf.Length == 0) return;

        int cnt = Mathf.Min(BeatBandsToAverage, buf.Length);
        float sum = 0f;
        for (int i = 0; i < cnt; i++) sum += buf[i];
        float level = (sum / cnt) * BeatIntensity;

        if (level > beatCutoff && level > BeatThreshold)
        {
            beatHoldCounter = 0;
            beatCutoff = level * 1.1f;
            NextAnimation();
        }
        else if (beatHoldCounter >= beatHoldTime)
        {
            beatCutoff *= beatDecayRate;
            beatCutoff = Mathf.Max(beatCutoff, BeatThreshold);
        }
        else
        {
            beatHoldCounter++;
        }
    }

    // Play Next Anim//
    void NextAnimation()
    {
        StopAllCoroutines();
        animID = (animID + 1) % Animations.Count;
        StartCoroutine(AnimationPlay());
    }

    // Animation Play //
    IEnumerator AnimationPlay()
    {
        if (Animations == null || Animations.Count == 0)
            yield break;

        currentAnim = Animations[animID];
        float stepx = 1f / currentAnim.XTiles;
        float stepy = 1f / currentAnim.YTiles;
        string prop = (Map == TMap.Main ? "_MainTex" : "_EmissionMapScaleOffset");

        if (Map == TMap.Main)
            mat.mainTexture = currentAnim.AnimTexture;
        else
        {
            mat.SetTexture("_EmissionMap", currentAnim.AnimTexture);
            mat.EnableKeyword("_EMISSION");
        }

        do
        {
            for (int p = 0; p < currentAnim.PlayCount; p++)
            {
                for (int y = 0; y < currentAnim.YTiles; y++)
                {
                    for (int x = 0; x < currentAnim.XTiles; x++)
                    {
                        Vector4 so = new Vector4(
                            stepx,
                            stepy,
                            x * stepx,
                            1f - stepy - y * stepy
                        );
                        mat.SetVector(prop, so);

                        yield return new WaitForSeconds(1f / currentAnim.FPS);
                    }
                }
            }

            if (!Loop) break;

        } while (true);

        if (!Loop)
            NextAnimation();
    }
}
