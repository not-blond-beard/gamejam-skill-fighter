using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{


    [Header("Character")]
    public float WalkSpeed = 1000;
    public float JumpPower = 500;
    public int MaxHealth = 100;
    public int CurrentHealth { get; private set; }
    public bool IsDead { get { return this.CurrentHealth <= 0; } }

    // properties
    private Animator animatorObject;
    private Rigidbody2D rigidBody;

    // character move
    private bool isJumping = false;
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
        
        this.CurrentHealth = this.MaxHealth;
    }

    private void Awake() {
        
    }


    private void Move() {
        float horizontal = Input.GetAxis("Horizontal");

        this.rigidBody.velocity = new Vector2(horizontal * this.WalkSpeed * Time.deltaTime, this.rigidBody.velocity.y);
        this.animatorObject.SetFloat("VelocityX", Mathf.Abs(this.rigidBody.velocity.x));
    }
    
    private void Jump() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            this.isJumping = true;

            this.animatorObject.SetBool("IsJumping", this.isJumping);
            this.rigidBody.AddForce(new Vector2(0, this.JumpPower));
        }
    }
    
    void Update()
    {
        this.rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (!this.IsDead) {
            this.Move();
            this.Jump();
        }
    }

    private bool IsOnGround()
    {
            return true;
    }

    void FixedUpdate() 
    {   
        Debug.Log(IsOnGround());
        
        if (IsOnGround() && this.isJumping) {
            this.isJumping = false;
            this.animatorObject.SetBool("IsJumping", this.isJumping);
        }
    }
}
