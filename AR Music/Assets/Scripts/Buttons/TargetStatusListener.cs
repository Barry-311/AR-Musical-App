using UnityEngine;
using Vuforia;

[RequireComponent(typeof(ObserverBehaviour))]
public class ShowOnTargetFound : MonoBehaviour
{
    [SerializeField] private FingertipUIButtonSystem handDetectionSys;
    private ObserverBehaviour observer;
    private Canvas canvas;

    void Awake()
    {
        observer = GetComponent<ObserverBehaviour>();
        observer.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnDestroy()
    {
        observer.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    private void OnTargetStatusChanged(ObserverBehaviour obs, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            Canvas obsCanvas = obs.GetComponentInChildren<Canvas>(true);
            if (canvas != obsCanvas)
            {
                canvas = obsCanvas;
                handDetectionSys.RefreshCanvasButtons(canvas);
            }
        }
    }
}
