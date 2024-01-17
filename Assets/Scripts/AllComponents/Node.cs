using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Node : MonoBehaviour
{

    public event Action OnChildConnected;
    public event Action OnChildDisconnected;

    public bool isFlipped = false;

    [SerializeField] ConnectableComponent connectableComponent;

    XRSocketInteractor socket;
    ConnectableComponent connectedConnectableComponent;

    public bool isConnected = false;

    [SerializeField] bool isWaterTankNode = false;


    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    void Start()
    {
        if (!isWaterTankNode)
        {
            connectableComponent.OnConnect += ConnectableComponent_OnConnect;
            connectableComponent.OnDisconnect += ConnectableComponent_OnDisconnect;
        }

        if (!isWaterTankNode)
            DisableSelf();
    }

    void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    void ConnectableComponent_OnDisconnect()
    {
        Disconnect();
        DisableSelf();

        if (isFlipped)
        {
            isFlipped = false;
            FlipElbowJointNode();
        }
    }

    void ConnectableComponent_OnConnect()
    {
        gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if (!isWaterTankNode)
        {
            connectableComponent.OnConnect -= ConnectableComponent_OnConnect;
            connectableComponent.OnDisconnect -= ConnectableComponent_OnDisconnect;
        }
    }

    void Update()
    {
        if(!isConnected && socket.hasSelection)
        {
            connectedConnectableComponent = socket.GetOldestInteractableSelected().transform.GetComponent<ConnectableComponent>();
            connectedConnectableComponent.Connect(this);
            isConnected = true;

            OnChildConnected?.Invoke();
        }
    }

    public void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            connectedConnectableComponent.Disconnect();
            connectedConnectableComponent = null;
        }
    }

    public void ChildGotDisconnected()
    {
        isConnected = false;

        OnChildDisconnected?.Invoke();
    }

    //void OnTriggerEnter(Collider col)
    //{
    //    if (isConnected) return;

    //    //if (col.gameObject.CompareTag("T-Joint Main"))
    //    //    col.transform.parent.GetComponent<TJoint>().MainGrabbed();
    //    //else if (col.gameObject.CompareTag("T-Joint Side"))
    //    //    col.transform.parent.GetComponent<TJoint>().SideGrabbed();
    //}

    // Get's called on the pipe which the elbow joint is to be attached to
    public void ParentFlipNodeToAcceptElbowJoint()
    {
        transform.localEulerAngles += Vector3.up * 180;
    }

    // Get's called on the elbow joint's node
    public void FlipElbowJointNode()
    {
        transform.localEulerAngles += (Vector3.up * 180) - (Vector3.right * 180);
    }

}
