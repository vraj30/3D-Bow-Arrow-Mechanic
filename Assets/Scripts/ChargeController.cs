using UnityEngine;
using UnityEngine.UI;

public class ChargeController : MonoBehaviour
{
    [SerializeField] private Image chargeBarFill;
    [SerializeField] private BowController bowController;

    // Define the color gradient (Green → Yellow → Red)
    [SerializeField] private Color lowPowerColor = Color.green;
    [SerializeField] private Color midPowerColor = Color.yellow;
    [SerializeField] private Color highPowerColor = Color.red;

    private void Update()
    {
        UpdateChargeBar();
    }

    void UpdateChargeBar()
    {
        if (bowController != null && chargeBarFill != null)
        {
            float fillAmount = Mathf.InverseLerp(0, bowController.MaxPower, bowController.ChargePower);
            chargeBarFill.fillAmount = fillAmount;

            //Change color based on fill amount
            chargeBarFill.color = GetChargeColor(fillAmount);
        }
    }

    // Interpolates color based on charge level
    private Color GetChargeColor(float fillAmount)
    {
        if (fillAmount < 0.5f)
        {
            // Green to Yellow for the first 50%
            return Color.Lerp(lowPowerColor, midPowerColor, fillAmount * 2f);
        }
        else
        {
            // Yellow to Red for the next 50%
            return Color.Lerp(midPowerColor, highPowerColor, (fillAmount - 0.5f) * 2f);
        }
    }

    public void ResetChargeBar()
    {
        if (chargeBarFill != null)
        {
            chargeBarFill.fillAmount = 0f;
            chargeBarFill.color = lowPowerColor; // Reset color to green
        }
    }
}
