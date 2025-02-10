using UnityEngine;
using UnityEngine.UI;
public class Crosshair : MonoBehaviour
{
    public Image crosshair;                // Assign the UI crosshair in Inspector
    public Camera playerCamera;            // Assign the player's camera
    public float maxDistance = 100f;       // Maximum range to detect hits

    void Update()
    {
        UpdateCrosshair();
    }

    void UpdateCrosshair()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            crosshair.rectTransform.position = playerCamera.WorldToScreenPoint(hit.point);
        }
        else
        {
            // Keep crosshair at center when not hitting anything
            crosshair.rectTransform.position = new Vector2(Screen.width / 2, Screen.height / 2);
        }
    }
}
