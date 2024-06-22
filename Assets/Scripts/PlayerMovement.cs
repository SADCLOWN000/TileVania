using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float jumpSpeed = 5;
    [SerializeField] float climbSpeed = 7;
    [SerializeField] Vector2 deathKick = new Vector2(20f, 20f);
    [SerializeField] GameObject arrow;
    [SerializeField] Transform bow;
    [SerializeField] float rateOfFire = 0.3f;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;
    
    bool isAlive = true;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if (!isAlive) { return; }
        Die();
        Run();
        FlipSprite();
        ClimbLadder();
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        StartCoroutine(RateOfFire());
        myAnimator.SetTrigger("isShooting");
    }
    IEnumerator RateOfFire()
    {
        yield return new WaitForSecondsRealtime(rateOfFire);
        Instantiate(arrow, bow.position, transform.rotation);
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (value.isPressed)
            {
                myRigidbody.velocity += new Vector2(0f, jumpSpeed);
            }
        }
    }


    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizantalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizantalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizantalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizantalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void ClimbLadder()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing Ladder")))
        {
            Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
            myRigidbody.velocity = climbVelocity;
            myRigidbody.gravityScale = 0f;

            bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
            myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
        }
        else
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
        }
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy"))||
            myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    private void OnTriggerEnter2D(Collider2D collidedObject)
    {
        if (collidedObject.gameObject.CompareTag("Enemy"))
        {
            Destroy(collidedObject.gameObject);
        }
    }
}