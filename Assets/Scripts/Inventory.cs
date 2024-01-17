using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [SerializeField] Transform rightHand;
    [SerializeField] AnimateHands rightHandAnimateHands;
    [SerializeField] Transform scrollingTransform;

    [Tooltip("Minimum amount to swipe")]
    [SerializeField] float threshold = 0.05f;
    [SerializeField] float panelWidth;
    [SerializeField] float scrollDuration;

    Vector3 pinchStartPos;
    bool handInBounds = false;

    float timer = 0;

    enum Panel
    {
        Pipes,
        Joints,
        Misc,
    }
    Panel currentPanel = Panel.Pipes;
    
    [Serializable]
    struct PanelGameObjectRef
    {
        public Panel panel;
        public GameObject panelGO;
    }
    [SerializeField] PanelGameObjectRef[] panelGORefArray;
    Dictionary<Panel, GameObject> panelGoRefDict = new Dictionary<Panel, GameObject>();

    void Awake()
    {
        foreach (PanelGameObjectRef panelRef in panelGORefArray)
            panelGoRefDict[panelRef.panel] = panelRef.panelGO;
    }

    void Start()
    {
        rightHandAnimateHands.OnPinch += RightHandAnimateHands_OnPinch;
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("RightHand"))
        {
            handInBounds = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("RightHand"))
        {
            handInBounds = false;
        }
    }

    Vector3 GetRightHandPosWRTInventory()
    {
        return rightHand.position - transform.position;
    }

    void RightHandAnimateHands_OnPinch()
    {
        if (handInBounds)
        {
            //pinchStartPos = rightHand.position;
            pinchStartPos = GetRightHandPosWRTInventory();

            rightHandAnimateHands.OnPinchStop += RightHandAnimateHands_OnPinchStop;
        }
    }
    
    void RightHandAnimateHands_OnPinchStop()
    {
        Vector3 pinchEndPos = GetRightHandPosWRTInventory();

        Vector3 displacement = pinchEndPos - pinchStartPos;
        Vector3 projectionAlongInventory = Vector3.Project(displacement, transform.right);

        float magnitude = Vector3.Dot(projectionAlongInventory, transform.right.normalized);

        if (Math.Abs(magnitude) >= threshold)
        {
            if (magnitude > 0)
            {
                // Swiped right - Move gameobject right - Select the panel to the left
                if (currentPanel != Panel.Pipes)
                {

                    timer = 0;
                    CancelInvoke("DisableOldAndEnableCurrentPanel");
                    Invoke("DisableOldAndEnableCurrentPanel", scrollDuration / 4);

                    if (IsScrolling())
                    {
                        // No need to call LerpToTarget() as it's already running
                        currentPanel--;
                    } else
                    {
                        currentPanel--;
                        StartCoroutine(LerpToTarget());
                    }
                }
            } else
            {
                // Swiped left - Move gameobject left
                if (currentPanel != Panel.Misc)
                {
                    timer = 0;
                    CancelInvoke("DisableOldAndEnableCurrentPanel");
                    Invoke("DisableOldAndEnableCurrentPanel", scrollDuration / 4);

                    if (IsScrolling())
                    {
                        // No need to call LerpToTarget() as it's already running
                        currentPanel++;
                    } else
                    {
                        currentPanel++;
                        StartCoroutine(LerpToTarget());
                    }
                }
            }
        }

        rightHandAnimateHands.OnPinchStop -= RightHandAnimateHands_OnPinchStop;
    }

    void DisableOldAndEnableCurrentPanel()
    {
        foreach (PanelGameObjectRef panelRef in panelGORefArray)
        {
            if (panelRef.panel != currentPanel)
            {
                panelGoRefDict[panelRef.panel].SetActive(false);
            } else
            {
                panelGoRefDict[currentPanel].SetActive(true);

            }
        }
    }

    bool IsScrolling()
    {
        return scrollingTransform.localPosition.x != CalculateTargetX();
    }

    float CalculateTargetX()
    {
        return -(int)currentPanel * panelWidth;
    }

    IEnumerator LerpToTarget()
    {
        while (timer < scrollDuration)
        {
            scrollingTransform.localPosition = Vector3.Lerp(scrollingTransform.localPosition, Vector3.right * CalculateTargetX(), 
                timer / scrollDuration);
            timer += Time.deltaTime;

            yield return null;
        }

        scrollingTransform.localPosition = Vector3.right * CalculateTargetX();
    }

}
