using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;

    [Header("Jump")]
    [SerializeField] private float jumpStrenght = 5f;
    [SerializeField] private float jumpTime = 0.5f;

    [Header("Ground check")]
    [SerializeField] private float extraHeight = 0.2f;
    [SerializeField] private LayerMask GroundIs;

    [HideInInspector] private bool IsFacingRight = true;

    private Rigidbody2D rb;
    private Collider2D coll;
    private RaycastHit2D groundhit;
    private Animator amin;
    private float moveInput;
    private bool IsJumping;
    private bool IsFalling;
    private float jumpTimeCounter;

    private Coroutine resetTriggerCoroutine;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        amin =GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }
    private void Update()
    {
        Move();
        Jump();
    }
    #region Movement
    private void Move()
    {
        moveInput = UserInputs.instance.MoveInput.x;
        if (moveInput > 0 || moveInput < 0)
        {
            amin.SetBool("IsMove", true);
            TurnCheck();
        }
        else
        {
            amin.SetBool("IsMove", false);
        }
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocityY);
    }

    private void Jump()
    {
        if(InputManager.jumpPressed && IsGrounded())
        {
            IsJumping = true;
            jumpTimeCounter = jumpTime; 
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpStrenght);
            amin.SetTrigger("jump");
        }
        if (InputManager.JumpHeld)
        {
            if(jumpTimeCounter > 0 && IsJumping)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpStrenght);
                jumpTimeCounter -= Time.deltaTime;
            }
            else if (jumpTimeCounter == 0)
            {
                IsFalling = true;
                amin.SetBool("IsFall", true);
                IsJumping = false;
                
            }
            else
            {
                IsJumping = false;
            }
        }
        if(InputManager.jumpReleased)
        {
            IsJumping = false;
            IsFalling = true;
            //amin.SetTrigger("land");
        }
        
        if(!IsGrounded() && !IsJumping)
        {
            IsFalling = true;
            amin.SetBool("IsFall", true);
        }
        if (!IsJumping && IsLanded())
        {
            amin.SetTrigger("land");
            amin.SetBool("IsFall",false);
            resetTriggerCoroutine = StartCoroutine(Reset());
        }
        
        drawGroundCheck();
    }
    #endregion


    #region turn check
    
    private void Turn()
    {
        if (IsFacingRight)
        {
            Vector3 rotate = new Vector3(transform.rotation.x,180f,transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotate);
            IsFacingRight = !IsFacingRight;
        }
        else
        {
            Vector3 rotate = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotate);
            IsFacingRight = !IsFacingRight;
        }
    }
    private void TurnCheck()
    {
        //Debug.Log(UserInputs.instance.MoveInput.x);
        if (UserInputs.instance.MoveInput.x > 0 && !IsFacingRight)
        {
            Turn();
        }
        else if (UserInputs.instance.MoveInput.x < 0 && IsFacingRight)
        {
            Turn();
        }
    }
    
    #endregion

    #region Ground/Land Check

    private bool IsGrounded()
    {
        groundhit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size ,0f,Vector2.down,extraHeight, GroundIs);

        if (groundhit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private bool IsLanded()
    {
        if (IsFalling)
        { 
            if (IsGrounded())
            {
                IsFalling = false;
                
                return true;
            }
            else
            {
                return false;
            }
        
        }else
        {
            return false;
        }
    }

    private IEnumerator Reset()
    {
        yield return null;

        amin.ResetTrigger("land");
        amin.ResetTrigger("Dead");
    }

    #endregion
    #region Debug
    private void drawGroundCheck()
    {
        Color raycolor;
        if (IsGrounded())
            raycolor = Color.green;
        else
            raycolor = Color.red;

        Debug.DrawRay(coll.bounds.center + new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), raycolor);
        Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), raycolor);
        Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, coll.bounds.extents.y + extraHeight), Vector2.right * (coll.bounds.extents.y + extraHeight), raycolor);

    }
    #endregion
}
