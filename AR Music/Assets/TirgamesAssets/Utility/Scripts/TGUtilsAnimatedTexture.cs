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

    [Header("基础设置")]
    public bool PlayOnStart = true;
    public bool Loop = true;
    public TMap Map = TMap.Main;
    public List<TAnimation> Animations = new List<TAnimation>();

    [Header("驱动模式")]
    [Tooltip("选 Beat: 打到节拍才切；选 Freq: 实时频率映射")]
    public bool UseFrequencyMapping = false;

    [Header("节拍驱动设置 (模式=Beat)")]
    [Tooltip("触发一次节拍所需的最低强度")]
    public float BeatThreshold = 0.1f;
    [Tooltip("整体强度系数，放大/衰减频谱值")]
    public float BeatIntensity = 1f;
    [Tooltip("前 N 个低频段求平均")]
    public int BeatBandsToAverage = 8;

    // 动态阈值
    float beatCutoff = 0f;
    float beatDecayRate = 0.97f;
    int beatHoldTime = 60;
    int beatHoldCounter = 0;

    [Header("频率映射设置 (模式=Freq)")]
    [Range(0, 63)]
    public int FreqBandIndex = 0;
    [Tooltip("映射到索引前的放大系数")]
    public float FreqIntensity = 1f;

    // —— 私有状态 —— //
    Material mat;
    int animID = -1;
    TAnimation currentAnim;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        beatCutoff = BeatThreshold;

        if (Animations == null || Animations.Count == 0)
        {
            Debug.LogWarning("请在 Inspector 填入至少一个 TAnimation。");
            enabled = false;
            return;
        }

        if (PlayOnStart) NextAnimation();
    }

    void Update()
    {
        if (UseFrequencyMapping)
            FrequencyMapAdvance();
        else
            BeatDetectAdvance();
    }

    //—— 节拍方案 ——//
    void BeatDetectAdvance()
    {
        var buf = AudioPeer._audioBandBuffer;
        if (buf == null || buf.Length == 0) return;

        int cnt = Mathf.Min(BeatBandsToAverage, buf.Length);
        float sum = 0f;
        for (int i = 0; i < cnt; i++) sum += buf[i];
        float level = (sum / cnt) * BeatIntensity;

        // Debug.Log($"level={level:F3}, cutoff={beatCutoff:F3}");

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

    //—— 频率映射方案 ——//
    void FrequencyMapAdvance()
    {
        var buf = AudioPeer._audioBandBuffer;
        if (buf == null || buf.Length == 0) return;

        float lvl = buf[Mathf.Clamp(FreqBandIndex, 0, buf.Length - 1)] * FreqIntensity;
        lvl = Mathf.Clamp01(lvl);

        // map [0,1] → [0, Count-1]
        int newID = Mathf.FloorToInt(lvl * Animations.Count);
        newID = Mathf.Clamp(newID, 0, Animations.Count - 1);

        if (newID != animID)
        {
            StopAllCoroutines();
            animID = newID;
            StartCoroutine(AnimationPlay());
        }
    }

    //—— 切下一段动画 ——//
    void NextAnimation()
    {
        StopAllCoroutines();
        animID = (animID + 1) % Animations.Count;
        StartCoroutine(AnimationPlay());
    }

    //—— 帧动画播放（同原版） ——//
    IEnumerator AnimationPlay()
    {
        if (Animations == null || Animations.Count == 0)
            yield break;

        currentAnim = Animations[animID];
        float stepx = 1f / currentAnim.XTiles;
        float stepy = 1f / currentAnim.YTiles;
        string prop = (Map == TMap.Main ? "_MainTex" : "_EmissionMapScaleOffset");

        // 应用贴图材质
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

                        // —— 可选：实时调速 ——//
                        float audioLv = 0f;
                        var buf2 = AudioPeer._audioBandBuffer;
                        if (buf2 != null && buf2.Length > 0)
                            audioLv = buf2[Mathf.Clamp(FreqBandIndex, 0, buf2.Length - 1)] * FreqIntensity;

                        float fps = currentAnim.FPS * (1f + audioLv);
                        fps = Mathf.Max(fps, 0.1f);
                        yield return new WaitForSeconds(1f / fps);
                    }
                }
            }

            if (!Loop) break;

        } while (true);

        // 如果不循环，动画播完后自动切下一段
        if (!Loop)
            NextAnimation();
    }
}
