using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ground movement")]
    public float maxGroundSpeed = 7f;       // top speed when on ground
    public float groundAcceleration = 60f;  // how fast you reach that speed
    public float groundFriction = 8f;       // how quickly you slow down when not pressing input

    [Header("Air movement")]
    public float maxAirSpeed = 8f;          // max horizontal speed in air
    public float airAcceleration = 30f;     // how fast you can change horizontal speed in air
    [Range(0f, 1f)]
    public float airControl = 0.6f;         // how much control you have mid-air (0 = none, 1 = full)

    [Header("Jumping")]
    public float jumpHeight = 1.4f;         // world units to reach at jump apex
    public float gravity = -25f;            // negative value, stronger = snappier jump
    public float coyoteTime = 0.1f;         // time after leaving ground you can still jump

    [Header("Wall jumping")]
    public float wallCheckDistance = 0.6f;  // how far from the player we search for walls
    public LayerMask wallMask;              // which layers count as walls
    public float wallJumpUpForce = 8f;      // vertical part of the wall jump
    public float wallJumpAwayForce = 7f;    // push away from wall
    public float wallJumpControlLockTime = 0.15f; // short time after wall jump where input is weaker

    private CharacterController controller;
    private Vector3 velocity;               // full velocity (x,z,y)
    private float lastGroundedTime;         // for coyote time
    [SerializeField] private float jumpBufferTime;
    private bool hasJumped;
    private float jumpBufferCounter = 0f;                // jump pressed this frame
    private Vector3 lastWallNormal;         // normal of last wall we can jump from
    private float wallControlLockTimer;     // timer after wall jump where control is reduced
    private Camera cam;                     // to move relative to camera
    public Animator anim;

    [Header("Camera Movement")]
    public float mouseSensitivity;
    private float yRotation = 0f;
   
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Update()
    {
        // --- INPUT ---
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        if (inputZ != 0 || inputX != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);


        // Move relative to camera forward/right (flat on XZ plane)
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();
        Vector3 camRight = cam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 wishDir = (camRight * inputX + camForward * inputZ).normalized;

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
            hasJumped = true;
        }
            
        
        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;

        bool grounded = controller.isGrounded;

        if (grounded)
        {
            lastGroundedTime = Time.time;
            GroundMove(wishDir);

            // reset vertical velocity so we stick to ground
            if (velocity.y < 0f)
                velocity.y = -2f;
        }
        else
        {
            AirMove(wishDir);
        }
        anim.SetBool("isGrounded", grounded);

        // Handle jump after movement code so we can jump from ground or wall
        HandleJump(grounded,hasJumped);

        // Apply gravity (always)
        velocity.y += gravity * Time.deltaTime;

        // Move character
        controller.Move(velocity * Time.deltaTime);

        // Update timers
        if (wallControlLockTimer > 0f)
            wallControlLockTimer -= Time.deltaTime;
    }

    void GroundMove(Vector3 wishDir)
    {
        ApplyFriction(groundFriction);
        Accelerate(wishDir, groundAcceleration, maxGroundSpeed);
    }

    void AirMove(Vector3 wishDir)
    {
        // reduce input while control is locked after a wall jump
        if (wallControlLockTimer > 0f)
            wishDir *= 0.3f; // weaker control right after wall jump
        else
            wishDir *= airControl;

        Accelerate(wishDir, airAcceleration, maxAirSpeed);
    }

    void ApplyFriction(float friction)
    {
        Vector3 horizVel = new Vector3(velocity.x, 0f, velocity.z);
        float speed = horizVel.magnitude;
        if (speed < 0.01f) return;

        float drop = speed * friction * Time.deltaTime;
        float newSpeed = Mathf.Max(speed - drop, 0f);
        horizVel *= newSpeed / speed;

        velocity.x = horizVel.x;
        velocity.z = horizVel.z;
    }

    void Accelerate(Vector3 wishDir, float accel, float maxSpeed)
    {
        if (wishDir.sqrMagnitude == 0f) return;

        Vector3 horizVel = new Vector3(velocity.x, 0f, velocity.z);
        float currentSpeed = Vector3.Dot(horizVel, wishDir);
        float addSpeed = maxSpeed - currentSpeed;
        if (addSpeed <= 0f) return;

        float accelSpeed = accel * Time.deltaTime;
        if (accelSpeed > addSpeed) accelSpeed = addSpeed;

        velocity += wishDir * accelSpeed;
    }

    void HandleJump(bool grounded, bool jump)
    {
        // If we have no buffered jump, do nothing
        if (jumpBufferCounter <= 0f)
            return;

        // 1) Ground / coyote jump
        if (grounded || Time.time - lastGroundedTime <= coyoteTime)
        {
            float jumpVel = Mathf.Sqrt(-2f * gravity * jumpHeight);
            velocity.y = jumpVel;

            // consume the buffer
            jumpBufferCounter = 0f;
            hasJumped = false;
            anim.SetTrigger("Jump");
            return;
        }

        // 2) Wall jump if we have a valid wall and we're in the air
        Vector3 wallNormal;
        if (CanWallJump(out wallNormal))
        {
            // Remove current vertical velocity to make jump consistent
            if (velocity.y < 0f) velocity.y = 0f;

            // OPTIONAL: damp horizontal velocity into the wall so we don't stack speed
            Vector3 horizVel = new Vector3(velocity.x, 0f, velocity.z);
            Vector3 tangential = Vector3.ProjectOnPlane(horizVel, wallNormal); // keep along-wall speed, kill into-wall

            // Build wall jump impulse
            Vector3 jumpDir = Vector3.up * wallJumpUpForce + wallNormal * wallJumpAwayForce;

            // Set new velocity instead of infinitely adding to it
            velocity = tangential + jumpDir;

            wallControlLockTimer = wallJumpControlLockTime;

            // consume the buffer
            jumpBufferCounter = 0f;
            hasJumped = false;
            return;
        }

        // if we reach here, we didn't jump this frame; buffer will keep ticking down in Update
    }


    bool CanWallJump(out Vector3 wallNormal)
    {
        wallNormal = Vector3.zero;

        // Sphere cast to find nearby walls
        Vector3 origin = transform.position;
        origin.y += controller.height * 0.5f; // cast from middle

        if (Physics.SphereCast(origin, controller.radius * 0.9f,
                               transform.forward, out RaycastHit hit,
                               wallCheckDistance, wallMask, QueryTriggerInteraction.Ignore))
        {
            wallNormal = hit.normal;
            return true;
        }

        // Also check left and right for better coverage
        if (Physics.SphereCast(origin, controller.radius * 0.9f,
                               transform.right, out hit,
                               wallCheckDistance, wallMask, QueryTriggerInteraction.Ignore))
        {
            wallNormal = hit.normal;
            return true;
        }

        if (Physics.SphereCast(origin, controller.radius * 0.9f,
                               -transform.right, out hit,
                               wallCheckDistance, wallMask, QueryTriggerInteraction.Ignore))
        {
            wallNormal = hit.normal;
            return true;
        }

        return false;
    }

    
}
