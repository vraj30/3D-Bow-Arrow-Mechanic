//using UnityEngine;
//using UnityEngine.UI;

//[RequireComponent(typeof(RectTransform))]
//public class UIScaler : MonoBehaviour
//{
//    public Vector2 referenceResolution = new Vector2(1080, 1920); // Portrait 9:16
//    public Vector2 baseMargin = new Vector2(50, 50); // Base margin for reference resolution

//    private RectTransform rectTransform;
//    private Vector2 lastScreenSize;

//    void Start()
//    {
//        rectTransform = GetComponent<RectTransform>();
//        AdjustUI();
//    }

//    void AdjustUI()
//    {
//        Rect cameraRect = AspectRatio.CameraRect;
       

//        // Dynamic Scaling
//        float screenRatio = (float)Screen.height / Screen.width;
//        float referenceRatio = referenceResolution.y / referenceResolution.x;
//        float scaleFactor = screenRatio / referenceRatio;
//        rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

//        //  Dynamic Margin Calculation (relative to screen size)
//        float dynamicMarginX = (baseMargin.x / referenceResolution.x) * Screen.width;
//        float dynamicMarginY = (baseMargin.y / referenceResolution.y) * Screen.height;

//        // 3️⃣ Position Adjustment with Safe Area Consideration
//        float offsetX = (cameraRect.x * Screen.width) + dynamicMarginX;
//        float offsetY = (cameraRect.y * Screen.height) + dynamicMarginY;

//        // Adjust Anchored Position (supports different anchor presets)
//        Vector2 anchoredPos = rectTransform.anchoredPosition;
//        anchoredPos.x = Mathf.Clamp(anchoredPos.x, offsetX - Screen.width / 2, (cameraRect.xMax * Screen.width) - dynamicMarginX - Screen.width / 2);
//        anchoredPos.y = Mathf.Clamp(anchoredPos.y, offsetY - Screen.height / 2, (cameraRect.yMax * Screen.height) - dynamicMarginY - Screen.height / 2);

//        rectTransform.anchoredPosition = anchoredPos;
//    }

//    void Update()
//    {
//        // Optimize: Update only when screen size changes
//        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
//        {
//            AdjustUI();
//            lastScreenSize = new Vector2(Screen.width, Screen.height);
//        }
//    }
//}
