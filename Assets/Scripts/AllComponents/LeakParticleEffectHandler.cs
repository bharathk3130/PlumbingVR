using Unity.VisualScripting;
using UnityEngine;

public class LeakParticleEffectHandler : MonoBehaviour
{

    [SerializeField] Node node;
    [SerializeField] ConnectableComponent connectableComponent;

    ParticleSystem leakEffect;

    void Awake()
    {
        leakEffect = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        WaterTankTap.Instance.OnSwitchOn += WaterTankTap_OnSwitchOn;
        WaterTankTap.Instance.OnSwitchOff += WaterTankTap_OnSwitchOff;

        if (connectableComponent != null)
        {
            // Make sure this doesn't run for the water tank node
            connectableComponent.OnConnect += ConnectableComponent_OnConnect;
            connectableComponent.OnDisconnect += ConnectableComponent_OnDisconnect;
        }

        node.OnChildConnected += Node_OnChildConnected;
        node.OnChildDisconnected += Node_OnChildDisconnect;
    }


    #region CONNECT/DISCONNECT
    void ConnectableComponent_OnDisconnect()
    {
        // Pipe got disconnected(it's in your hand)
        StopAndClearEffect();
    }

    void ConnectableComponent_OnConnect()
    {
        // Pipe got connected
        if (WaterTankTap.Instance.isOn && !node.isConnected)
        {
            // If the switch is on and the pipe isn't connected to a child pipe, play effect
            PlayEffect();
        }
    }
    #endregion



    #region CHILD NODE CONNECTED/DISCONNECTED
    void Node_OnChildConnected()
    {
        StopAndClearEffect();
    }

    void Node_OnChildDisconnect()
    {
        // Child got disconnected so play effect if the tap is on
        if (WaterTankTap.Instance.isOn)
        {
            PlayEffect();
        }
    }
    #endregion



    #region MASTER TAP ON/OFF
    void WaterTankTap_OnSwitchOn()
    {
        if (!node.isConnected)
        {
            // Main tap got switched on so play effect if the node isn't connected to a child pipe
            PlayEffect();
        }
    }

    void WaterTankTap_OnSwitchOff()
    {
        StopEffect();
    }
    #endregion


    #region PLAY EFFECT
    void PlayEffect()
    {
        leakEffect.Play();
    }
    #endregion



    #region STOP EFFECT
    void StopAndClearEffect()
    {
        if (leakEffect.isPlaying)
        {
            leakEffect.Stop();
            leakEffect.Clear();
        }
    }

    void StopEffect()
    {
        if (leakEffect.isPlaying)
        {
            leakEffect.Stop();
        }
    }
    #endregion



    void OnDestroy()
    {
        WaterTankTap.Instance.OnSwitchOn -= WaterTankTap_OnSwitchOn;
        WaterTankTap.Instance.OnSwitchOff -= WaterTankTap_OnSwitchOff;

        if (connectableComponent != null)
        {
            connectableComponent.OnConnect -= ConnectableComponent_OnConnect;
            connectableComponent.OnDisconnect -= ConnectableComponent_OnDisconnect; 
        }

        node.OnChildConnected -= Node_OnChildConnected;
        node.OnChildDisconnected -= Node_OnChildDisconnect;
    }

}
