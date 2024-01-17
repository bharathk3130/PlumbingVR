using UnityEngine;

public class Player : MonoBehaviour
{

    PlayerInputActions playerInputActions;

    [Tooltip("Press 1 on keyboard / A on left controller to activate")]
    [SerializeField] GameObject leftHandRayInteractor;

    [Tooltip("Press 2 on keyboard / A on right controller to activate")]
    [SerializeField] GameObject rightHandRayInteractor;
    
    void Start()
    {
        leftHandRayInteractor.SetActive(false);
        rightHandRayInteractor.SetActive(false);

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        playerInputActions.Player.EnableLeftRayInteractor.performed += EnableLeftRayInteractor_performed;
        playerInputActions.Player.EnableLeftRayInteractor.canceled += EnableLeftRayInteractor_canceled;

        playerInputActions.Player.EnableRightRayInteractor.performed += EnableRightRayInteractor_performed;
        playerInputActions.Player.EnableRightRayInteractor.canceled += EnableRightRayInteractor_canceled;
    }

    // Left hand interactor
    void EnableLeftRayInteractor_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        leftHandRayInteractor.SetActive(true);
    }

    void EnableLeftRayInteractor_canceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        leftHandRayInteractor.SetActive(false);
    }
    

    // Right hand ray interactor
    void EnableRightRayInteractor_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        rightHandRayInteractor.SetActive(true);
    }

    void EnableRightRayInteractor_canceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        rightHandRayInteractor.SetActive(false);
    }

    void OnDestroy()
    {
        playerInputActions.Player.EnableLeftRayInteractor.performed -= EnableLeftRayInteractor_performed;
        playerInputActions.Player.EnableLeftRayInteractor.canceled -= EnableLeftRayInteractor_canceled;

        playerInputActions.Player.EnableRightRayInteractor.performed -= EnableRightRayInteractor_performed;
        playerInputActions.Player.EnableRightRayInteractor.canceled -= EnableRightRayInteractor_canceled;
    }

}
