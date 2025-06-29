using UnityEngine;
using System.Collections;

public class HoloSphereIntro : MonoBehaviour
{
    [Header("����ʱ�䣨�룩")]
    public float scaleDuration = 1.5f;
    [Header("��ת���ĽǶȣ��ȣ�")]
    public float targetYAngle = 135f;
    [Header("��תʱ�䣨�룩")]
    public float rotateDuration = 2f;

    // ��Χ�еײ����ģ��������꣩
    private Vector3 bottomCenterWS;

    void Start()
    {
        // 1) ������������ Renderer �������Χ��
        var rends = GetComponentsInChildren<Renderer>();
        if (rends.Length == 0)
        {
            Debug.LogWarning("û���ҵ��κ��� Renderer�������㽫ʹ������λ�á�");
            bottomCenterWS = transform.position;
        }
        else
        {
            var b = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++)
                b.Encapsulate(rends[i].bounds);
            // ��Χ�е������� = b.center ���� b.extents.y
            bottomCenterWS = b.center - Vector3.up * b.extents.y;
        }

        // һ��ʼ���� 0
        transform.localScale = Vector3.zero;
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // 2) ���Ŷ���
        float t = 0f;
        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            float f = Mathf.SmoothStep(0f, 1f, t / scaleDuration);
            ScaleAroundPoint(f, bottomCenterWS);
            yield return null;
        }
        ScaleAroundPoint(1f, bottomCenterWS);

        // 3) ��ת����
        float elapsed = 0f;
        float startY = transform.eulerAngles.y;
        while (elapsed < rotateDuration)
        {
            elapsed += Time.deltaTime;
            float f = Mathf.SmoothStep(0f, 1f, elapsed / rotateDuration);
            float currentY = Mathf.LerpAngle(startY, startY + targetYAngle, f);
            RotateAroundPoint(Vector3.up, currentY - transform.eulerAngles.y);
            yield return null;
        }
        // ��֤ͣ�ھ�ȷֵ
        RotateAroundPoint(Vector3.up, (startY + targetYAngle) - transform.eulerAngles.y);
    }

    // �� transform.localScale ��Ϊ (s,s,s)��ͬʱ���� position ʹ point ����
    void ScaleAroundPoint(float s, Vector3 pointWS)
    {
        Vector3 oldScale = transform.localScale;
        Vector3 newScale = Vector3.one * s;

        // �������ǰλ�õ�ê�������
        Vector3 dir = transform.position - pointWS;
        // ���Ÿ�����
        Vector3 scaledDir = new Vector3(
            dir.x * (newScale.x / (oldScale.x > 0 ? oldScale.x : 1)),
            dir.y * (newScale.y / (oldScale.y > 0 ? oldScale.y : 1)),
            dir.z * (newScale.z / (oldScale.z > 0 ? oldScale.z : 1))
        );
        transform.position = pointWS + scaledDir;
        transform.localScale = newScale;
    }

    // ��ê�� pointWS ��ת deltaAngle �ȣ����� transform �ᣩ
    void RotateAroundPoint(Vector3 axisLocal, float deltaAngle)
    {
        transform.RotateAround(bottomCenterWS, transform.TransformDirection(axisLocal), deltaAngle);
    }
}