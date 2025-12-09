using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("Preferences")]
    public PlayerMovementStats movementStats;
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;

    private Animator anim; 
    private Rigidbody2D _rb;

    private Vector2 _moveVelocity;
    private bool _isFacingRight;

    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private bool _isGrounded;
    private bool _bumpedHead;


    // jump vars
    public float VerticalVelocity { get; private set; }
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    // apex vars
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    // jump buffer vars
    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;

    // coyote time vars
    private float _coyoteTimer;



    private void Awake()
    {
        _isFacingRight = true;

        _rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        CountTimer();
        JumpCheck();
    }

    private void FixedUpdate()
    {
        CollisionCheck();
        Jump();

        if (_isGrounded)
        {
            Move(movementStats.groundAcceleration, movementStats.groundDeceleration, InputManager.Movement);
        }
        else
        {
            Move(movementStats.airAcceleration, movementStats.airDeceleration, InputManager.Movement);
        }
    }

    private void OnDrawGizmos()
    {
        if (movementStats.ShowWalkJumpArc)
        {
            DrawJumpArc(movementStats.maxWalkSpeed, Color.white);
        }
        if (movementStats.ShowRunJumpArc)
        {
            DrawJumpArc(movementStats.maxRunSpeed, Color.yellow);
        }

    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        
        if (moveInput != Vector2.zero)
        {
            anim.SetBool("IsMove",true);
            TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            if (InputManager.RunHeld)
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * movementStats.maxRunSpeed;
            }
            else
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * movementStats.maxWalkSpeed;
            }
            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
        }
        else if (moveInput == Vector2.zero)
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
            
            anim.SetBool("IsMove", false);
            
        }
    }

    #endregion

    #region Jump

    private void JumpCheck()
    {
        // Jump btn release
        if (InputManager.jumpPressed)
        {
            _jumpBufferTimer = movementStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }
        if (InputManager.jumpReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleasedDuringBuffer = true;

            }

            if (_isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = movementStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        // Jump btn pressed
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitialJump(1);
            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        //double jump
        else if (_jumpBufferTimer > 0f && !_isJumping && _numberOfJumpsUsed < movementStats.NumberOfJumpsAllowed)
        {
            _isFastFalling = false;
            InitialJump(1);
        }

        //air jump
        else if (_jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < movementStats.NumberOfJumpsAllowed -1)
        {
            InitialJump(2);
            _isFastFalling = false;
        }

        //landed
        if ((_isJumping || _isFalling) && _isGrounded && VerticalVelocity <= 0f)
        {
            _isJumping = false;
            _isFalling = false;
            _isFastFalling = false;
            _isPastApexThreshold = false;
            VerticalVelocity = 0f;
            _numberOfJumpsUsed = 0;
            _fastFallTime = 0f;

            VerticalVelocity = Physics2D.gravity.y;
            Landed();
        }
    }

        

    private void InitialJump(int numberOfJumpsUsed)
    {
        if(!_isJumping)
        {
            _isJumping = true;
        }

        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed+= numberOfJumpsUsed;
        VerticalVelocity = movementStats.InitialJumpVelocity;
        anim.SetTrigger("jump");
    }

    private void Jump()
    {
        //grav for jump
        if (_isJumping)
        {
            // check head bump
            if (_bumpedHead)
            {
                _isFastFalling = true;
                Falling();
            }

            //grav on up
            if (VerticalVelocity >= 0f)
            {
                //apex control
                _apexPoint = Mathf.InverseLerp(movementStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (_apexPoint > movementStats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;

                    }
                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < movementStats.ApexHangTime)
                        {
                            VerticalVelocity += 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }

                //grav on up but not pass apex point/threshold
                else
                {
                    VerticalVelocity += movementStats.Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }
            }

            //grav on down
            else if (!_isFastFalling)
            {
                VerticalVelocity += movementStats.Gravity * movementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (VerticalVelocity < 0f )
            {
                if (!_isFastFalling)
                {
                    _isFastFalling = true;
                    Falling();
                }
            }
        }

        //jump cut
        if (_isFastFalling)
        {
            if (_fastFallTime >= movementStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += movementStats.Gravity * movementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / movementStats.TimeForUpwardsCancel));

            }
            _fastFallTime += Time.fixedDeltaTime;
        }

        //falling grav
        if (!_isGrounded && !_isJumping)
        {
            if (!_isFalling)
            {
                _isFalling = true;
                Falling();
            }
            VerticalVelocity += movementStats.Gravity * Time.fixedDeltaTime;
        }

        //cap fall speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -movementStats.MaxFallSpeed,50f);

        _rb.linearVelocity = new Vector2(_rb.linearVelocityX, VerticalVelocity);

    }

    #endregion

    #region Turn
    private void TurnCheck(Vector2 moveInput)
    {
        if (_isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!_isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }
    private void Turn(bool faceRight)
    {
        if (faceRight)
        {
            _isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }else
        {
            _isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }
    #endregion

    #region Collisions Checks

    private void IsGrounded()
    {
        Vector2 boxCastOriigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, movementStats.groundCheckDistance);

        _groundHit = Physics2D.BoxCast(boxCastOriigin, boxCastSize, 0f, Vector2.down,movementStats.groundCheckDistance, movementStats.groundLayer);
        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }

        # region Visualized

        if (movementStats.DebugShowIsGroundedBox)
        {
            Color boxColor;
            if (_isGrounded)
            {
                boxColor = Color.green;
            }
            else
            {
                boxColor = Color.red;
            }
            Debug.DrawRay(new Vector2(boxCastOriigin.x - boxCastSize.x / 2, boxCastOriigin.y), Vector2.down * movementStats.groundCheckDistance, boxColor);
            Debug.DrawRay(new Vector2(boxCastOriigin.x + boxCastSize.x / 2, boxCastOriigin.y), Vector2.down * movementStats.groundCheckDistance, boxColor);
            Debug.DrawRay(new Vector2(boxCastOriigin.x - boxCastSize.x / 2, boxCastOriigin.y - movementStats.groundCheckDistance), Vector2.right * boxCastSize.x, boxColor);
            
        }

        #endregion
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x * movementStats.HeadWidth, movementStats.headDetectDistance);

        RaycastHit2D _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, movementStats.headDetectDistance, movementStats.groundLayer);

        if (_headHit.collider != null)
        {
            _bumpedHead = true;
        }
        else { _bumpedHead = false; }

        #region Debug Visualization

        if (movementStats.DebugShowHeadBumpBox)
        {
            float headWidth = movementStats.HeadWidth;

            Color rayColor;
            if (_bumpedHead)
            {
                rayColor = Color.green;
            }
            else { rayColor = Color.red; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * movementStats.headDetectDistance, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * movementStats.headDetectDistance, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + movementStats.headDetectDistance), Vector2.right * boxCastSize.x * headWidth, rayColor);
        }

        #endregion
    }

    private void CollisionCheck()
    {
        IsGrounded();
        BumpedHead();
    }

    #endregion

    #region Timer

    private void CountTimer()
    {
        _jumpBufferTimer -= Time.fixedDeltaTime;
        if (!_isGrounded)
        {
            _coyoteTimer -= Time.fixedDeltaTime;
        }else
        {
            _coyoteTimer = movementStats.JumpCoyoteTime;
        }
    }
    #endregion

    #region draw Debug Gizmos

    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPosition = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 previousPosition = startPosition;

        float speed = 0f;

        if (movementStats.DrawRight)
        {
            speed = moveSpeed;
        }
        else
        {
            speed = -moveSpeed;
        }

        Vector2 velocity = new Vector2(speed, movementStats.InitialJumpVelocity);

        Gizmos.color = gizmoColor;

        float timeStep = 2 * movementStats.TimeTillJumpApex / movementStats.ArcResolution;
        // float totalTime = (2 * movementStats.TimeTillJumpApex) + movementStats.ApexHangTime; // total time of the arc including hang time

        for (int i = 0; i < movementStats.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if (simulationTime < movementStats.TimeTillJumpApex)  // Ascending
            {
                displacement = velocity * simulationTime +
                               0.5f * new Vector2(0, movementStats.Gravity) *
                               simulationTime * simulationTime;
            }
            else if (simulationTime < movementStats.TimeTillJumpApex + movementStats.ApexHangTime) // Apex hang time
            {
                float apexTime = simulationTime - movementStats.TimeTillJumpApex;

                displacement = velocity * movementStats.TimeTillJumpApex +
                               0.5f * new Vector2(0, movementStats.Gravity) *
                               movementStats.TimeTillJumpApex * movementStats.TimeTillJumpApex;

                displacement += new Vector2(speed, 0) * apexTime; // No vertical movement during hang time
            }
            else // Descending
            {
                float descendTime = simulationTime - (movementStats.TimeTillJumpApex + movementStats.ApexHangTime);

                displacement = velocity * movementStats.TimeTillJumpApex +
                               0.5f * new Vector2(0, movementStats.Gravity) *
                               movementStats.TimeTillJumpApex * movementStats.TimeTillJumpApex;

                displacement += new Vector2(speed, 0) * movementStats.ApexHangTime; // horizontal movement during hang time

                displacement += new Vector2(speed, 0) * descendTime +
                               0.5f * new Vector2(0, movementStats.Gravity) *
                               descendTime * descendTime;
            }

            drawPoint = startPosition + displacement;

            if (movementStats.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(
                    previousPosition,
                    drawPoint - previousPosition,
                    Vector2.Distance(previousPosition, drawPoint),
                    movementStats.groundLayer
                );

                if (hit.collider != null)
                {
                    // If a hit is detected, stop drawing the arc at the hit point
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }

            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }


    #endregion

    #region animation trigger
    
    public void Falling()
    {
        anim.SetBool("IsFall",true);
        anim.ResetTrigger("jump");
    }

    public void Landed()
    {
        anim.SetBool("IsFall", false);
        anim.SetTrigger("land");
        anim.ResetTrigger("land");
    }


    #endregion
}
