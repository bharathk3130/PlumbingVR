using System;
using System.Collections;
using UnityEngine;

public class WaterTankTap : MonoBehaviour
{

    public static WaterTankTap Instance;

    public event Action OnSwitchOn;
    public event Action OnSwitchOff;

    [HideInInspector] public bool isOn = false;
    [HideInInspector] public AnimateHands animateHands;

    [SerializeField] Animator anim;

    bool canSwitchOnOff = true;

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }

    IEnumerator SwitchOnOff()
    {
        isOn = !isOn;

        if (isOn)
        {
            anim.SetTrigger("OpenTap");

            yield return new WaitForSeconds(1);
            OnSwitchOn?.Invoke();
        } else
        {
            anim.SetTrigger("CloseTap");

            yield return new WaitForSeconds(1);
            OnSwitchOff?.Invoke();
        }
    }

    void Update()
    {
        if (!canSwitchOnOff && !animateHands.isPinching)
        {
            canSwitchOnOff = true;
            animateHands = null;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Hand") || col.CompareTag("RightHand"))
        {
            animateHands = col.transform.GetChild(0).GetComponent<AnimateHands>();
            if (animateHands.isPinching && canSwitchOnOff)
            {
                canSwitchOnOff = false;
                StartCoroutine(SwitchOnOff());
            }
        }
    }

}
