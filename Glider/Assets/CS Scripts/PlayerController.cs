using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Problems:
    /* 
     * 1.) We don't want the player to be able to turn around during the roll or dash (Solved)
     * 2.) We don't want the player to be able to spam and buffer dashes or rolls (Solved)
     * 5.) Fixed bugs while moving on ground (Solved)
     */

    [SerializeField] private float horizontalMovementSpeed = 2f; //dictates horizontal movement speed in inspector GROUNDED MOVEMENT ONLY
    [SerializeField] private float aerialHorizontalMovementSpeed = 2f; //dictates aerial horizontal movement speed in inspector AERIAL MOVEMENT ONLY
    [SerializeField] private float horizontalSpeedLimit = 10f; //running speed limit
    [SerializeField] private float friction = 20f;
    [SerializeField] private float crouchFriction = 40f;
    [SerializeField] private float groundSlamFriction = 100f;
    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private float dashSpeedLimit = 15f;
    [SerializeField] private float rollSpeed = 0.2f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float fastFallSpeed = 20f;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private int dashNumberLimit = 1;
    [SerializeField] private int doubleJumpNumberLimit = 1;
    [SerializeField] private float doubleClickDownTime = 0.5f;
    [SerializeField] private float rayCastDistance = 5f;
    [SerializeField] private float fastFallCancelRollSpeed = 20f;

    //keyboard input variables
    private float moveHorizontal; //input to move left and right (value is +1/-1 at right and left respectively)
    private float crouchLeftAndRight; //input to flip sprite left and right while crouching (this was separated from moveHorizontal to separatize and simplify)
    private float inputUpAndDown; //input for looking up and crouching
    private bool rollClick = false; //input for if you click the roll button
    private bool dashClick = false; //input for if you click the dash button (this only triggers the animation and not the physics event)
    private bool singleJumpClick = false; //input for single jumping
    private bool doubleJumpClick = false; //input for double jumping
    private bool fastFallDoubleClick = false;
    private bool fastFallCancelRollClick = false;

    //Game Object Components
    private Rigidbody2D myRigidBody2D;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    private BoxCollider2D myBoxCollider2D; 

    //Animator String Literals
    private const string IS_RUNNING = "isRunning";
    private const string IS_CROUCHING = "isCrouching";
    private const string TRIGGER_ROLL = "triggerRoll";
    private const string IS_UP_LOOKING = "isUpLooking";
    private const string IS_SINGLE_JUMPING = "isSingleJumping";
    private const string IS_DOUBLE_JUMPING = "isDoubleJumping";
    private const string TRIGGER_DASH = "triggerDash";
    private const string TRIGGER_NTH_JUMP = "triggerNthJump";
    private const string IS_FAST_FALLING = "isFastFalling";
    //private const string TRIGGER_GROUND_SLAM = "triggerGroundSlam";
    //private const string IS_FAST_FALL_CANCELING = "isFastFallCanceling";
    //private const string FAST_FALL_CANCEL = "fastFallCancel";

    //AnimationBinding for booleans
    private bool isRunning = false;
    private bool isUpLooking = false;
    private bool isCrouching = false;
    private bool isDoubleJumping = false;
    private bool isFastFalling = false;
    private bool isFastFallCanceling = false;

    //Animation Event Variables
    private bool isDashAnimationEvent = false;
    private bool isRollAnimationEvent = false;
    private bool isGroundSlamAnimationEvent = false;

    //Counter variables
    private int numberOfDashes = 0;
    private int numberOfDoubleJumps = 0;

    //timer variables
    private float lastClickDownTime;

    //Vector2
    private Vector2 zeroVector = new Vector2(0f, 0f);

    //RayCast
    RaycastHit2D rayCastHit2D;


    private void Awake()
    {
        myRigidBody2D = this.GetComponent<Rigidbody2D>();
        myAnimator = this.GetComponent<Animator>();
        mySpriteRenderer = this.GetComponent<SpriteRenderer>();
        myBoxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!LevelController.IsGamePaused()) //if the game is not paused then we want to check for input
        {
            InputManager();
            AnimatorManager();

            if (isDashAnimationEvent) { EnforceDashSpeedLimit(); }
            else { EnforceSpeedLimit(); }

            CrouchFriction();

            if (IsGrounded())
            {
                ResetDashCount();
                ResetDoubleJumpCount();
            }
        }
    }

    //****************************************************************************
    //ANIMATION + INPUT
    //This will manage our animations in an attempt to separate movement mechanics from animations
    private void AnimatorManager()
    {
        myAnimator.SetBool(IS_RUNNING, isRunning); //run animation handling
        myAnimator.SetBool(IS_CROUCHING, isCrouching); //crouch animation handling
        myAnimator.SetBool(IS_UP_LOOKING, isUpLooking); //upLooking animation handling

        if (rollClick) { myAnimator.SetTrigger(TRIGGER_ROLL); } //roll animation handling
        if (dashClick) { myAnimator.SetTrigger(TRIGGER_DASH); } //dash animation handling

        //Single Jump Animation Handling
        if (IsGrounded()) { myAnimator.SetBool(IS_SINGLE_JUMPING, false); }
        else { myAnimator.SetBool(IS_SINGLE_JUMPING, true); }

        //Nth Jump Animation Handling (this has to go before the Double Jump Animation Handling because the isDoubleJumping boolean is being set to false)
        if (isDoubleJumping && numberOfDoubleJumps > 0 && numberOfDoubleJumps <= doubleJumpNumberLimit)
        {
            myAnimator.SetTrigger(TRIGGER_NTH_JUMP);
        }

        //Double Jump Animation Handling
        if (isDoubleJumping) { myAnimator.SetBool(IS_DOUBLE_JUMPING, true); isDoubleJumping = false; }
        if (IsGrounded() || dashClick) { myAnimator.SetBool(IS_DOUBLE_JUMPING, false); isDoubleJumping = false; }

        //GroundSlam/FastFall Animation Handling
        //Order of operations for this goes Input -> Animator -> Physics
        //I found that the physics call was interrupting very low ground slams and so going straight to animation from input means you can ground slam from lower
        if (fastFallDoubleClick) { myAnimator.SetBool(IS_FAST_FALLING, true); } //handles ground slam part 1
        if(IsGrounded()) {
            //if we fast fall cancel roll click then do this
            //IMPORTANT NOTE: The logic flow for this is INPUT -> ANIMATOR -> PHYSICS
            if (fastFallCancelRollClick) 
            { 
                myAnimator.SetTrigger(TRIGGER_ROLL); 
                fastFallCancelRollClick = false;
                isFastFallCanceling = true;
            }
            //if we are grounded and fastfalling is still true we will intitiate groundshake
            //fastFalling will be set to false right after so that this method can only be called at one frame and not continuously
            //Singleton is for ease of access
            else if (isFastFalling && !isFastFallCanceling)
            {
                CameraShake.Instance.ShakeCamera();
                myAnimator.SetBool(IS_FAST_FALLING, false);
                isFastFalling = false;
            }
        } //handles ground slam part 2 and ground slam exit
    }

    //This will manage all input from the keyboard
    private void InputManager()
    {
        if (inputUpAndDown == 0) //this if statement will prevent the user from being able to move left and right if they are holding up or down
        {
            //this if statement will prevent the user from inputting movement while dashing or rolling
            if (isDashAnimationEvent == false && isRollAnimationEvent == false && isGroundSlamAnimationEvent == false)
            {
                //grounded horizontal movement (input, sprite, animationBinding boolean)
                moveHorizontal = Input.GetAxisRaw("Horizontal");
                SpriteFlip(moveHorizontal); //flip sprite to left if necessary
                if (moveHorizontal != 0) { isRunning = true; }
                else { isRunning = false; }
            }
        }

        if (moveHorizontal == 0) //this if statement will prevent the user from being able to crouch or look up if they are holding left or right
        {
            //crouch and upLooking management
            inputUpAndDown = Input.GetAxisRaw("Vertical");
            if (inputUpAndDown > 0 && IsGrounded()) { isUpLooking = true; }
            else { isUpLooking = false; }
            if (inputUpAndDown < 0 && IsGrounded()) {
                isCrouching = true;
                crouchLeftAndRight = Input.GetAxisRaw("Horizontal"); //will get left and right input to flip sprite ONLY when crouching
                SpriteFlip(crouchLeftAndRight); //flips sprite while crouching
            }
            else { isCrouching = false; }
        }

        //rolling management (Important: rolling can only be done if you are running and grounded)
        if (IsGrounded() && isRunning && isDashAnimationEvent == false && isFastFalling == false)
        {
            //input can only be checked if the animation has NOT begun yet (this prevents player from spamming and buffering)
            if (isRollAnimationEvent == false && Input.GetKeyDown("left ctrl")) { rollClick = true; }
            else { rollClick = false; }
        }

        //dashing management (Important: dashing can only occur if running or being airborne)
        //the dashNumberLimit is meant to set an aerial dash limit so you cannot dash infinitely in the air
        //on the ground however, you are free to dash as much as you like
        if (numberOfDashes < dashNumberLimit)
        {
            //We had issues with dash input being read during the fast fall
            //It turns out I had to check two booleans because one is activated in the inputManager (fastFallDoubleClick) and the other in the FixedUpdate (isFastFalling)
            //This creates sloppy code and requires revising for cleaner code
            //Mathf.Abs(moveHorizontal) was added because we were able to dash in the air without pressing left or right, causing stationary dashes
            if ((isRunning && isRollAnimationEvent == false && (isFastFalling == false || fastFallDoubleClick == false) && isGroundSlamAnimationEvent == false) || (Mathf.Abs(moveHorizontal) == 1 && !IsGrounded()))
            {
                //input can only be checked if the animation has NOT begun yet (this prevents player from spamming and buffering)
                if (isDashAnimationEvent == false && Input.GetMouseButtonDown(0)) { dashClick = true; numberOfDashes++; }
                else { dashClick = false; } //prevents multiple dash bug on the ground
            }
        }
        else { dashClick = false; } //prevents multiple dash bug in the air

        //single jump input
        //we only want to single jump if we are grounded, press space, not dashing, not ground-slamming
        if (IsGrounded() && Input.GetKeyDown("space") && isGroundSlamAnimationEvent == false && isDashAnimationEvent == false && isCrouching == false && isUpLooking == false && isRollAnimationEvent == false) 
        { 
            singleJumpClick = true;
        }

        //double jump input as well as Nth jump input
        //can't jump while fastFalling or dashing
        if (numberOfDoubleJumps < doubleJumpNumberLimit && isFastFalling == false && isDashAnimationEvent == false && isRollAnimationEvent == false)
        {
            if (isDoubleJumping == false && !IsGrounded() && Input.GetKeyDown("space")) { doubleJumpClick = true; numberOfDoubleJumps++; }
        }

        
        //fast fall management (double click 's')
        //ground slam
        if(!IsGrounded() && isDashAnimationEvent == false)
        {
            if(Input.GetKeyDown(KeyCode.S))
            {
                float timeSinceLastClickDown = Time.time - lastClickDownTime;

                if(timeSinceLastClickDown <= doubleClickDownTime)
                {
                    fastFallDoubleClick = true;
                }

                lastClickDownTime = Time.time;
            }
            
        }

        //fast fall cancel input checker
        RayCastCheck();
    }

    //FastFallCancel Button Check with RayCast
    private void RayCastCheck()
    {
        rayCastHit2D = Physics2D.Raycast(this.transform.position, Vector2.down, rayCastDistance, groundLayerMask, -Mathf.Infinity, Mathf.Infinity);
        if(rayCastHit2D.collider != null)
        {
            if(Input.GetMouseButtonDown(1) && (isFastFalling || fastFallDoubleClick)) 
            {
                fastFallCancelRollClick = true;
            }
            
        }
    }

    //resets the number of double jumps you have performed
    private void ResetDoubleJumpCount()
    {
        numberOfDoubleJumps = 0;
    }

    //resets the number of dashes you have performed
    private void ResetDashCount()
    {
        numberOfDashes = 0;
    }

    private bool IsGrounded()
    {
        return groundCheck.isGrounded;
    }

    //Apply greater friction to character if they choose to crouch
    //There is a bug with altering Unity's friction dynamically and to overcome this we must disable and enable the collider
    private void CrouchFriction()
    {
        if(inputUpAndDown < 0)
        {
            myBoxCollider2D.sharedMaterial.friction = crouchFriction;
            myBoxCollider2D.enabled = false;
            myBoxCollider2D.enabled = true;
        }
        else
        {
            myBoxCollider2D.sharedMaterial.friction = friction;
            myBoxCollider2D.enabled = false;
            myBoxCollider2D.enabled = true;
        }
    }

    //Enforcing the speed limit constraints on horizontal movement
    private void EnforceSpeedLimit()
    {
        if (myRigidBody2D.velocity.x > horizontalSpeedLimit)
        {
            myRigidBody2D.velocity = new Vector2(horizontalSpeedLimit, myRigidBody2D.velocity.y);
        }
        if (myRigidBody2D.velocity.x < -horizontalSpeedLimit)
        {
            myRigidBody2D.velocity = new Vector2(-horizontalSpeedLimit, myRigidBody2D.velocity.y);
        }
    }

    //Enforcing the speed limit constraints on the dash
    private void EnforceDashSpeedLimit()
    {
        if (myRigidBody2D.velocity.x > dashSpeedLimit)
        {
            myRigidBody2D.velocity = new Vector2(dashSpeedLimit, myRigidBody2D.velocity.y);
        }
        if (myRigidBody2D.velocity.x < -dashSpeedLimit)
        {
            myRigidBody2D.velocity = new Vector2(-dashSpeedLimit, myRigidBody2D.velocity.y);
        }
    }
    //****************************************************************************


    //****************************************************************************
    //PHYSICS
    //Responsible for all Phyics movement calls
    private void FixedUpdate()
    {
        if(isDashAnimationEvent) { DashPhysics(); }
        else if(isFastFallCanceling) { //this needed to be called before the following else if clause because it was triggering that one first 
            FastFallCancelRollPhysics(); 
            //isFastFallCanceling = false; 
            //this is actually being set to false in the rollAnimationEvent function
            //this is so only this else if clause is called each time and not the following one!
            //we want isFastFallCanceling to remain true for the entire duration of this fastFallCancelRoll
            isFastFalling = false;
            myAnimator.SetBool(IS_FAST_FALLING, false);
        }
        else if(isRollAnimationEvent) { RollPhysics(); }
        else if(isGroundSlamAnimationEvent) { GroundSlamPhysics(); }
        else {
            if(IsGrounded()) { MoveLeftAndRightGroundedPhysics(); }
            else if(!IsGrounded()) { MoveLeftAndRightAerialPhysics(); }
            //we will jump if we click the jump button in the InputManager()
            //we actually need to set the jumpClick back to false here because fixedUpdate() is not running synchronously with Update()
            //if we set jumpClick back to false in an else statement in the InputManager(), jumpClick might become false before a FixedUpdate() call
            //meaning we will never execute a jump
            if (singleJumpClick) { 
                JumpPhysics(); 
                singleJumpClick = false; 
            }
            if(doubleJumpClick)
            {
                JumpPhysics();
                doubleJumpClick = false;
                isDoubleJumping = true;
            }
            if(fastFallDoubleClick)
            {
                FastFallPhysics();
                fastFallDoubleClick = false;
                isFastFalling = true;
            }
        }
    }

    private void MoveLeftAndRightGroundedPhysics()
    {
        myRigidBody2D.AddForce(new Vector2(moveHorizontal * horizontalMovementSpeed, 0f)); //move left and right grounded
    }

    private void MoveLeftAndRightAerialPhysics()
    {
        myRigidBody2D.AddForce(new Vector2(moveHorizontal * aerialHorizontalMovementSpeed, 0f)); //move left and right aerial
    }

    private void DashPhysics()
    {
        myRigidBody2D.velocity = new Vector2(myRigidBody2D.velocity.x, 0f); //makes dash go straight ahead in the air rather than fall down
        myRigidBody2D.AddForce(new Vector2(dashSpeed * moveHorizontal, 0f), ForceMode2D.Impulse);

    }

    private void RollPhysics()
    {
        myRigidBody2D.AddForce(new Vector2(rollSpeed * moveHorizontal, 0f), ForceMode2D.Impulse);
    }

    private void FastFallCancelRollPhysics()
    {
        if(mySpriteRenderer.flipX == true)
        {
            myRigidBody2D.AddForce(new Vector2(-fastFallCancelRollSpeed, 0f), ForceMode2D.Impulse);
        }
        else if(mySpriteRenderer.flipX == false)
        {
            myRigidBody2D.AddForce(new Vector2(fastFallCancelRollSpeed, 0f), ForceMode2D.Impulse);
        }
    }

    private void JumpPhysics()
    {
        myRigidBody2D.velocity = new Vector2(myRigidBody2D.velocity.x, 0f); // this makes double jump height consistent
        //the issue before was that if we were falling downwards, the negative y velocity would impact the height of our double jump
        //by setting the y velocity to 0 right before we jump, we now have a consistent double jump height
        myRigidBody2D.AddForce(new Vector2(0f, jumpSpeed), ForceMode2D.Impulse);
    }

    private void FastFallPhysics()
    {
        myRigidBody2D.velocity = new Vector2(0f, 0f); //cuts off all momentum so that the fastfall can go straight down
        myRigidBody2D.AddForce(new Vector2(0f, -fastFallSpeed), ForceMode2D.Impulse);
    }

    private void GroundSlamPhysics()
    {
        myRigidBody2D.velocity = zeroVector; //halts all movement for the entire durationg of the ground slam animation
    }
    //****************************************************************************


    //****************************************************************************
    /*MISCELLANEOUS FUNCTIONS*/
    private void SpriteFlip(float moveHorizontal)
    {
        if(moveHorizontal < 0)
        {
            mySpriteRenderer.flipX = true;
        }
        else if(moveHorizontal > 0)
        {
            mySpriteRenderer.flipX = false;
        }
    }

    //used to communicate to CameraShake script when a groundslam occurs to trigger camera shake
    public bool GetGroundSlameAnimationEvent()
    {
        return isGroundSlamAnimationEvent;
    }

    //this function is called in the player's dashing animation as an animation event
    //if isDashAnimationEvent is true, then we will enforce the dash speed limit in Update()
    //else we will enforce the standard speed limit in Update()
    /*these functions also lock out the player from buffering roll or dash on either movement
     * meaning that if you're dashing, you can't input a roll and once the dash animation is done a roll
     * will be buffered. You must wait and then input the roll once the dash has ended.
     */
    private void SetDashAnimationEvent(int x)
    {
        if(x == 1) { isDashAnimationEvent = true; }
        else { isDashAnimationEvent = false; }
    }

    //Same as above function but for rolling
    private void SetRollAnimationEvent(int x)
    {
        if (x == 1) { isRollAnimationEvent = true; }
        else { 
            isRollAnimationEvent = false;
            isFastFallCanceling = false;
        }
    }

    private void SetGroundSlamAnimationEvent(int x)
    {
        if(x == 1) { isGroundSlamAnimationEvent = true; }
        else { 
            isGroundSlamAnimationEvent = false; 
            myAnimator.ResetTrigger(TRIGGER_DASH); //minor glitch with dash trigger so we reset here to clear the glitch
        }
    }
    //****************************************************************************
}
