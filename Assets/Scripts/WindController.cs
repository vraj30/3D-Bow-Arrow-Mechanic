using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI windText;
    [SerializeField] private RectTransform windArrowUI;
    [SerializeField] private RectTransform windArrowImage; 

    [SerializeField] private GameObject rightWindEffect; 
    [SerializeField] private GameObject leftWindEffect; 

    [SerializeField] private bool isRightWind = true; 
    public float fixedWindIntensity = 10f; 
    private float windAngle; 

    [SerializeField] private Color lowWindColor = Color.green;
    [SerializeField] private Color midWindColor = Color.yellow;
    [SerializeField] private Color highWindColor = Color.red;

    void Start()
    {
        SetWindDirection();
    }

    void SetWindDirection()
    {
        // Set wind angle based on the boolean value
        windAngle = isRightWind ? 0f : 180f;

        // Convert angle to direction vector
        Arrow.WindDirection = new Vector3(Mathf.Cos(windAngle * Mathf.Deg2Rad), 0, Mathf.Sin(windAngle * Mathf.Deg2Rad));
        Arrow.WindIntensity = fixedWindIntensity;

        // Activate the appropriate effect
        rightWindEffect.SetActive(isRightWind);
        leftWindEffect.SetActive(!isRightWind);

        UpdateWindUI();
    }

    void UpdateWindUI()
    {
        windArrowUI.rotation = Quaternion.Euler(0, 0, isRightWind ? 0 : 180);

        // Fix position
        Vector2 size = windArrowUI.rect.size;
        windArrowUI.anchoredPosition += new Vector2(isRightWind ? 0 : -size.x, isRightWind ? 0 : size.y);

        windText.text = $"{fixedWindIntensity / 60} m/s";

        // Change arrow color based on wind intensity
        if (fixedWindIntensity < 450f)
            windArrowImage.GetComponentInChildren<Image>().color = lowWindColor;
        else if (fixedWindIntensity < 900f)
            windArrowImage.GetComponentInChildren<Image>().color = midWindColor;
        else
            windArrowImage.GetComponentInChildren<Image>().color = highWindColor;
    }
}
