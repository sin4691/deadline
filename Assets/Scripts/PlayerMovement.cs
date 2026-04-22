using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float gravity = -15f;

    [Header("Sound")]
    [SerializeField] private AudioClip[] footstepRunSounds;
    [SerializeField] private AudioClip[] footstepWalkSounds;
    [SerializeField] private float footstepInterval = 0.5f;
    [SerializeField] private float runFootstepInterval = 0.3f;

    private float footstepTimer = 0f;
    private AudioSource audioSource;
    public bool isMoving;
    private bool isRunning;
    private bool isCrouching;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private Animator animator;

    [Header("Look")]
    [SerializeField] private Transform cam;
    public float mouseSensitivity = 0.1f;
    private Vector2 lookInput;
    private float xRotation = 0f;

    [Header("Crouch")]
    [SerializeField] private float standingHeight = 1.8f;
    [SerializeField] private float crouchHeight = 0.9f;
    [SerializeField] private float cameraStandY = 1.6f;
    [SerializeField] private float cameraCrouchY = 0.7f;
    [SerializeField] private float crouchSmoothSpeed = 10f;

    public float noiseRadius = 0f;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        HandleCrouch(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        PlayerLook();
        PlayerMove();
        UpdateCameraHeight();
        HandleFootsteps();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            GameManager.Instance.PlayerDied();
        }
    }
    private void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    private void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    private void OnSprint(InputValue value) => isRunning = value.isPressed;
    private void OnCrouch(InputValue value) => HandleCrouch(value.isPressed);
    private void PlayerLook()
    {
        if (GameManager.Instance.isPaused) return;
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
        isMoving = moveInput.magnitude > 0.1f;

        animator.SetBool("isWalking", isMoving && !isRunning);
        animator.SetBool("isRunning", isMoving && isRunning && !isCrouching);
        animator.SetBool("isCrouching", isCrouching);

        if (isMoving && isRunning)          noiseRadius = 15f;  // 달리기
        else if (isMoving && isCrouching)   noiseRadius = 2f;   // 앉아서 이동
        else if (isMoving)                  noiseRadius = 6f;   // 걷기
        else                                noiseRadius = 0f;   // 정지
    }
    private void HandleCrouch(bool pressed)
    {
        isCrouching = pressed;
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        float targetCenterY = targetHeight / 2f;

        controller.height = targetHeight;
        controller.center = new Vector3(0, targetCenterY, 0);
    }
    private void UpdateCameraHeight()
    {
        float targetCamY = isCrouching ? cameraCrouchY : cameraStandY;
        Vector3 currentCamPos = cam.localPosition;
        currentCamPos.y = Mathf.Lerp(currentCamPos.y, targetCamY, crouchSmoothSpeed * Time.deltaTime);
        cam.localPosition = currentCamPos;
    }
    void HandleFootsteps()
    {
        if (!isMoving || !controller.isGrounded)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            AudioClip[] clips = isRunning ? footstepRunSounds : footstepWalkSounds;
            if (clips != null && clips.Length > 0)
            {
                audioSource.volume = isCrouching ? 0.2f : 1f;
                audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
            }

            footstepTimer = isCrouching ? footstepInterval * 1.5f : 
                isRunning ? runFootstepInterval : footstepInterval;
        }
    }
}
