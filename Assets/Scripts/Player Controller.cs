using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private PlayerInput playerInput;
    private Rigidbody rb;
    private PlayerInputActions playerInputActions;
    private Camera mainCamera;

    private Vector3 currentControllerLookDirection = Vector3.zero;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump;

        if (playerInput.currentControlScheme == "Gamepad")
            Cursor.visible = false;
    }

    private void Update()
    {
        // Get current controller look input
        Vector2 lookInputVector = playerInputActions.Player.Look.ReadValue<Vector2>();
        if (lookInputVector != Vector2.zero )
            currentControllerLookDirection = Vector3.right * lookInputVector.x + Vector3.forward * lookInputVector.y;    
    }

    private void FixedUpdate()
    {     
        // Movement
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        rb.velocity = new Vector3(inputVector.x, 0, inputVector.y) * moveSpeed;
   
        // Look by controller
        if (currentControllerLookDirection.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(currentControllerLookDirection, Vector3.up);
        }
        // Look by mouse
        else
        {
            Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayLength;

            if (groundPlane.Raycast(cameraRay, out rayLength))
            {
                Vector3 pointToLook = cameraRay.GetPoint(rayLength);
                transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
            }
        }     
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
            Debug.Log("Jump");
    }
}
