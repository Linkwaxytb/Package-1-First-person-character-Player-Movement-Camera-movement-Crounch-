namespace PlayerMovements.Linkwax {

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player Movement Variables
    [Header("Player Movement")]
    [Tooltip("Player movement speed.")]
    public float speed = 10.0f;
    [Tooltip("Player rotation speed.")]
    public float rotationSpeed = 10.0f;

    // Sprint Variables
    [Header("Sprint")]
    [Tooltip("Speed of sprinting.")]
    public float sprintSpeed = 15.0f;

    // Crouch Variables
    [Header("Crouch")]
    [Tooltip("Movement speed when crouching.")]
    public float crouchSpeed = 5.0f;
    private bool isCrouching = false;
    [Tooltip("Height of crouching.")]
    public float crouchHeight = 1.0f;
    [Tooltip("Collider radius when crouching.")]
    public float crouchRadius = 0.4f;
    [Tooltip("Collider offset during crouching.")]
    public float capsuleOffset = 0.5f;

    // Jump Variables
    [Header("Jump")]
    [Tooltip("Jump height of the player.")]
    public float jumpHeight = 2.0f;
    private bool isGrounded = true;
    private Vector3 jumpDirection;

    // Camera Settings Variables
    [Header("Camera Settings")]
    [Tooltip("Camera rotation sensitivity.")]
    public float lookSpeed = 2.0f;
    [Tooltip("Maximum vertical look angle.")]
    public float lookXLimit = 45.0f;
    public Camera camera;
    float rotationX = 0;
    float currentYRotation;
    float desiredYRotation;
    float yRotationV;

    // Collider Settings Variables
    [Header("Collider Settings")]
    [Tooltip("Height of the capsule collider.")]
    public float capsuleHeight = 2.0f;
    [Tooltip("Radius of the capsule collider.")]
    public float capsuleRadius = 0.5f;

    private Rigidbody rb;
    private float originalGravity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        originalGravity = rb.velocity.y;
    }

    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        CheckGrounded();

        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = sprintSpeed;
            }
            else
            {
                speed = 10.0f;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                isCrouching = true;
                speed = crouchSpeed;
            }
            else
            {
                isCrouching = false;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpDirection = rb.velocity.normalized;
                Jump();
            }

            Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
            rb.velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
        }

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        desiredYRotation += Input.GetAxis("Mouse X") * rotationSpeed;
        currentYRotation = Mathf.SmoothDampAngle(currentYRotation, desiredYRotation, ref yRotationV, lookSpeed / 100);
        transform.rotation = Quaternion.Euler(0, currentYRotation, 0);

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (isCrouching)
        {
            capsule.height = crouchHeight;
            capsule.radius = crouchRadius;
            capsule.center = new Vector3(0, -capsuleOffset, 0);
        }
        else
        {
            capsule.height = capsuleHeight;
            capsule.radius = capsuleRadius;
            capsule.center = Vector3.zero;
        }
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        float distance = capsuleHeight / 2 + 0.1f;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, distance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, CalculateJumpVerticalSpeed(), rb.velocity.z);
    }

    float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics.gravity.y));
    }
}
}
