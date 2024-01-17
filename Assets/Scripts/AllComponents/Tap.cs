using System;
using System.Collections;
using UnityEngine;

public class Tap : ConnectableComponent
{

    public event Action OnSwitchOn;
    public event Action OnSwitchOff;

    public TapParticleEffectHandler tapParticleEffectHandler;
    public bool isOpen { get; private set; } = false;
    bool canPinch = true; // Makes sure tap doesn't get toggled if the player keeps the pinch button helf down

    [SerializeField] Animator tapAnim;

    AnimateHands animateHands;

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Hand") || col.CompareTag("RightHand"))
        {
            animateHands = col.transform.GetChild(0).GetComponent<AnimateHands>();
            if (animateHands.isPinching && canPinch)
            {
                canPinch = false;
                StartCoroutine(ToggleTap());
            }
        }
    }

    void Update()
    {
        if (!canPinch && !animateHands.isPinching)
        {
            canPinch = true;
            animateHands = null;
        }
    }

    IEnumerator ToggleTap()
    {
        if (!isOpen)
        {
            tapAnim.SetTrigger("SwitchOn");

            yield return new WaitForSeconds(1);

            if (isConnected)
                OnSwitchOn();
        } else
        {
            tapAnim.SetTrigger("SwitchOff");

            yield return new WaitForSeconds(1);

            OnSwitchOff();
        }

        isOpen = !isOpen;
    }

    // Called when bucket is full
    public void SwitchOffTap()
    {
        isOpen = false;

        OnSwitchOff();
    }

}
