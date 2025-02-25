using UnityEngine;
using TMPro;

public class BowController : MonoBehaviour
{
    public float ChargePower => chargePower; 
    public float MaxPower => maxPower;       

    public Transform arrowSpawnPoint;
    public GameObject arrowPrefab;
    public Transform bow;
    private GameObject currentArrow;
    private bool isCharging = false;

    [SerializeField] private float chargePower = 0f;
    [SerializeField] private float maxPower = 50f;
    [SerializeField] private float chargeRate = 20f;
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private ChargeController chargeBarController;
    [SerializeField] private float dizzyIntensity = 0.1f; 
    [SerializeField] private float dizzySpeed = 2f;       

    public Camera mainCamera;
    public float zoomedFOV = 30f;
    public float normalFOV = 60f;
    public float zoomSpeed = 5f;

    private Vector2 startTouchPosition;

    private bool isDizzy = false;
    private float dizzyTimer = 0f;
    private Vector3 originalCameraPosition;

    void Start()
    {
        originalCameraPosition = mainCamera.transform.localPosition;
    }


    void Update()
    {
        HandleInput();
        HandleZoom();

        if (isCharging && currentArrow != null)
        {
            StickArrowToBow();
            UpdatePowerText();
            ApplyDizzyEffect();
        }
        else
        {
            powerText.text = "Power: ";
            ResetCameraPosition();

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
        if (Application.isMobilePlatform)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
            StartCharging();

        if (Input.GetMouseButton(0))
        {
            ChargeShot();
            AimArrowWithCrosshair(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
            ReleaseArrow();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    StartCharging();
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    ChargeShot();
                    AimArrowWithCrosshair(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    ReleaseArrow();
                    break;
            }
        }
    }

    void AimArrowWithCrosshair(Vector2 screenPosition)
    {
        if (currentArrow != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            Vector3 aimDirection = ray.direction;
            currentArrow.transform.rotation = Quaternion.LookRotation(aimDirection) * Quaternion.Euler(90, 0, 0);
        }
    }

    void HandleZoom()
    {
        float targetFOV = isCharging ? zoomedFOV : normalFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    void StartCharging()
    {
        isCharging = true;
        chargePower = 0f;

        Quaternion rotation = arrowSpawnPoint.rotation * Quaternion.Euler(90, 0, 0);
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, rotation);
        currentArrow.transform.SetParent(bow);

        isDizzy = true;
        dizzyTimer = 0f;
    }

    void ChargeShot()
    {
        if (isCharging)
        {
            chargePower += chargeRate * Time.deltaTime;
            chargePower = Mathf.Clamp(chargePower, 10, maxPower);
        }
    }

    void ReleaseArrow()
    {
        if (isCharging && currentArrow != null)
        {
            isCharging = false;
            isDizzy = false; // Stop dizzy effect

            currentArrow.transform.SetParent(null);
            Arrow arrowScript = currentArrow.GetComponent<Arrow>();

            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            Vector3 shootDirection;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                shootDirection = (hit.point - arrowSpawnPoint.position).normalized;
            }
            else
            {
                shootDirection = ray.direction;
            }

            arrowScript.LaunchArrow(chargePower, shootDirection);
            cameraSwitcher.OnArrowShot(currentArrow);

            currentArrow = null;
            chargePower = 0f;
            if (chargeBarController != null)
                chargeBarController.ResetChargeBar();

            ResetCameraPosition(); // Reset camera after release
        }
    }

    void ApplyDizzyEffect()
    {
        if (!isDizzy) return;

        dizzyTimer += Time.deltaTime * dizzySpeed;

        float offsetX = Mathf.PerlinNoise(dizzyTimer, 0) * dizzyIntensity - (dizzyIntensity / 2);
        float offsetY = Mathf.PerlinNoise(0, dizzyTimer) * dizzyIntensity - (dizzyIntensity / 2);

        mainCamera.transform.localPosition = originalCameraPosition + new Vector3(offsetX, offsetY, 0);
    }
    void ResetCameraPosition()
    {
        mainCamera.transform.localPosition = originalCameraPosition;
        isDizzy = false;
    }
}
