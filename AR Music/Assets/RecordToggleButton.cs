using UnityEngine;
using UnityEngine.UI;

public class RecordToggleButton : MonoBehaviour
{
    [Header("UI ����")]
    [Tooltip("����ͬһ�� GameObject �ϵ� Button")]
    [SerializeField] private Button toggleButton;
    [Tooltip("�л�ͼ���õ� Image ���")]
    [SerializeField] private Image iconImage;

    [SerializeField] private UnityAndGeminiV3 AI;

    [Header("ͼ����Դ")]
    [Tooltip("��ʼ¼��ʱ��ʾ��ͼ��")]
    [SerializeField] private Sprite startIcon;
    [Tooltip("����¼��ʱ��ʾ��ͼ��")]
    [SerializeField] private Sprite stopIcon;

    private bool isRecording = false;

    void Awake()
    {
        // 1. �� Button ���һ���ص��������߼����ŵ�ͬһ��������
        toggleButton.onClick.AddListener(OnToggleButtonClicked);

        // 2. ��ʼʱ��ʾ����ʼ¼����ͼ��
        iconImage.sprite = startIcon;
    }

    void OnDestroy()
    {
        toggleButton.onClick.RemoveListener(OnToggleButtonClicked);
    }

    private void OnToggleButtonClicked()
    {
        if (!isRecording)
        {
            // --- �е���¼���С�״̬ ---
            isRecording = true;

            // ִ�п�ʼ¼�����߼�
            AI.StartVoiceInput();


            // �л�ͼ�굽������˵����
            iconImage.sprite = stopIcon;
        }
        else
        {
            // --- �е�����ֹͣ��״̬ ---
            isRecording = false;

            // ִ��ֹͣ¼�������͵��߼�
            AI.StopVoiceInput();

            // �л�ͼ�굽����ʼ¼����
            iconImage.sprite = startIcon;
        }
    }

}
