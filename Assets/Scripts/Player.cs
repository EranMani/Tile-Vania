using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
   
    // Config
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float climbSpeed = 3f;
    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] LayerMask ladderLayerMask;

    [Range(1,10)]
    [SerializeField] float jumpVelocity;
    [Range(0.01f, 0.9f)]
    [SerializeField] float playerToGroundRayExtra;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);

    private float normalGravity;

    // State
    private bool isAlive = true;
    private bool isOnLadder = false;

    // Cached component references
    Rigidbody2D rb;
    Animator anim;
    CapsuleCollider2D collider;
    GameSession gameSession;
   
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        normalGravity = rb.gravityScale;
        gameSession = FindObjectOfType<GameSession>();
    }

    void Update()
    {
        if (!isAlive) { return; }

        UpdatePlayerMovement();

        if (IsPlayerMoving())
        {      
            FlipPlayerSprite();
        }

        if (collider.IsTouchingLayers(ladderLayerMask))
        {
            isOnLadder = true;
            rb.gravityScale = 0f;
        }
        else
        {
            isOnLadder = false;
            rb.gravityScale = normalGravity;
        }

        
        if (isOnLadder)
        {         
            float verticalInput = Input.GetAxisRaw("Vertical");
            Vector2 climbVelocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed);
            rb.velocity = climbVelocity;                 
        }

        Jump();
        UpdateAnimations();
        Die();
    }

    private void UpdatePlayerMovement()
    {
        // Get horizontal player input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        // Adjust movement speed
        float horizontalMovement = horizontalInput * movementSpeed;
        // Make the player move right or left
        rb.velocity = new Vector2(horizontalMovement, rb.velocity.y);
    }

    private bool IsPlayerMoving()
    {
        return Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
    }

    private bool IsPlayerClimbing()
    {
        return Mathf.Abs(rb.velocity.y) > Mathf.Epsilon && isOnLadder;
    }

    private bool IsPlayerJumping()
    {
        return Input.GetButtonDown("Jump") && IsGrounded();
    }

    private void Jump()
    {
        if (IsPlayerJumping())
        {
            rb.velocity = Vector2.up * jumpVelocity;
        }

        if (rb.velocity.y < 0)
        {
            // (fallMultiplier - 1) accounting for the physics system normal gravity since unity physics 
            // already applying 1 multiple of the gravity
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButtonDown("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }    
    }

    private void Die()
    {
        if (collider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            anim.SetTrigger("Die");
            rb.velocity = deathKick;
            isAlive = false;
            gameSession.ProcessPlayerDeath();
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(collider.bounds.center, Vector2.down, collider.bounds.extents.y + playerToGroundRayExtra, platformLayerMask);
        // Debug.DrawRay(collider.bounds.center, Vector2.down * (collider.bounds.extents.y + playerToGroundRayExtra), Color.red);
    
        if (hit)
        {
            // print(hit.collider);
            return true;
        }

        return false;
    }

    
    private void FlipPlayerSprite()
    {
        transform.localScale = new Vector2(Math.Sign(rb.velocity.x), transform.localScale.y);
    }

    private void UpdateAnimations()
    {
        anim.SetBool("Run", IsPlayerMoving());
        anim.SetBool("Climb", IsPlayerClimbing());
    }

   

}
