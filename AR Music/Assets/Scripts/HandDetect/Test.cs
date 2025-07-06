using UnityEngine;
using UnityEngine.UI;

public class ButtonClickLogger : MonoBehaviour
{
    private void Start()
    {
        // ��ȡ Button ���
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            // ע�����¼�������
            btn.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogWarning("Button component not found on this GameObject.");
        }
    }

    private void OnButtonClicked()
    {
        Debug.Log($"Button [{gameObject.name}] was clicked!");
    }
}
