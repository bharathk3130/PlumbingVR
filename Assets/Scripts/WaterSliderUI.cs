using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaterSliderUI : MonoBehaviour
{

    [SerializeField] int numberOfCollectors = 3;
    [SerializeField] TextMeshProUGUI waterText;
    [SerializeField] Image waterSlider;

    [field: SerializeField] public int waterPerCollector { get; private set; } = 250;
    int waterCount = 0;
    int totalWaterCount;

    void Awake()
    {
        totalWaterCount = numberOfCollectors * waterPerCollector;
    }

    public void AddWater()
    {
        if (waterCount <= totalWaterCount)
        {
            waterCount++;
            if (waterCount > totalWaterCount)
                waterCount = totalWaterCount;

            waterSlider.fillAmount = (float) waterCount / totalWaterCount;
            waterText.text = waterCount.ToString() + "/" + totalWaterCount.ToString() + "ml";
        }
    }

    public int GetWaterCount()
    {
        return waterCount;
    }

}
