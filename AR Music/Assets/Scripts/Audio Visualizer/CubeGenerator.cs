using UnityEngine;
using Vuforia;

public class OnImageTargetGenerateStage : MonoBehaviour
{
    public GameObject stagePrefab;
    public GameObject cubePrefab;
    public int cubeCount = 64;
    public float radius = 0.15f;

    private ObserverBehaviour observer;
    private bool hasGenerated = false;

    // 新增：用于控制 Cube 群体旋转
    private GameObject cubeGroup;

    void Start()
    {
        observer = GetComponent<ObserverBehaviour>();
        if (observer)
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnDestroy()
    {
        if (observer != null)
        {
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (!hasGenerated && status.Status == Status.TRACKED)
        {
            hasGenerated = true;
            GenerateStageAndCubes();
        }
    }

    void GenerateStageAndCubes()
    {
        // 保持舞台不动，作为中心
        GameObject stage = Instantiate(stagePrefab, transform.position, Quaternion.identity, transform);

        // 新增：生成 Cube Group 空对象，用于绕舞台旋转
        cubeGroup = new GameObject("CubeGroup");
        cubeGroup.transform.parent = transform;
        cubeGroup.transform.localPosition = Vector3.zero;

        for (int i = 0; i < cubeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / cubeCount;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Vector3 cubePos = transform.position + offset;

            GameObject cube = Instantiate(cubePrefab, cubePos, Quaternion.identity, cubeGroup.transform);
            cube.transform.LookAt(stage.transform.position);

            ParamCube pc = cube.GetComponent<ParamCube>();
            if (pc != null)
                pc._band = i;
        }
    }

    void Update()
    {
        // 让 cubeGroup 旋转，实现环绕舞台运动
        if (cubeGroup != null)
        {
            cubeGroup.transform.Rotate(Vector3.up * 20f * Time.deltaTime, Space.Self);
        }
    }
}
