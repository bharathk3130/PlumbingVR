using UnityEngine;

public class ConnectableComponentVisual : MonoBehaviour
{

    const string DISSOLVE_AMOUNT_STRING = "_DissolveAmount";

    [SerializeField] ConnectableComponent connectableComponent;
    [SerializeField] MeshRenderer[] meshRenderersArray;

    Material[] materialArray;

    float dissolveSpeed = 0.03f;
    bool dissolve = false;

    void Awake()
    {
        materialArray = new Material[meshRenderersArray.Length];

        for (int i = 0; i < materialArray.Length; i++)
        {
            materialArray[i] = meshRenderersArray[i].material;
        }
    }

    void Start()
    {
        connectableComponent.OnDestroy += Dissolve;
    }

    void Dissolve()
    {
        dissolve = true;
        foreach (Material mat in materialArray)
        {
            mat.SetFloat(DISSOLVE_AMOUNT_STRING, 0);
        }
    }

    void Update()
    {
        float dissolveAmount = materialArray[0].GetFloat(DISSOLVE_AMOUNT_STRING);
        if (dissolve && dissolveAmount < 1)
        {
            float newDissovleAmount = Mathf.Lerp(dissolveAmount, 1, dissolveSpeed);

            foreach (Material mat in materialArray)
                mat.SetFloat(DISSOLVE_AMOUNT_STRING, newDissovleAmount);
        } else if (dissolveAmount >= 1)
        {
            foreach (MeshRenderer renderer in meshRenderersArray)
                renderer.enabled = false;
        }
    }

    void OnApplicationQuit()
    {
        foreach (Material mat in materialArray)
            mat.SetFloat(DISSOLVE_AMOUNT_STRING, 0);
    }
}
