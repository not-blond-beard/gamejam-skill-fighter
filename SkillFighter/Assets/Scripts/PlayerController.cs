using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{


    [Header("Character")]
    public float WalkSpeed = 300;
    public float JumpPower = 500;
    public int MaxHealth = 100;
    public int MaxJumpStep = 2;
    public int CurrentHealth { get; private set; }
    public bool IsDead { get { return this.CurrentHealth <= 0; } }

    public ClericWeapon weapon;
    public Transform LaunchPoint;

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
    

    void Start()
    {
        
        this.animatorObject = this.GetComponent<Animator>();
        this.rigidBody = this.GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();

        this.boxCastSize = new Vector2(0.5f, 0.4f);
    
        this.CurrentHealth = this.MaxHealth;
    }

    private void Awake() {
        
    }


    private void Move() {
        float horizontal = Input.GetAxis("Horizontal");
        float nextHorizontal = horizontal * this.WalkSpeed * Time.deltaTime;

        this.rigidBody.velocity = new Vector2(horizontal * this.WalkSpeed * Time.deltaTime, this.rigidBody.velocity.y);

        if (this.currentJumpStep == 0) {
            this.animatorObject.SetFloat("VelocityX", Mathf.Abs(this.rigidBody.velocity.x));
        }

        
        if (!Mathf.Approximately(this.rigidBody.velocity.x, 0))
        {
            this.FlipPlayer(this.rigidBody.velocity.x < 0 ? Direction.Left : Direction.Right);
        }
    }

    private void FlipPlayer(Direction nextDirection) {
        if (this.currentDirection == nextDirection) {
            return;
        }

        
        Vector3 scale = this.transform.localScale;
        scale.x = -scale.x;

        this.transform.localScale = scale;
        this.currentDirection = nextDirection;
    }
    
    private void Jump() {
        if (Input.GetKeyDown(KeyCode.Space) && this.currentJumpStep < this.MaxJumpStep) {
            this.rigidBody.AddForce(new Vector2(0, this.JumpPower));
            this.currentJumpStep = this.currentJumpStep + 1;

            if (!animatorObject.GetBool("IsJumping")) {
                this.animatorObject.SetBool("IsJumping", true);
            }

            if (animatorObject.GetFloat("VelocityX") > 0) {
                this.animatorObject.SetFloat("VelocityX", 0);
            }
        }
    }
    
    void Update()
    {
        this.rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (!this.IsDead) {
            this.Move();
            this.Jump();

            if (Input.GetKeyDown("j")) {
                this.animatorObject.SetBool("TriggerCast", true);
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

    private void GroundChecker() {
        if (this.rigidBody.velocity.y < 0 && this.currentJumpStep != 0) {
            if (this.IsOnGround()) {
                this.currentJumpStep = 0;
                this.animatorObject.SetBool("IsJumping", false);
            }
        }
    }

    void FixedUpdate() 
    {   
        this.GroundChecker();
    }

    private void OnCastEffect()
    {
        
        Debug.Log("test1");
    }

    private void OnCastComplete()
    {
        Debug.Log("test2");

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
