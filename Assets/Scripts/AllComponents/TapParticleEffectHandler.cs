using System;
using UnityEngine;

public class TapParticleEffectHandler : MonoBehaviour
{

    public event Action OnTapSwitchOff;

    [SerializeField] Tap tap;
    [SerializeField] GameObject tapIndicatorGO;
    
    ParticleSystem leakEffect;

    [SerializeField] float switchOnOffDelay = 0.3f;
    [SerializeField] float timeToHitBucketDelay;
    bool tapIsOn = false;

    void Awake()
    {
        leakEffect = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        WaterTankTap.Instance.OnSwitchOn += WaterTankTap_OnSwitchOn;
        WaterTankTap.Instance.OnSwitchOff += WaterTankTap_OnSwitchOff;

        tap.OnConnect += Tap_OnConnect;
        tap.OnDisconnect += Tap_OnDisconnect;
        tap.OnSwitchOn += Tap_OnSwitchOn;
        tap.OnSwitchOff += Tap_OnSwitchOff;
    }


    #region TAP ON/OFF
    void Tap_OnSwitchOn()
    {
        Invoke("SwitchOn", switchOnOffDelay);
    }

    void Tap_OnSwitchOff()
    {
        Invoke("SwitchOff", switchOnOffDelay);
    }

    void SwitchOn()
    {
        tapIsOn = true;
        TryPlayEffect();
    }

    void SwitchOff()
    {
        tapIsOn = false;
        StopEffect();
    }
    #endregion



    #region CONNECT/DISCONNECT
    void Tap_OnConnect()
    {
        // Tap got connected
        TryPlayEffect();
    }

    void Tap_OnDisconnect()
    {
        // Tap got disconnected(it's in your hand)
        StopAndClearEffect();
    }
    #endregion



    #region MASTER TAP ON/OFF
    void WaterTankTap_OnSwitchOn()
    {
        TryPlayEffect();
    }

    void WaterTankTap_OnSwitchOff()
    {
        StopEffect();
    }
    #endregion



    #region PLAY EFFECT
    void TryPlayEffect()
    {
        if (WaterTankTap.Instance.isOn && tapIsOn && !leakEffect.isPlaying)
        {
            PlayEffect();
        }
    }

    void PlayEffect()
    {
        leakEffect.Play();

        tapIndicatorGO.SetActive(true);

        //CancelInvoke("DisableTapIndicator");
        //Invoke("EnableTapIndicator", timeToHitBucketDelay);
    }
    #endregion



    #region STOP EFFECT
    void StopEffect()
    {
        if (leakEffect.isPlaying)
        {
            leakEffect.Stop();

            tapIndicatorGO.SetActive(false);
            OnTapSwitchOff?.Invoke();
            //CancelInvoke("EnableTapIndicator");
            //Invoke("DisableTapIndicator", timeToHitBucketDelay);
        }
    }
    
    void StopAndClearEffect()
    {
        if (leakEffect.isPlaying)
        {
            leakEffect.Stop();
            leakEffect.Clear();

            tapIndicatorGO.SetActive(false);
            OnTapSwitchOff?.Invoke();
            //CancelInvoke("EnableTapIndicator");
            //Invoke("DisableTapIndicator", timeToHitBucketDelay);
        }
    }
    #endregion



    #region TapIndicator
    void EnableTapIndicator()
    {
        if (leakEffect.isPlaying)
            tapIndicatorGO.SetActive(true);
    }

    void DisableTapIndicator()
    {
        if (!leakEffect.isPlaying)
        {
            tapIndicatorGO.SetActive(false);
            OnTapSwitchOff?.Invoke();
        }
    }
    #endregion

    void OnDestroy()
    {
        WaterTankTap.Instance.OnSwitchOn -= WaterTankTap_OnSwitchOn;
        WaterTankTap.Instance.OnSwitchOff -= WaterTankTap_OnSwitchOff;

        tap.OnConnect -= Tap_OnConnect;
        tap.OnDisconnect -= Tap_OnDisconnect;
    }

}
