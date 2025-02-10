using UnityEngine;
using TMPro;

public class BowController : MonoBehaviour
{
    public Transform arrowSpawnPoint;
    public GameObject arrowPrefab;
    public Transform bow;
    private GameObject currentArrow;
    private bool isCharging = false;
    private bool isZooming = false;

    [SerializeField] private float chargePower = 0f;
    [SerializeField] private float maxPower = 50f;
    [SerializeField] private float chargeRate = 20f;
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private TextMeshProUGUI powerText; 

    public Camera mainCamera;
    public float zoomedFOV = 30f;
    public float normalFOV = 60f;
    public float zoomSpeed = 5f;

    void Update()
    {
        HandleInput();
        HandleZoom();
        if (isCharging && currentArrow != null)
        {
            StickArrowToBow();
            UpdatePowerText();
        }
        else
        {
            powerText.text = "";
        }
    }
    void UpdatePowerText()
    {
        powerText.text = $"Power: {Mathf.RoundToInt(chargePower)}";  
    }


    void StickArrowToBow()
    {
        currentArrow.transform.position = arrowSpawnPoint.position;
        currentArrow.transform.rotation = arrowSpawnPoint.rotation * Quaternion.Euler(90, 0, 0);
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
            StartCharging();

        if (Input.GetMouseButton(0))
        {
            ChargeShot();
            AimArrowWithCrosshair();
        }

        if (Input.GetMouseButtonUp(0))
            ReleaseArrow();

        if (Input.GetMouseButtonDown(1))
            isZooming = true;

        if (Input.GetMouseButtonUp(1))
            isZooming = false;
    }

    void AimArrowWithCrosshair()
    {
        if (currentArrow != null)
        {
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector3 aimDirection = ray.direction;
            currentArrow.transform.rotation = Quaternion.LookRotation(aimDirection) * Quaternion.Euler(90, 0, 0);
        }
    }

    void HandleZoom()
    {
        float targetFOV = isZooming ? zoomedFOV : normalFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    void StartCharging()
    {
        isCharging = true;
        chargePower = 0f;

        Quaternion rotation = arrowSpawnPoint.rotation * Quaternion.Euler(90, 0, 0);
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, rotation);
        currentArrow.transform.SetParent(bow);
    }

    void ChargeShot()
    {
        if (isCharging)
        {
            chargePower += chargeRate * Time.deltaTime;
            chargePower = Mathf.Clamp(chargePower, 20, maxPower);
        }
    }

    void ReleaseArrow()
    {
        if (isCharging && currentArrow != null)
        {
            isCharging = false;

            currentArrow.transform.SetParent(null);
            Arrow arrowScript = currentArrow.GetComponent<Arrow>();

            // Accurate shoot direction from the crosshair
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector3 shootDirection = ray.direction;

            arrowScript.LaunchArrow(chargePower, shootDirection);
            cameraSwitcher.OnArrowShot(currentArrow); // Camera switch AFTER shooting

            currentArrow = null;
        }
    }
}
