using UnityEngine;

public class GrabbingUI : MonoBehaviour
{

    [HideInInspector] public bool instantiated = false;

    [SerializeField] GameObject componentPrefab;
    [SerializeField] Vector3 rotSpeed;

    GameObject componentGO;

    void Start()
    {
        componentGO = ConnectableComponent.SpawnConnectedComponent(componentPrefab, transform.position, transform.parent, this);
    }

    void Update()
    {
        transform.Rotate(new Vector3(rotSpeed.x, rotSpeed.y, rotSpeed.z) * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if (componentGO)
            componentGO.SetActive(true);
    }

    void OnTriggerExit(Collider col)
    {
        if (!instantiated)
            componentGO.SetActive(false);
        else
        {
            instantiated = false;
            
            componentGO = ConnectableComponent.SpawnConnectedComponent(componentPrefab, transform.position, transform.parent, this);
        }
    }
}
