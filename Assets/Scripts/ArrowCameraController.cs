using Unity.Cinemachine;
using UnityEngine;


public class ArrowCameraController : MonoBehaviour
{
    public CinemachineCamera virtualCamera; 
    private bool isFollowing = false;

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) 
        {
            GameObject arrow = GameObject.FindWithTag("Arrow"); 
            if (arrow != null && !isFollowing)
            {
                virtualCamera.Follow = arrow.transform;
                virtualCamera.LookAt = arrow.transform;
                isFollowing = true;
            }
        }

        // Stop following when the arrow hits something
        if (isFollowing && virtualCamera.Follow == null)
        {
            isFollowing = false;
        }
    }
}
