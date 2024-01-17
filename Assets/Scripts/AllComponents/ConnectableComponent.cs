using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ConnectableComponent : MonoBehaviour
{

    public event Action OnLetGo;
    public event Action OnDisconnect;
    public event Action OnConnect;
    public event Action OnDestroy;
    [HideInInspector] public GrabbingUI grabbingUI;

    protected XRGrabInteractable grabInteractable;
    protected Node touchedNode = null;

    [SerializeField] InteractionLayerMask interactionLayerMask;
    [SerializeField] GameObject visualsGO;

    float dissolveAnimDuration = 2;

    Rigidbody rb;
    Collider col;

    Node parentNode;

    protected bool isConnected;
    bool isGrabbed = false;
    bool isGrabbingFromUI = true;
    bool firstDrop = true;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    protected virtual void Start()
    {
        grabInteractable.lastSelectExited.AddListener((SelectExitEventArgs eventData) =>
        {
            // Idk why i have to check but it gives an error when the game quits if i dont
            if (gameObject.activeSelf)
                StartCoroutine(Dropped());
        });

        grabInteractable.selectEntered.AddListener(Grabbed);
    }

    public static GameObject SpawnConnectedComponent(GameObject componentPrefab, Vector3 pos, Transform parent, GrabbingUI grabbingUI)
    {
        GameObject componentGO = Instantiate(componentPrefab, pos, Quaternion.identity, parent);

        componentGO.transform.localEulerAngles = Vector3.zero;
        componentGO.SetActive(false);

        componentGO.GetComponent<Rigidbody>().isKinematic = true;
        componentGO.GetComponent<ConnectableComponent>().DisableVisuals();
        componentGO.GetComponent<ConnectableComponent>().grabbingUI = grabbingUI;

        return componentGO;
    }

    void EnableVisuals()
    {
        visualsGO.SetActive(true);
    }

    public void DisableVisuals()
    {
        visualsGO.SetActive(false);
    }

    public void Connect(Node parent)
    {
        OnConnect?.Invoke();

        // For organizing hierarchy
        transform.parent = GameManager.Instance.spawnedComponentsParent;

        parentNode = parent;
        isConnected = true;
        rb.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("ConnectedComponent");
    }

    // Is a listener
    void Grabbed(SelectEnterEventArgs eventData)
    {
        if (isGrabbingFromUI)
        {
            EnableVisuals();

            grabInteractable.interactionLayers = interactionLayerMask;
            grabbingUI.instantiated = true;
            isGrabbingFromUI = false;
        }

        if (isConnected)
            Disconnect();

        col.isTrigger = true;
        isGrabbed = true;
    }

    // Is a listener
    IEnumerator Dropped()
    {
        if (firstDrop)
        {
            // This runs if the player has grabbed it from the UI
            rb.isKinematic = false;
            transform.parent = GameManager.Instance.spawnedComponentsParent;
            firstDrop = false;
        }

        OnLetGo?.Invoke();

        col.isTrigger = false;
        isGrabbed = false;

        // Temporarily change it to "ConnectedComponent" because that layer doesn't interact with anything. This prevents the pipe from flying
        // away if its collider was overlapping another collider
        gameObject.layer = LayerMask.NameToLayer("ConnectedComponent");
        yield return new WaitForSeconds(0.2f);
        gameObject.layer = isConnected ? LayerMask.NameToLayer("ConnectedComponent") : LayerMask.NameToLayer("DisconnectedComponent");
    }

    public void Disconnect()
    {
        OnDisconnect?.Invoke();

        // Update parent (the node which the pipe got connected to
        parentNode.ChildGotDisconnected();
        parentNode = null;

        // Reset pipe
        isConnected = false;
        rb.isKinematic = false;
        gameObject.layer = LayerMask.NameToLayer("DisconnectedComponent");
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ground") && !isGrabbed)
            Invoke("DestroySelf", 1);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Ground") && !isGrabbed)
            Invoke("DestroySelf", 1);

        if (col.gameObject.CompareTag("Node"))
            touchedNode = col.gameObject.GetComponent<Node>();
    }

    protected virtual void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Node"))
            touchedNode = null;
    }

    void DestroySelf()
    {
        if (!isGrabbed && !isConnected)
        {
            OnDestroy?.Invoke();

            grabInteractable.enabled = false;

            Invoke("Destroy", dissolveAnimDuration);
        }
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

}
