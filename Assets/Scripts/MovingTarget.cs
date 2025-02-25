using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frames
    void Update()
    {
        transform.position = new Vector3(startPosition.x + Mathf.PingPong(Time.time, 3), startPosition.y, startPosition.z);
    }
}
