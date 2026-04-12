using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float jumpHeight = 1.5f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isRunning;
    private bool isCrouching;

    [Header("Look")]
    [SerializeField] private Transform cam;
    [SerializeField] private float mouseSensitivity = 0.1f;
    private Vector2 lookInput;
    private float xRotation = 0f;

    [Header("Crouch")]
    [SerializeField] private float standingHeight = 1.8f;
    [SerializeField] private float crouchHeight = 0.9f;
    [SerializeField] private float cameraStandY = 1.6f;
    [SerializeField] private float cameraCrouchY = 0.7f;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        HandleCrouch(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        PlayerLook();
        PlayerMove();
    }
    private void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    private void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    private void OnJump() { if (controller.isGrounded) velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); }
    private void OnSprint(InputValue value) => isRunning = value.isPressed;
    private void OnCrouch(InputValue value) => HandleCrouch(value.isPressed);
    private void PlayerLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 60f);
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    private void PlayerMove()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f; 
        float targetSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * targetSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    private void HandleCrouch(bool pressed)
    {
        isCrouching = pressed;
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        float targetCenterY = targetHeight / 2f;

        controller.height = targetHeight;
        controller.center = new Vector3(0, targetCenterY, 0);

        Vector3 camPos = cam.localPosition;
        camPos.y = isCrouching ? cameraCrouchY : cameraStandY;
        cam.localPosition = camPos;
    }
}
