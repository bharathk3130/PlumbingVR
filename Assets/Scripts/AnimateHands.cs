using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHands : MonoBehaviour
{

    public event Action OnPinch;
    public Action OnPinchStop;

    [SerializeField] InputActionProperty pinchAnimAction;
    [SerializeField] InputActionProperty grabAnimAction;
    Animator handAnim;

    public bool isPinching { get; private set; } = false;

    float pinchThreshold = 0.7f;
    float pinchVal;
    
    void Start()
    {
        handAnim = GetComponent<Animator>();
    }

    void Update()
    {
        pinchVal = pinchAnimAction.action.ReadValue<float>();
        handAnim.SetFloat("Trigger", pinchVal);

        float grabVal = grabAnimAction.action.ReadValue<float>();
        handAnim.SetFloat("Grip", grabVal);

        if (isPinching)
        {
            if (pinchVal < pinchThreshold)
            {
                isPinching = false;
                OnPinchStop?.Invoke();
            }
        } else
        {
            if (pinchVal >= pinchThreshold)
            {
                isPinching = true;
                OnPinch?.Invoke();
            }
        }
    }

}
