using UnityEngine;
using UnityEngine.UI;
public class Crosshair : MonoBehaviour
{
    public Image crosshair;                
    public Camera playerCamera;            
    public float maxDistance = 100f;   

    void Update()
    {
        UpdateCrosshair();
    }

    void UpdateCrosshair()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

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
