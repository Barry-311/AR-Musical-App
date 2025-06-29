using UnityEngine;

[ExecuteAlways]
public class SpinWithPivotMode : MonoBehaviour
{
    public enum PivotMode { Pivot, Center }
    public enum SpinAxis { X, Y, Z }

    [Header("����")]
    public bool enableRotation = false;

    [Header("��תģʽ")]
    public PivotMode pivotMode = PivotMode.Center;

    [Header("��ת��")]
    public SpinAxis spinAxis = SpinAxis.Z;

    [Header("���� (��/��)��ֻ��ѡ��������Ч")]
    public float speed = 90f;

    // Center ģʽ�»��������ռ伸������
    private Vector3 centerPoint;

    void Start()
    {
        UpdateCenterPoint();
    }

    void UpdateCenterPoint()
    {
        var rends = GetComponentsInChildren<Renderer>();
        if (rends.Length > 0)
        {
            Bounds b = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++)
                b.Encapsulate(rends[i].bounds);
            centerPoint = b.center;
        }
        else centerPoint = transform.position;
    }

    void Update()
    {
        if (!enableRotation) return;

        float dt = Time.deltaTime;
        if (pivotMode == PivotMode.Center)
            UpdateCenterPoint();

        Vector3 pivot = (pivotMode == PivotMode.Center)
            ? centerPoint
            : transform.position;

        // ����ö��ֻ�Ե�һ����Ч
        float angle = speed * dt;
        switch (spinAxis)
        {
            case SpinAxis.X:
                transform.RotateAround(pivot, transform.right, angle);
                break;
            case SpinAxis.Y:
                transform.RotateAround(pivot, transform.up, angle);
                break;
            case SpinAxis.Z:
                transform.RotateAround(pivot, transform.forward, angle);
                break;
        }
    }
}
