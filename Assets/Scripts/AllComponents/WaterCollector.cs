using UnityEngine;

public class WaterCollector : MonoBehaviour
{

    [SerializeField] WaterSliderUI waterSliderUI;
    [SerializeField] Transform waterParent;       // Increase height of this as the bucket fills
    [SerializeField] GameObject waterPipeEffect;  // Enable it to form foam in the middle of the bucket when the tap is on
    Transform waterVisual;                        // Increase diameter of this as the bucket fills

    int waterCount = 0;
    int maxWaterCount;

    float emptyWaterLocalY = -0.065f;
    float filledWaterLocalY = 0.032f;

    float emptyWaterLocalDiameter = 0.07f;
    float filledWaterLocalDiameter = 0.083f;

    Tap tap;
    bool switchedOffTap = false;

    void Start()
    {
        waterVisual = transform;

        maxWaterCount = waterSliderUI.waterPerCollector;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("TapIndicator"))
        {
            waterPipeEffect.SetActive(true);
            //waterPipeEffect.transform.position = new Vector3(col.transform.position.x, waterPipeEffect.transform.position.y, col.transform.position.z);

            tap = col.transform.parent.GetComponent<Tap>();
            tap.tapParticleEffectHandler.OnTapSwitchOff += TapParticleEffectHandler_OnTapSwitchOff;
        }
    }

    void TapParticleEffectHandler_OnTapSwitchOff()
    {
        if (tap != null)
        {
            waterPipeEffect.SetActive(false);
            tap.tapParticleEffectHandler.OnTapSwitchOff -= TapParticleEffectHandler_OnTapSwitchOff;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("TapIndicator"))
        {
            waterPipeEffect.SetActive(false);
            tap.tapParticleEffectHandler.OnTapSwitchOff -= TapParticleEffectHandler_OnTapSwitchOff;
        }
    }

    void OnParticleCollision(GameObject particle)
    {
        if (waterCount >= maxWaterCount)
        {
            if (tap != null && !switchedOffTap)
            {
                switchedOffTap = true;
                tap.SwitchOffTap();
                Invoke("LetScriptSwitchOffTap", 2);
            }
            return;
        }
        
        waterCount++;
        waterSliderUI.AddWater();

        UpdateWaterSize();
    }

    void LetScriptSwitchOffTap()
    {
        switchedOffTap = false;
    }

    void UpdateWaterSize()
    {
        Vector3 localPos = waterParent.localPosition;
        localPos.y = GetRespectiveValue(emptyWaterLocalY, filledWaterLocalY);
        waterParent.transform.localPosition = localPos;
        
        Vector3 localScale = waterVisual.localScale;
        float diameter = GetRespectiveValue(emptyWaterLocalDiameter, filledWaterLocalDiameter);
        localScale.x = localScale.z = diameter;
        waterVisual.transform.localScale = localScale;
    }

    float GetRespectiveValue(float empty, float filled)
    {
        float ratio = (float) waterCount / maxWaterCount;
        float difference = filled - empty;

        return empty + (ratio * difference);
    }

}
