using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI windText;
    [SerializeField] private RectTransform windArrowUI;
    [SerializeField] private Image windArrowImage; // Reference to the UI arrow image

    [SerializeField] private GameObject rightWindEffect; // Effect for right wind
    [SerializeField] private GameObject leftWindEffect;  // Effect for left wind

    public float fixedWindIntensity = 10f; // Set a constant wind intensity
    private float windAngle; // Fixed wind direction

    [SerializeField] private Color lowWindColor = Color.green;
    [SerializeField] private Color midWindColor = Color.yellow;
    [SerializeField] private Color highWindColor = Color.red;

    void Start()
    {
        GenerateFixedWind();
    }
    void GenerateFixedWind()
    {
        // Wind is mostly left or right
        bool isRightWind = Random.value > 0.5f;
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
        windArrowUI.rotation = Quaternion.Euler(0, 0, -windAngle); // Rotate UI arrow to match wind direction
        windText.text = $"{fixedWindIntensity / 60} m/s";
        
        // Change arrow color based on wind intensity
        if (fixedWindIntensity < 300f)
            windArrowImage.color = lowWindColor;
        else if (fixedWindIntensity < 650f)
            windArrowImage.color = midWindColor;
        else
            windArrowImage.color = highWindColor;
    }
}
