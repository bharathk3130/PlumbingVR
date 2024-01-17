using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public Transform spawnedComponentsParent;

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);

        Instance = this;
    }

}
