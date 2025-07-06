using UnityEngine;
using UnityEngine.UI;

public class ButtonClickLogger : MonoBehaviour
{
    private void Start()
    {
        // 获取 Button 组件
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            // 注册点击事件监听器
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
