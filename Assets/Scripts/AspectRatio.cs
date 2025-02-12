using UnityEngine;
using System; // For Action delegate

[RequireComponent(typeof(Camera))]
public class AspectRatio : MonoBehaviour
{
    public float targetAspectRatio = 9f / 16f; // Portrait mode aspect ratio
    private Camera _camera;
    public static Rect CameraRect { get; private set; }  // Expose for UI adjustments

    //  Event to notify UI elements when the aspect changes
    public static event Action OnAspectRatioChanged;

    void Start()
    {
        _camera = GetComponent<Camera>();
        SetCameraAspect();
    }

    void SetCameraAspect()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspectRatio;

        Rect rect;

        if (scaleHeight < 1.0f)
        {
            // Letterboxing
            rect = new Rect(0, (1.0f - scaleHeight) / 2.0f, 1.0f, scaleHeight);
        }
        else
        {
            // Pillarboxing
            float scaleWidth = 1.0f / scaleHeight;
            rect = new Rect((1.0f - scaleWidth) / 2.0f, 0, scaleWidth, 1.0f);
        }

        _camera.rect = rect;
        CameraRect = rect;

        OnAspectRatioChanged?.Invoke(); // 🔔 Notify UI elements
    }

    private void Update()
    {
        SetCameraAspect();
    }
}
