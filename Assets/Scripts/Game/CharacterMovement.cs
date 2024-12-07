using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(GrappleHook))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float decelleration;
    [SerializeField, Range(0, 1)] private float airControl = 0.5f;

    [SerializeField] private float jumpHeight = 5;
    [SerializeField, Range(0,1)] private float jumpCancel = 0.5f;
    private bool isJumpCancellable;

    private const float GROUND_ANGLE = 45f;
    private bool isGrounded;
    private bool cancellingGrounded;
    public bool IsGrounded => isGrounded;

    private Rigidbody2D rb;
    private GrappleHook grappleHook;

    private float defaultGravity;
    [SerializeField] private float fallingGravityMultiplier;
    [SerializeField] private float softFallSpeedCap;
    private const float FALL_SPEED_SOFT_CAP_STRENGTH = 5;
    private const float ADDITIONAL_FALL_SPEED_CAP = 5;


    private PlayerInputActions actions;

    Vector2 moveDirection;
    Vector2 aimDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        actions = new PlayerInputActions();
        actions.Enable();
        grappleHook = GetComponent<GrappleHook>();
    }

    private void OnEnable()
    {
        if (actions == null) 
        { 
            actions = new PlayerInputActions();
            actions.Enable();
        }
        SubscribeInputs();
    }

    private void OnDisable()
    {
        UnsubscribeInputs();
    }

    void SubscribeInputs()
    {
        actions.Player.Move.performed += Move_performed;
        actions.Player.Move.canceled += Move_canceled;
        actions.Player.Jump.performed += Jump_performed;
        actions.Player.Jump.canceled += Jump_canceled;
        actions.Player.AimPoint.performed += AimPoint_performed;
        actions.Player.Grapple.performed += Grapple_performed;
        actions.Player.Grapple.canceled += Grapple_canceled;
    }

    private void Grapple_canceled(InputAction.CallbackContext obj)
    {

    }

    private void Grapple_performed(InputAction.CallbackContext obj)
    {
        if (obj.performed)
            Grapple();
    }

    private void AimPoint_performed(InputAction.CallbackContext obj)
    {
        Vector2 aimPosition = Camera.main.ScreenToWorldPoint(obj.ReadValue<Vector2>());
        aimDirection = (aimPosition - rb.position).normalized;
    }

    void UnsubscribeInputs()
    {
        actions.Player.Move.performed -= Move_performed;
        actions.Player.Move.canceled -= Move_canceled;
        actions.Player.Jump.performed -= Jump_performed;
        actions.Player.Jump.canceled -= Jump_canceled;
        actions.Player.AimPoint.performed -= AimPoint_performed;
    }

    #region inputFunctions

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        if (obj.performed)
            Jump();
    }
    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
            CancelJump();
    }

    private void Move_canceled(InputAction.CallbackContext obj)
    {
        if(obj.canceled)
            moveDirection = Vector2.Scale(obj.ReadValue<Vector2>(), Vector2.right).normalized;   
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        if(obj.performed)
            moveDirection = Vector2.Scale(obj.ReadValue<Vector2>(), Vector2.right).normalized;
    }

    #endregion

    private void FixedUpdate()
    {
        UpdateGravity();
        Move();
    }

    void UpdateGravity()
    {
        rb.gravityScale = rb.velocity.y < 0 ? fallingGravityMultiplier : rb.gravityScale = defaultGravity;

        if(rb.velocity.y < -Mathf.Abs(softFallSpeedCap) - ADDITIONAL_FALL_SPEED_CAP)
        {
            rb.velocity = new Vector2(rb.velocity.x, -Mathf.Abs(softFallSpeedCap) - ADDITIONAL_FALL_SPEED_CAP);
        }

        if(rb.velocity.y < -Mathf.Abs(softFallSpeedCap))
        {
            Vector2 desiredVelocity = new Vector2(rb.velocity.x, -Mathf.Abs(softFallSpeedCap));
            Vector2 force = (desiredVelocity - rb.velocity) * FALL_SPEED_SOFT_CAP_STRENGTH;

            rb.AddForce(force);
        }
    }

    void Move()
    {
        Vector2 force;
        Vector2 horizontalVelocity = Vector2.Scale(rb.velocity, Vector2.right);

        float movementDot = Vector2.Dot(horizontalVelocity, moveDirection);

        if (moveDirection.magnitude > 0.1f )
        {
            Vector2 desiredVelocity = moveDirection * moveSpeed;
            force = (desiredVelocity - horizontalVelocity) * acceleration;

            if (!IsGrounded) { force *= airControl; }
            rb.AddForce(force);
        }
        else
        {
            force = -horizontalVelocity * decelleration;

            if (!IsGrounded) { force *= airControl; }
            rb.AddForce(force);
        }
    }

    public void Jump()
    {
        if (!IsGrounded) return;
        Jump(jumpHeight, true);
    }

    public void Jump(float height, bool cancellable = false)
    {
        isJumpCancellable = cancellable;

        float jumpForce = Mathf.Sqrt(2 * defaultGravity * height);
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    public void CancelJump()
    {
        if (isJumpCancellable)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCancel);
        }
    }

    public void Grapple()
    {
        grappleHook.Fire(aimDirection);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (IsGround(contact.normal))
            {
                isGrounded = true;
                cancellingGrounded = false;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), 0.1f);
        }
    }

    void StopGrounded()
    {
        isGrounded = false;
    }

    bool IsGround(Vector2 normal)
    {
        return Vector2.Angle(normal, Vector2.up) < GROUND_ANGLE;
    }
}
