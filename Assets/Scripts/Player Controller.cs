using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Multipliers")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private bool MoveTowardCursor;

    private PlayerInput playerInput;
    private Rigidbody rb;
    private PlayerInputActions playerInputActions;
    private Camera mainCamera;
    private Animator animator;

    private Vector3 currentControllerLookDirection = Vector3.zero;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        animator = GetComponentInChildren<Animator>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump;

        if (playerInput.currentControlScheme == "Gamepad")
            Cursor.visible = false;
    }

    private void Update()
    {
        // Get current controller look input -- TODO: Try moving to Move function
        Vector2 lookInputVector = playerInputActions.Player.Look.ReadValue<Vector2>();
        if (lookInputVector != Vector2.zero)
            currentControllerLookDirection = Vector3.right * lookInputVector.x + Vector3.forward * lookInputVector.y;   
    }

    private void FixedUpdate()
    {
        Move();
        Look();      
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void Move()
    {
        // Get Movement input and multiply by moveSpeed to get velocity needed
        Vector2 movementVelocity = playerInputActions.Player.Movement.ReadValue<Vector2>() * moveSpeed;

        if (MoveTowardCursor)
            rb.velocity = ((transform.right * movementVelocity.x) + (transform.forward * movementVelocity.y));
        else
            rb.velocity = new Vector3(movementVelocity.x, rb.velocity.y, movementVelocity.y);

        animator.SetFloat("speed", movementVelocity.sqrMagnitude);
    }

    private void Look()
    {      
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
}
