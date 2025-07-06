using UnityEngine;

public class AnimationTriggerByButton : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float cooldownSeconds = 5.0f;

    private float lastTriggerTime = -Mathf.Infinity;

    public void TriggerWaving()
    {
        if (Time.time >= lastTriggerTime + cooldownSeconds)
        {
            animator.SetTrigger("wavingTrigger");
            lastTriggerTime = Time.time;
        }
        else
        {
            Debug.Log("Waving is on cooldown.");
        }
    }
}
