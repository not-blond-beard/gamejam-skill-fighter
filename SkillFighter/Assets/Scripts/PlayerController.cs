using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{


    [Header("Character")]
    public float WalkSpeed = 1000;
    public float JumpPower = 500;
    public int MaxHealth = 100;
    public int MaxJumpStep = 2;
    public int CurrentHealth { get; private set; }
    public bool IsDead { get { return this.CurrentHealth <= 0; } }

    public ClericWeapon weapon;
    public Transform LaunchPoint;
    public WeaponEffect spell;
    public Transform SpellPoint;
    private WeaponEffect runningEffect;

    // properties
    private Animator animatorObject;
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private Vector2 boxCastSize;
    private float boxCastMaxDistance = 0.5f;

    // character move

    private int currentJumpStep = 0;
    private enum Direction
    {
        Left,
        Right
    }
    private Direction currentDirection = Direction.Right;

    public Vector2 movementInput;
    public bool jumped;
    public bool attacked;

    void Start()
    {
        this.animatorObject = this.GetComponent<Animator>();
        this.rigidBody = this.GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();

        this.boxCastSize = new Vector2(0.5f, 0.4f);

        this.CurrentHealth = this.MaxHealth;

        this.animatorObject.SetBool("IsGrounded", true);
    }

    private void Awake()
    {

    }

    public void OnMove(InputAction.CallbackContext ctx) {
        movementInput = ctx.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext ctx) {
        jumped = ctx.action.triggered;
    }


    public void OnAttack(InputAction.CallbackContext ctx) {
        attacked = ctx.action.triggered;
    }


    private void Move() {
        float horizontal = movementInput.x;
        float nextHorizontal = horizontal * this.WalkSpeed * Time.deltaTime;

        this.rigidBody.velocity = new Vector2(horizontal * this.WalkSpeed * Time.deltaTime, this.rigidBody.velocity.y);

        if (this.currentJumpStep == 0)
        {
            this.animatorObject.SetFloat("VelocityX", Mathf.Abs(this.rigidBody.velocity.x));
        }


        if (!Mathf.Approximately(this.rigidBody.velocity.x, 0))
        {
            this.FlipPlayer(this.rigidBody.velocity.x < 0 ? Direction.Left : Direction.Right);
        }
    }

    private void FlipPlayer(Direction nextDirection)
    {
        if (this.currentDirection == nextDirection)
        {
            return;
        }


        Vector3 scale = this.transform.localScale;
        scale.x = -scale.x;

        this.transform.localScale = scale;
        this.currentDirection = nextDirection;
    }

    private void Jump()
    {
        if (this.currentJumpStep < this.MaxJumpStep)
        {
            this.rigidBody.AddForce(new Vector2(0, this.JumpPower));
            this.currentJumpStep = this.currentJumpStep + 1;

            if (animatorObject.GetBool("IsGrounded"))
            {
                this.animatorObject.SetBool("IsGrounded", false);
            }

            if (animatorObject.GetFloat("VelocityX") > 0)
            {
                this.animatorObject.SetFloat("VelocityX", 0);
            }
        }
    }


    void OnDrawGizmos()
    {

        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, this.boxCastSize, 0f, Vector2.down, this.boxCastMaxDistance, LayerMask.GetMask("Ground"));

        Gizmos.color = Color.red;
        if (raycastHit.collider != null)
        {
            Gizmos.DrawRay(transform.position, Vector2.down * raycastHit.distance);
            Gizmos.DrawWireCube(transform.position + Vector3.down * raycastHit.distance, this.boxCastSize);
        }
        else
        {
            Gizmos.DrawRay(transform.position, Vector2.down * this.boxCastMaxDistance);
        }
    }

    private bool IsOnGround()
    {

        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, this.boxCastSize, 0f, Vector2.down, this.boxCastMaxDistance, LayerMask.GetMask("Ground"));
        return (raycastHit.collider != null);
    }

    private void GroundChecker()
    {
        if (this.rigidBody.velocity.y < 0 && this.currentJumpStep != 0)
        {
            if (this.IsOnGround())
            {
                this.currentJumpStep = 0;
                this.animatorObject.SetBool("IsGrounded", true);
            }
        }
    }

    private bool IsAbleSpecialEffect()
    {
        AnimatorStateInfo animationStateInfo = this.animatorObject.GetCurrentAnimatorStateInfo(0);

        return (animationStateInfo.IsName("Idle") || animationStateInfo.IsName("Walk") || animationStateInfo.IsName("Jump"));
    }

    void Update()
    {
        if (this.IsDead)
        {
            return;
        }

        this.rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        this.Move();


        if (jumped)
        {
            this.Jump();
        }
        // else if (this.IsAbleSpecialEffect() && Input.GetKeyDown(KeyCode.Q))
        // {
        //     this.animatorObject.SetTrigger("TriggerAttack");
        // }
        else if (this.IsAbleSpecialEffect() && attacked)
        {
            this.animatorObject.SetTrigger("TriggerQuickAttack");
        }
        // else if (this.IsAbleSpecialEffect() && Input.GetKeyDown("j"))
        // {
        //     this.animatorObject.SetBool("TriggerCast", true);
        // }
    }

    void FixedUpdate()
    {
        this.GroundChecker();
    }

    private void OnCastEffect()
    {
        if (this.animatorObject.GetCurrentAnimatorStateInfo(0).IsName("Spell"))
        {
            this.runningEffect = WeaponEffect.Create(this.spell, this.SpellPoint);
        }
    }

    private void OnCastComplete()
    {
        if (this.runningEffect != null)
        {
            this.runningEffect.Stop();
        }

        if (!this.animatorObject.GetCurrentAnimatorStateInfo(0).IsName("Spell"))
        {
            ClericWeapon weapon;

            weapon = this.weapon;

            if (weapon != null)
            {
                ClericWeapon.Create(
                    weapon,
                    this.LaunchPoint,
                    (this.currentDirection == Direction.Left ? -1 : 1));
            }
        }
    }
}
