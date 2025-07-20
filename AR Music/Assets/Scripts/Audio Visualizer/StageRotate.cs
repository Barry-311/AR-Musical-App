using UnityEngine;

public class OrbitAroundCenter : MonoBehaviour
{
    public Transform centerPoint;
    public float rotationSpeed = 20f;

    void Update()
    {
        if (centerPoint != null)
        {
            transform.RotateAround(centerPoint.position, Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}
