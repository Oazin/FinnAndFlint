using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Variables for moving
    protected float SPEED = 5f;
    [SerializeField]
    protected Rigidbody2D rb_player;
    [SerializeField]
    protected Animator ref_animator;
    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    protected bool isCurrentlyCollidingFloor;
    protected bool isCurrentlyCollidingPlayer;
    protected Vector3 Scale_Player;


    // ------------ Variables for power up ----------- 
    // Gravity's Variable
    [SerializeField]
    protected GameObject pref_PowerUpGravity;
    protected Vector3 pos_PowerUpGravity;

    // Flash's Variables
    [SerializeField]
    protected GameObject pref_PowerUpFlash;
    protected Vector3 pos_PowerUpFlash;

    // Slow motion's variables
    [SerializeField]
    protected GameObject pref_PowerUpSlowMotion;
    protected Vector3 pos_PowerUpSlowMotion;

    // Big player's variables
    [SerializeField]
    protected GameObject pref_PowerUpBig;
    protected Vector3 pos_PowerUpBig;

    // Small player's variables
    [SerializeField]
    protected GameObject pref_PowerUpSmall;
    protected Vector3 pos_PowerUpSmall;

    protected float timeRemaining = 10f;

    // Start is called before the first frame update
    void Start()
    {
        Scale_Player = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "Finn")
        {
            FinnMove();
        }
        if (gameObject.name == "Flint")
        {
            FlintMove();
        }

        // Decrease the timer 
        if ((SPEED != 5 || rb_player.gravityScale < 0 || transform.localScale != Scale_Player) && timeRemaining > 0)
        {
            timeRemaining -= 1 * Time.deltaTime;
        }

        // Reset the gravity
        if (rb_player.gravityScale < 0 && timeRemaining <= 0)
        {
            ChangeGravity();
            RespawnUpGravity();
        }

        // Reset the speed
        if (SPEED > 5 && timeRemaining <= 0 )
        {
            ResetSpeed();
            RespawnPowerUpFlash();
        }
        if (SPEED < 5 && timeRemaining <= 0)
        {
            ResetSpeed();
            RespawnPowerUpSlowMotion();
        }

        // Reset the size
        if (transform.localScale.x > Scale_Player.x && timeRemaining <= 0)
        {
            ResetSize();
            RespawnPowerUpBig();
        }
        if (transform.localScale.x < Scale_Player.x && timeRemaining <= 0)
        {
            ResetSize();
            RespawnPowerUpSmall();
        }
    }


    // ------------------------------------------------------------------------------------
    // --------------------------------- Player Moving ------------------------------------
    // ------------------------------------------------------------------------------------

    // Method allow to choose the controls for Finn
    protected void FinnMove()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            MoveLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }

        // If the player don't move the anamtions change and become idle
        else
        {
            ref_animator.SetBool("isMoving", false);
        }

        // The player can only jump when he is currently colliding with a floor or the other player
        if ((isCurrentlyCollidingFloor || isCurrentlyCollidingPlayer) && Input.GetKeyDown(KeyCode.Z))
        {
            JumpUp();
        }
        if ((isCurrentlyCollidingFloor || isCurrentlyCollidingPlayer) && Input.GetKeyDown(KeyCode.S))
        {
            JumpDown();
        }
    }

    // Method allow to choose the controls for Flint
    protected void FlintMove()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }

        // If the player don't move the anamtions change and become idle
        else
        {
            ref_animator.SetBool("isMoving", false);
        }

        // The player can only jump when he is currently colliding with a floor or the other player
        if ((isCurrentlyCollidingFloor || isCurrentlyCollidingPlayer) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            JumpUp();
        }
        if ((isCurrentlyCollidingFloor || isCurrentlyCollidingPlayer) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            JumpDown();
        }
    }

    // Method use to the player moves on the left and apply modification for animations 
    protected void MoveLeft()
    {
        transform.Translate(-SPEED * Time.deltaTime, 0, 0);
        spriteRenderer.flipX = true;
        ref_animator.SetBool("isMoving", true);
    }

    // Method use to the player moves on the right and apply modification for animations
    protected void MoveRight()
    {
        transform.Translate(SPEED * Time.deltaTime, 0, 0);
        spriteRenderer.flipX = false;
        ref_animator.SetBool("isMoving", true);
    }

    // Method use when the gravity is normal to allow the player jumps
    protected void JumpUp()
    {
        rb_player.AddForce(Vector2.up * 6, ForceMode2D.Impulse);
    }

    // Method use when the gravity is reversed to allow the player jumps
    protected void JumpDown()
    {
        rb_player.AddForce(Vector2.down * 6, ForceMode2D.Impulse);
    }



    // ------------------------------------------------------------------------------------
    // --------------------------------- Collisions --------------------------------------
    // ------------------------------------------------------------------------------------


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floors"))
        {
            isCurrentlyCollidingFloor = true;
            Debug.Log("[Floor] true");
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            isCurrentlyCollidingPlayer = true;
            Debug.Log("[Player] true");
        }


        // Collision with a gravity dot so that's change the gravity of the players
        if (collision.gameObject.CompareTag("PowerUpGravity"))
        {
            pos_PowerUpGravity = collision.gameObject.transform.position;
            ResetPowerUpTimer();
            Destroy(collision.gameObject);
            ChangeGravity();
        }

        if (collision.gameObject.CompareTag("PowerUpFlash"))
        {
            pos_PowerUpFlash = collision.gameObject.transform.position;
            ResetPowerUpTimer();
            Destroy(collision.gameObject);
            IncreaseSpeed();
        }

        if (collision.gameObject.CompareTag("PowerUpSlowMotion"))
        {
            pos_PowerUpSlowMotion = collision.gameObject.transform.position;
            ResetPowerUpTimer();
            Destroy(collision.gameObject);
            DecreaseSpeed();
        }

        if (collision.gameObject.CompareTag("PowerUpBig"))
        {
            pos_PowerUpBig = collision.gameObject.transform.position;
            ResetPowerUpTimer();
            Destroy(collision.gameObject);
            IncreaseSize();
        }

        if (collision.gameObject.CompareTag("PowerUpSmall"))
        {
            pos_PowerUpSmall = collision.gameObject.transform.position;
            ResetPowerUpTimer();
            Destroy(collision.gameObject);
            DecreaseSize();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // if the collision exit the player isn't currently colliding
        if (collision.gameObject.CompareTag("Floors"))
        {
            isCurrentlyCollidingFloor = false;
            Debug.Log("[Floor] false");
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            isCurrentlyCollidingPlayer = false;
            Debug.Log("[Player] false");
        }


    }


    // ------------------------------------------------------------------------------------
    // --------------------------------- Power Up -----------------------------------------
    // ------------------------------------------------------------------------------------

    // Reset the timer
    protected void ResetPowerUpTimer()
    {
        timeRemaining = 10;
    }

    // ---------- Gravity ------------
    // Change the gravity 
    protected void ChangeGravity()
    {
        rb_player.gravityScale = -rb_player.gravityScale;

        // Check and flip the Sprite renderer
        if (spriteRenderer.flipY == false)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }

    // When a player take the power up gravity that desapear so after 10 sec it has to reappear to reuse it
    protected void RespawnUpGravity()
    {
        Instantiate(pref_PowerUpGravity, pos_PowerUpGravity, Quaternion.identity);
    }


    // ---------- Speed ------------
    // Increase the speed of the player
    protected void IncreaseSpeed()
    {
        SPEED = 8;
    }

    // Reset the speed of the player at the default speed
    protected void ResetSpeed()
    {
        SPEED = 5;
    }

    // Decrease the speed of the player
    protected void DecreaseSpeed()
    {
        SPEED = 2;
    }

    // When a player take the power up Flash that desapear so after 10 sec it has to reappear to reuse it
    protected void RespawnPowerUpFlash()
    {
        Instantiate(pref_PowerUpFlash, pos_PowerUpFlash, Quaternion.identity);
    }

    // When a player take the power up Slow Motion that desapear so after 10 sec it has to reappear to reuse it
    protected void RespawnPowerUpSlowMotion()
    {
        Instantiate(pref_PowerUpSlowMotion, pos_PowerUpSlowMotion, Quaternion.identity);
    }

    // ---------- Size ------------
    // Increase the size of the player
    protected void IncreaseSize()
    {
        transform.localScale = new Vector3(8, 8, 1);
    }

    // Decrease the size of the player
    protected void DecreaseSize()
    {
        transform.localScale = new Vector3(2, 2, 1);
    }

    // Reset the size 
    protected void ResetSize()
    {
        transform.localScale = Scale_Player;
    }

    // When a player take the power up Big that desapear so after 10 sec it has to reappear to reuse it
    protected void RespawnPowerUpBig()
    {
        Instantiate(pref_PowerUpBig, pos_PowerUpBig, Quaternion.identity);
    }

    // When a player take the power up Small that desapear so after 10 sec it has to reappear to reuse it
    protected void RespawnPowerUpSmall()
    {
        Instantiate(pref_PowerUpSmall, pos_PowerUpSmall, Quaternion.identity);
    }
}
