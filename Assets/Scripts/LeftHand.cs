using UnityEngine;

public class LeftHand : MonoBehaviour
{

    [SerializeField] GameObject inventory;

    [Tooltip("If your hand is tilted at an angle between these limits, the menu opens")]
    [SerializeField] Vector2 limits;

    void Update()
    {
        if (transform.localEulerAngles.z >= limits.x && transform.localEulerAngles.z <= limits.y)
        {
            if (!inventory.activeSelf)
                inventory.SetActive(true);
        } else
        {
            if (inventory.activeSelf)
                inventory.SetActive(false);
        }
    }
}
