using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    public Transform pointA, pointB;
    public float speed = 3f;
    private Transform targetPoint;

    void Start()
    {
        targetPoint = pointA;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            SwitchTarget();
    }

    void SwitchTarget()
    {
        targetPoint = (targetPoint == pointA) ? pointB : pointA;
    }
}
