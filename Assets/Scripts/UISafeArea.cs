using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]

public class UISafeArea : MonoBehaviour
{
    //private RectTransform rectTransform;

    //void Start()
    //{
    //    rectTransform = GetComponent<RectTransform>();
    //    AdjustForSafeArea();
    //}

    //void AdjustForSafeArea()
    //{
    //    Rect safeArea = AspectRatio.CameraRect;

    //    // Convert safe area to anchor values
    //    rectTransform.anchorMin = new Vector2(safeArea.xMin, safeArea.yMin);
    //    rectTransform.anchorMax = new Vector2(safeArea.xMax, safeArea.yMax);

    //    // Optional: Add padding if needed
    //    rectTransform.offsetMin = new Vector2(10, 10); // Left & Bottom Padding
    //    rectTransform.offsetMax = new Vector2(-10, -10); // Right & Top Padding
    //}

    //void Update()
    //{
    //    // Adjust dynamically if the screen size changes
    //    AdjustForSafeArea();
    //}

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        AspectRatio.OnAspectRatioChanged += AdjustUI; // Subscribe to aspect ratio changes
        AdjustUI(); // Adjust immediately on start
    }

    void AdjustUI()
    {
        Rect cameraRect = AspectRatio.CameraRect;

        // Convert normalized viewport rect to screen space
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float leftPadding = cameraRect.xMin * screenWidth;
        float rightPadding = (1f - cameraRect.xMax) * screenWidth;
        float bottomPadding = cameraRect.yMin * screenHeight;
        float topPadding = (1f - cameraRect.yMax) * screenHeight;

        // Apply safe area padding
        rectTransform.offsetMin = new Vector2(leftPadding + 40f, bottomPadding + 80f); // Bottom-left padding
        rectTransform.offsetMax = new Vector2(-rightPadding - 20f, -topPadding - 20f); // Top-right padding
    }

    private void OnDestroy()
    {
        AspectRatio.OnAspectRatioChanged -= AdjustUI; // Unsubscribe to avoid memory leaks
    }
}
