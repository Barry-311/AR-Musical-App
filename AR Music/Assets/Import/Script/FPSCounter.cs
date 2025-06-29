using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI fpsText;
    private Canvas fpsCanvas;
    private float deltaTime;

    void Awake()
    {
        CreateFPSCanvas();
    }

    void CreateFPSCanvas()
    {
        // ����Canvas
        GameObject canvasObj = new GameObject("FPSCanvas");
        canvasObj.transform.SetParent(this.transform); // �����������
        fpsCanvas = canvasObj.AddComponent<Canvas>();
        fpsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fpsCanvas.sortingOrder = 1000; // ��֤�����ϲ�

        // ���Canvas Scaler
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // ���GraphicRaycaster�����ܲ���Ҫ����Unity�Ƽ����ϣ�
        canvasObj.AddComponent<GraphicRaycaster>();

        // ����FPS Text
        GameObject textObj = new GameObject("FPSText");
        textObj.transform.SetParent(canvasObj.transform);

        fpsText = textObj.AddComponent<TextMeshProUGUI>();

        // ���������С����ɫ�����뷽ʽ
        fpsText.fontSize = 48;
        fpsText.color = Color.green;
        fpsText.alignment = TextAlignmentOptions.TopRight;

        // ����RectTransform��λ�����Ͻǣ�
        RectTransform rectTransform = fpsText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(-50, -70);
        rectTransform.sizeDelta = new Vector2(200, 50);
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
    }
}
