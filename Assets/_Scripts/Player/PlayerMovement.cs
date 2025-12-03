using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ground movement")]
    public float maxGroundSpeed = 7f;    
    public float groundAcceleration = 60f;  
    public float groundFriction = 8f;       

    [Header("Air movement")]
    public float maxAirSpeed = 8f;      
    public float airAcceleration = 30f;  
    [Range(0f, 1f)]
    public float airControl = 0.6f;      

    [Header("Jumping")]
    public float jumpHeight = 1.4f;      
    public float gravity = -25f;       
    public float coyoteTime = 0.1f;   

    [Header("Wall jumping")]
    public float wallCheckDistance = 0.6f;  
    public LayerMask wallMask;            
    public float wallJumpUpForce = 8f;   
    public float wallJumpAwayForce = 7f;   
    public float wallJumpControlLockTime = 0.15f;

    private CharacterController controller;
    private Vector3 velocity;       
    private float lastGroundedTime;   
    [SerializeField] private float jumpBufferTime;
    private bool hasJumped;
    private float jumpBufferCounter = 0f;       
    private Vector3 lastWallNormal;  
    private float wallControlLockTimer;    
    private Camera cam;  
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

            if (velocity.y < 0f)
                velocity.y = -2f;
        }
        else
        {
            AirMove(wishDir);
        }
        anim.SetBool("isGrounded", grounded);
        
        HandleJump(grounded,hasJumped);
        
        velocity.y += gravity * Time.deltaTime;
        
        controller.Move(velocity * Time.deltaTime);
        
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
        if (wallControlLockTimer > 0f)
            wishDir *= 0.3f; 
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
        if (jumpBufferCounter <= 0f)
            return;

        if (grounded || Time.time - lastGroundedTime <= coyoteTime)
        {
            float jumpVel = Mathf.Sqrt(-2f * gravity * jumpHeight);
            velocity.y = jumpVel;

            jumpBufferCounter = 0f;
            hasJumped = false;
            anim.SetTrigger("Jump");
            return;
        }

        Vector3 wallNormal;
        if (CanWallJump(out wallNormal))
        {
            if (velocity.y < 0f) velocity.y = 0f;

            Vector3 horizVel = new Vector3(velocity.x, 0f, velocity.z);
            Vector3 tangential = Vector3.ProjectOnPlane(horizVel, wallNormal); 
            
            Vector3 jumpDir = Vector3.up * wallJumpUpForce + wallNormal * wallJumpAwayForce;
            
            velocity = tangential + jumpDir;

            wallControlLockTimer = wallJumpControlLockTime;
            
            jumpBufferCounter = 0f;
            hasJumped = false;
            return;
        }
    }


    bool CanWallJump(out Vector3 wallNormal)
    {
        wallNormal = Vector3.zero;
        
        Vector3 origin = transform.position;
        origin.y += controller.height * 0.5f;

        if (Physics.SphereCast(origin, controller.radius * 0.9f,
                               transform.forward, out RaycastHit hit,
                               wallCheckDistance, wallMask, QueryTriggerInteraction.Ignore))
        {
            wallNormal = hit.normal;
            return true;
        }
        
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
