using UnityEngine;
using Vuforia;
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using Mediapipe.Unity.Sample;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Concurrent;
using static Mediapipe.BoxDetectorOptions.Types;
using Newtonsoft.Json.Linq;

public class VuforiaHandLandmarkerRunner : VisionTaskApiRunner<HandLandmarker>
{
    public readonly HandLandmarkDetectionConfig config = new HandLandmarkDetectionConfig();
    [SerializeField] private Camera uiCamera;
    [SerializeField] private FingertipUIButtonSystem fingertipUIButtonSystem;
    private Vector2[] latestFingerScreenPoint = null;

    private RectTransform buttonRectTransform;
    private readonly ConcurrentQueue<Action> _mainThreadActions = new ConcurrentQueue<Action>();
    private bool wasPressing = false;
    private Mediapipe.Unity.Experimental.TextureFramePool _textureFramePool;
    private Texture2D cameraTexture;
    private Color32[] pixelBuffer;


    void Awake()
    {
        config.NumHands = 4;
        Debug.Log("Application.targetFrameRate" + Application.targetFrameRate);
    }

    void Update()
    {
        while (_mainThreadActions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }


    public override void Stop()
    {
        base.Stop();
        _textureFramePool?.Dispose();
        _textureFramePool = null;
    }

    IEnumerator WaitForVuforiaVideoTexture()
    {
        while (VuforiaBehaviour.Instance.VideoBackground.VideoBackgroundTexture == null)
        {
            Debug.Log("Waiting for Vuforia Video Background Texture...");
            yield return null;
        }

        Debug.Log("Vuforia Video Background Texture is ready.");
    }

    protected override IEnumerator Run()
    {
        yield return AssetLoader.PrepareAssetAsync(config.ModelPath);

        var options = config.GetHandLandmarkerOptions(OnHandLandmarkDetectionOutput);
        taskApi = HandLandmarker.CreateFromOptions(options, GpuManager.GpuResources);

        yield return WaitForVuforiaVideoTexture();


        // Initialize camera texture and pixel buffer
        var bgTexture = VuforiaBehaviour.Instance.VideoBackground.VideoBackgroundTexture;

        RenderTexture rt = new RenderTexture(bgTexture.width, bgTexture.height, 0, RenderTextureFormat.ARGB32);
        cameraTexture = new Texture2D(bgTexture.width, bgTexture.height, TextureFormat.RGBA32, false);
        pixelBuffer = new Color32[bgTexture.width * bgTexture.height];
        _textureFramePool = new Mediapipe.Unity.Experimental.TextureFramePool(cameraTexture.width, cameraTexture.height, TextureFormat.RGBA32, 5);

        var waitForEndOfFrame = new WaitForEndOfFrame();

        while (true)
        {
            if (isPaused)
            {
                yield return new WaitWhile(() => isPaused);
            }

            yield return waitForEndOfFrame;

            // Get Camera texture
            if (VuforiaBehaviour.Instance.VideoBackground.VideoBackgroundTexture == null)
            {
                Debug.LogWarning("Vuforia Video Background Texture is null");
                continue;
            }

            // Use Graphics.Blit to change RenderTexture to Texture2D
            Graphics.Blit(VuforiaBehaviour.Instance.VideoBackground.VideoBackgroundTexture, rt);

            RenderTexture.active = rt;
            cameraTexture.ReadPixels(new UnityEngine.Rect(0, 0, rt.width, rt.height), 0, 0);
            cameraTexture.Apply();
            RenderTexture.active = null;

            if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
                continue;

            textureFrame.ReadTextureOnCPU(cameraTexture);
            var image = textureFrame.BuildCPUImage();
            textureFrame.Release();

            taskApi.DetectAsync(image, GetCurrentTimestampMillisec());
        }
    }


    private Vector2 ToScreenPoint(Mediapipe.Tasks.Components.Containers.NormalizedLandmark lm)
    {
        float rawX = lm.x;
        float rawY = lm.y;

#if !UNITY_EDITOR
        float tmp = rawX;
        rawX = rawY;
        rawY = tmp;
        rawX = 1.0f - rawX; // flip horizontally
#endif
        rawY = 1.0f - rawY;
        float screenX = rawX * UnityEngine.Screen.width;
        float screenY = rawY * UnityEngine.Screen.height;
        return new Vector2(screenX, screenY);
    }

    private void OnHandLandmarkDetectionOutput(HandLandmarkerResult result, Mediapipe.Image image, long timestamp)
    {
        if (result.handedness == null || result.handedness.Count == 0)
        {
            return;
        }
        Debug.Log("Hand Detected!");

        Vector2[] screenPoints = new Vector2[result.handLandmarks.Count];
        for (int i = 0; i < result.handLandmarks.Count; i++)
        {
            var indexTip = result.handLandmarks[i].landmarks[8];
            screenPoints[i] = ToScreenPoint(indexTip);
        }

        latestFingerScreenPoint = screenPoints;

        _mainThreadActions.Enqueue(() =>
        {
            //CheckFingerTouch(screenPoints);
            fingertipUIButtonSystem.QueueFingerScreenPoints(screenPoints);
        });
    }

    private void CheckFingerTouch(Vector2[] screenPoints)
    {
        if (buttonRectTransform == null)
        {
            Debug.LogWarning("Button RectTransform not assigned.");
            return;
        }

        bool isAnyFingerTouching = false;

        for (int i = 0; i < screenPoints.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(buttonRectTransform, screenPoints[i], uiCamera))
            {
                isAnyFingerTouching = true;
                if (!wasPressing)
                {
                    wasPressing = true;
                    Debug.Log("Finger touching the virtual button!");
                    //playButton.onClick.Invoke();
                }
                break;
            }

        }

        if(!isAnyFingerTouching)
        {
            wasPressing = false;
        }
    }
}
