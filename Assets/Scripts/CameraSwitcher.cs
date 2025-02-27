using UnityEngine;
using System.Collections;
using Unity.Cinemachine;


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
    private Vector3 initialMainCam2Position;
    private Quaternion initialMainCam2Rotation;

    private float initialPan;
    private float initialTilt;
    private CinemachinePanTilt panTilt;



    private void Start()
    {
        panTilt = arrowCamera.GetComponent<CinemachinePanTilt>();

        if (panTilt != null)
        {
            initialPan = panTilt.PanAxis.Value;
            initialTilt = panTilt.TiltAxis.Value;
        }

        initialArrowCamPosition = arrowCamera.transform.position;
        initialArrowCamRotation = arrowCamera.transform.rotation;

        
        initialMainCamPosition = mainCamera.transform.position;
        initialMainCamRotation = mainCamera.transform.rotation;

        initialMainCam2Position = mainCamera2.transform.position;
        initialMainCam2Rotation = mainCamera2.transform.rotation;
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

        arrowCamera.gameObject.SetActive(true);
        mainCamera2.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        crosshair.gameObject.SetActive(false);
        chargeBar.gameObject.SetActive(false);
        SetPanTilt(7.53f, 10f, 1f); // Smoothly pan to 30°, tilt to 15° over 1.5s

        StartCoroutine(AssignCameraAfterDelay(arrow));
    }

    IEnumerator AssignCameraAfterDelay(GameObject arrow)
    {
        yield return new WaitForSeconds(0.05f);

        if (arrow != null)
        {
            Vector3 cameraOffset = new Vector3(0, 0, 5);
            arrowCamera.transform.position = arrow.transform.position + cameraOffset;

            GameObject lookAtPoint = new GameObject("LookAheadPoint");
            lookAtPoint.transform.position = arrow.transform.position + arrow.transform.forward * 5f;
            lookAtPoint.transform.SetParent(arrow.transform);

            arrowCamera.Follow = arrow.transform;

            var followComponent = arrowCamera.GetComponent<CinemachineFollow>();
            if (followComponent != null)
            {
                followComponent.FollowOffset = new Vector3(3f, 0f, 3f);
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

        Vector3 startPos = arrowCamera.transform.position + new Vector3(0, 0, 1);  // Starting position (current camera)
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

        mainCamera.transform.position = initialMainCamPosition;
        mainCamera.transform.rotation = initialMainCamRotation;

        StartCoroutine(SmoothResetCamera(() =>
        {
            crosshair.gameObject.SetActive(true);
            chargeBar.gameObject.SetActive(true);
            onComplete?.Invoke();
        }));
        // Reset Pan & Tilt smoothly
        if (panTilt != null)
        {
            StartCoroutine(SmoothPanTilt(panTilt, initialPan, initialTilt, 0.1f));
        }
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

            mainCamera2.transform.position = Vector3.Lerp(startPos1, initialMainCam2Position, elapsedTime / duration);
            mainCamera2.transform.rotation = Quaternion.Slerp(startRot1, initialMainCam2Rotation, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        arrowCamera.transform.position = initialArrowCamPosition;
        arrowCamera.transform.rotation = initialArrowCamRotation;

        mainCamera2.transform.position = initialMainCam2Position;
        mainCamera2.transform.rotation = initialMainCam2Rotation;

        onComplete?.Invoke();
    }
    IEnumerator SmoothPanTilt(CinemachinePanTilt panTilt, float targetPan, float targetTilt, float duration)
    {
        float elapsedTime = 0f;
        float startPan = panTilt.PanAxis.Value;
        float startTilt = panTilt.TiltAxis.Value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            panTilt.PanAxis.Value = Mathf.Lerp(startPan, targetPan, t);
            panTilt.TiltAxis.Value = Mathf.Lerp(startTilt, targetTilt, t);

            yield return null;
        }

        // Ensure final values are set correctly
        panTilt.PanAxis.Value = targetPan;
        panTilt.TiltAxis.Value = targetTilt;
    }

    public void SetPanTilt(float pan, float tilt, float transitionTime)
    {
        if (panTilt != null)
        {
            StartCoroutine(SmoothPanTilt(panTilt, pan, tilt, transitionTime));
        }
    }



}
