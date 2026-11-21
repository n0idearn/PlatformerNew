using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public enum FacingDirection
    {
        left, right
    }

    public Rigidbody2D rb;
    public float maxSpeed = 8f;
    public float accelerationTime = 0.2f;
    public float decelerationTime = 0.1f;

    public float walkingThreshold = 0.01f;
    private FacingDirection facingDirection = FacingDirection.right;

    private List<Collider2D> groundColliders = new List<Collider2D>();

    private bool wasGrounded;

    void Start()
    {

    }

    void FixedUpdate()
    {

        Vector2 playerInput = new Vector2();

        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.

        if (Keyboard.current.leftArrowKey.isPressed)
        {
            playerInput += Vector2.left;
        }

        if (Keyboard.current.rightArrowKey.isPressed)
        {
            playerInput += Vector2.right;
        }

        MovementUpdate(playerInput);

        bool grounded = IsGrounded();
        if (grounded != wasGrounded)
        {
            Debug.Log("IsGrounded changed to: " + grounded);
            wasGrounded = grounded;
        }
    }

    private void MovementUpdate(Vector2 movementInput)
    {
        float velocityX = rb.linearVelocity.x;

        if (movementInput.x != 0f)
        {
            float acceleration = maxSpeed / accelerationTime;
            velocityX += movementInput.x * acceleration * Time.fixedDeltaTime;

            if (velocityX > maxSpeed)
            {
                velocityX = maxSpeed;
            }
            else if (velocityX < -maxSpeed)
            {
                velocityX = -maxSpeed;
            }

            if (movementInput.x < 0f)
            {
                facingDirection = FacingDirection.left;
            }
            else
            {
                facingDirection = FacingDirection.right;
            }
        }
        else
        {
            float deceleration = maxSpeed / decelerationTime;
            if (velocityX > 0f)
            {
                velocityX -= deceleration * Time.fixedDeltaTime;
                if (velocityX < 0f)
                {
                    velocityX = 0f;
                }
            }
            else if (velocityX < 0f)
            {
                velocityX += deceleration * Time.fixedDeltaTime;
                if (velocityX > 0f)
                {
                    velocityX = 0f;
                }
            }
        }

        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        groundColliders.Remove(collision.collider);
    }

    private void CheckGround(Collision2D collision)
    {
        bool touchingGround = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                touchingGround = true;
            }
        }

        if (touchingGround)
        {
            if (!groundColliders.Contains(collision.collider))
            {
                groundColliders.Add(collision.collider);
            }
        }
        else
        {
            groundColliders.Remove(collision.collider);
        }
    }

    public bool IsWalking()
    {
        return rb.linearVelocity.x > walkingThreshold || rb.linearVelocity.x < -walkingThreshold;
    }
    public bool IsGrounded()
    {
        return groundColliders.Count > 0;
    }

    public FacingDirection GetFacingDirection()
    {
        return facingDirection;
    }
}
