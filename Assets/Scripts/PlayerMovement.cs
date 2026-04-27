using System.Collections;
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
    public float noiseRadius = 0f;
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

    [Header("Jumpscare")]
    [SerializeField] private Transform jumpScarePoint;
    [SerializeField] private AudioClip jumpScareSound;
    public Animator jumpScareAnimator;

    [Header("Flashlight")]
    [SerializeField] private FlashLight flashLight;
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
            StartCoroutine(JumpScare());
        }
    }
    private void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    private void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    private void OnSprint(InputValue value) => isRunning = value.isPressed;
    private void OnCrouch(InputValue value)
    {
        if (GameManager.Instance.isPaused) return;
        HandleCrouch(value.isPressed);
    }
    private void PlayerLook()
    {
        if (GameManager.Instance.isPaused|| GameManager.Instance.currentState != GameState.Playing) return;
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
                audioSource.volume = isCrouching ? 0.2f : isRunning ? 1f : 0.5f;
                audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
            }

            footstepTimer = isCrouching ? footstepInterval * 1.5f : 
                isRunning ? runFootstepInterval : footstepInterval;
        }
    }
    IEnumerator JumpScare()
    {
        GameManager.Instance.isPaused = true;
        HandleCrouch(false);
        Vector3 camPos = cam.localPosition;
        camPos.y = cameraStandY;
        cam.localPosition = camPos;
        if (flashLight != null) flashLight.TurnOff();
        // 플레이어 점프스케어 공간으로 이동
        controller.enabled = false;
        transform.position = jumpScarePoint.position;
        transform.rotation = jumpScarePoint.rotation; // 방향도 맞추기
        velocity = Vector3.zero;
        xRotation = -10f;
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Bite 애니메이션 실행
        jumpScareAnimator.SetTrigger("Bite");
        StartCoroutine(CameraShake(1.7f, 0.03f));
        // 사운드
        if (jumpScareSound != null)
            audioSource.PlayOneShot(jumpScareSound,5f);
        // 애니메이션 재생되는 동안 대기
        yield return new WaitForSeconds(1.7f);
        float suckDuration = 0.1f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        float startFOV = Camera.main.fieldOfView;

        while (elapsed < suckDuration)
        {
            elapsed += Time.unscaledDeltaTime; 
            float t = elapsed / suckDuration;
            transform.position = Vector3.Lerp(startPos, startPos +
                (transform.forward + transform.right * -0.4f).normalized * 0.3f, t);
            Camera.main.fieldOfView = Mathf.Lerp(startFOV, 20f, t);

            yield return null;
        }
        GameManager.Instance.isPaused = false;
        GameManager.Instance.PlayerDied();
    }
    IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = cam.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            cam.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            yield return null;
        }

        cam.localPosition = originalPos;
    }
}
