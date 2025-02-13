using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AdvancedCameraScaler : MonoBehaviour
{
    [Header("Aspect Ratio Settings")]
    public float targetAspectRatio = 9f / 16f; // Portrait target ratio

    [Header("Field of View Settings")]
    [SerializeField] private float defaultFOV = 60f;   // Default FOV for standard phones
    [SerializeField] private float maxFOV = 80f;       // Max FOV for tall/narrow screens
    [SerializeField] private float minFOV = 45f;       // Min FOV for wide screens
    [SerializeField] private float smoothTransitionSpeed = 5f; // FOV transition speed

    [Header("Device-Specific Tweaks")]
    [SerializeField] private float tabletFOVBoost = 1.15f;     // FOV boost for tablets
    [SerializeField] private float foldableFOVBoost = 1.1f;    // FOV boost for foldables
    [SerializeField] private float tabletAspectThreshold = 0.75f;  // Aspect ratio threshold for tablets
    [SerializeField] private float foldableAspectThreshold = 1.3f; // Aspect ratio threshold for foldables (Samsung Fold)

    private Camera _camera;
    private float currentAspect;
    private float targetFOV;

    void Start()
    {
        _camera = GetComponent<Camera>();
        currentAspect = GetAspectRatio();
        AdjustCamera();
    }

    void Update()
    {
        if (HasAspectRatioChanged())
        {
            AdjustCamera();
        }

        // Smooth FOV transitions
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * smoothTransitionSpeed);
    }

    private void AdjustCamera()
    {
        float aspectRatio = GetAspectRatio();

        if (IsTablet(aspectRatio))
        {
            // Apply FOV boost for tablets
            targetFOV = Mathf.Clamp(defaultFOV * tabletFOVBoost, minFOV, maxFOV);
        }
        else if (IsFoldable(aspectRatio))
        {
            // Apply FOV boost for foldable devices
            targetFOV = Mathf.Clamp(defaultFOV * foldableFOVBoost, minFOV, maxFOV);
        }
        else if (aspectRatio < targetAspectRatio)
        {
            // For narrower screens, increase FOV proportionally
            float scaleFactor = targetAspectRatio / aspectRatio;
            targetFOV = Mathf.Clamp(defaultFOV * scaleFactor, minFOV, maxFOV);
        }
        else
        {
            // For wider screens, keep default FOV within limits
            targetFOV = Mathf.Clamp(defaultFOV, minFOV, maxFOV);
        }

        currentAspect = aspectRatio;
    }

    private bool HasAspectRatioChanged()
    {
        return Mathf.Abs(currentAspect - GetAspectRatio()) > 0.01f; // Detect changes
    }

    private float GetAspectRatio()
    {
        return (float)Screen.width / Screen.height;
    }

    // Device Profile Detection
    private bool IsTablet(float aspectRatio)
    {
        return aspectRatio > tabletAspectThreshold && aspectRatio <= foldableAspectThreshold;
    }

    private bool IsFoldable(float aspectRatio)
    {
        return aspectRatio > foldableAspectThreshold; // Samsung Fold in unfolded mode
    }

}
