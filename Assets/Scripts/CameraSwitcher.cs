using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using static UnityEditor.PlayerSettings;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;                       
    public Camera mainCamera2;                      
    public CinemachineCamera arrowCamera;           
    public Camera impactCamera;
    public GameObject crosshair;
    public GameObject chargeBar;

    private bool isArrowFlying = false;
    private GameObject currentArrow;

    private Vector3 initialArrowCamPosition;
    private Quaternion initialArrowCamRotation;
    private Vector3 initialMainCamPosition;
    private Quaternion initialMainCamRotation;

    

    private void Start()
    {
        initialArrowCamPosition = arrowCamera.transform.position;
        initialArrowCamRotation = arrowCamera.transform.rotation;

        initialMainCamPosition = mainCamera2.transform.position;
        initialMainCamRotation = mainCamera2.transform.rotation;

      
    }

    public void OnArrowShot(GameObject arrow)
    {
        if (arrow == null) return;
        currentArrow = arrow;

        Arrow arrowCollision = arrow.GetComponent<Arrow>();
        if (arrowCollision != null)
        {
            arrowCollision.SetCameraSwitcher(this);
        }

        mainCamera.gameObject.SetActive(false);
        arrowCamera.gameObject.SetActive(true);
        mainCamera2.gameObject.SetActive(true);
        crosshair.gameObject.SetActive(false);
        chargeBar.gameObject.SetActive(false);

        StartCoroutine(AssignCameraAfterDelay(arrow));
    }

    IEnumerator AssignCameraAfterDelay(GameObject arrow)
    {
        yield return new WaitForSeconds(0.05f);

        if (arrow != null)
        {
            Vector3 cameraOffset = new Vector3(0, 0, -3);
            arrowCamera.transform.position = arrow.transform.position + cameraOffset;

            GameObject lookAtPoint = new GameObject("LookAheadPoint");
            lookAtPoint.transform.position = arrow.transform.position + arrow.transform.forward * 5f;
            lookAtPoint.transform.SetParent(arrow.transform);

            arrowCamera.Follow = arrow.transform;

            var followComponent = arrowCamera.GetComponent<CinemachineFollow>();
            if (followComponent != null)
            {
                followComponent.FollowOffset = new Vector3(3f, 0f, 0f);
            }

            isArrowFlying = true;
        }
    }

    private void Update()
    {
        if (isArrowFlying)
        {
            if (currentArrow == null)
            {
                ReturnToPlayerView();
            }
            else
            {
                Rigidbody arrowRb = currentArrow.GetComponent<Rigidbody>();

                if (arrowRb.linearVelocity.magnitude < 0.1f)
                {
                    StartCoroutine(ShowImpactCamera(currentArrow.transform.position));
                    isArrowFlying = false;
                }
            }
        }
    }

    IEnumerator ShowImpactCamera(Vector3 impactPosition)
    {
        arrowCamera.gameObject.SetActive(false);
        mainCamera2.gameObject.SetActive(false);

        Vector3 startPos = arrowCamera.transform.position + new Vector3(0,0,1); // Starting position (current camera)
       // Quaternion startRot = arrowCamera.transform.rotation;

        Vector3 targetPos = impactPosition + new Vector3(0, 1f, -2); // Impact view position
        // targetRot = Quaternion.LookRotation(impactPosition - targetPos); // Look at impact

        float duration = 0.5f; // Smooth transition duration
        float elapsedTime = 0f;

        impactCamera.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            // Smooth transition
            impactCamera.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
           // impactCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / duration);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Final position & rotation (ensure precision)
        impactCamera.transform.position = targetPos;
        
        //impactCamera.transform.rotation = targetRot;

        yield return new WaitForSeconds(1.5f); // Stay on impact for 1.5 seconds

        impactCamera.gameObject.SetActive(false);
        ReturnToPlayerView();
    }


    public void ReturnToPlayerView(System.Action onComplete = null)
    {
        mainCamera.gameObject.SetActive(true);
        arrowCamera.gameObject.SetActive(false);
        mainCamera2.gameObject.SetActive(false);

        StartCoroutine(SmoothResetCamera(() =>
        {
            crosshair.gameObject.SetActive(true);
            chargeBar.gameObject.SetActive(true);
            onComplete?.Invoke();
        }));

        arrowCamera.Follow = null;
        isArrowFlying = false;
        currentArrow = null;
    }

    IEnumerator SmoothResetCamera(System.Action onComplete)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;

        Vector3 startPos = arrowCamera.transform.position;
        Quaternion startRot = arrowCamera.transform.rotation;
        Vector3 startPos1 = mainCamera2.transform.position;
        Quaternion startRot1 = mainCamera2.transform.rotation;

        while (elapsedTime < duration)
        {
            arrowCamera.transform.position = Vector3.Lerp(startPos, initialArrowCamPosition, elapsedTime / duration);
            arrowCamera.transform.rotation = Quaternion.Slerp(startRot, initialArrowCamRotation, elapsedTime / duration);

            mainCamera2.transform.position = Vector3.Lerp(startPos1, initialMainCamPosition, elapsedTime / duration);
            mainCamera2.transform.rotation = Quaternion.Slerp(startRot1, initialMainCamRotation, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        arrowCamera.transform.position = initialArrowCamPosition;
        arrowCamera.transform.rotation = initialArrowCamRotation;

        mainCamera2.transform.position = initialMainCamPosition;
        mainCamera2.transform.rotation = initialMainCamRotation;

        onComplete?.Invoke();
    }
}
