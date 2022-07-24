using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class SpriteCharacterController : MonoBehaviour
{
    // Editor Properties
    [Header("Character")]
    public Transform GroundChecker;
    public LayerMask GroundLayer;
    public float WalkSpeed = 30;
    public float JumpPower = 100;
    public int MaxHealth = 100;

    [Header("Weapon")]
    public Transform LaunchPoint;
    public WeaponProjectile Projectile;
    public Transform EffectPoint; 
    public WeaponEffect Effect;
    public WeaponProjectile SpellProjectile;

    [Header("Demo")]
    public KeyCode SpellKey = KeyCode.S;

    // Script Properties
    public int CurrentHealth { get; private set; }
    public bool IsDead { get { return this.CurrentHealth <= 0; } }

    // Members
    private Animator animatorObject;
    private Rigidbody2D body;
    private bool isGrounded = true;
    private float groundRadius = 0.04f;
    private Direction currentDirection = Direction.Right;
    private WeaponEffect activeEffect;

    private enum Direction
    {
        Left,
        Right
    }

    void Start()
    {
        // Grab the editor objects
        this.animatorObject = this.GetComponent<Animator>();
        this.body = this.GetComponent<Rigidbody2D>();

        // Setup the character
        this.CurrentHealth = this.MaxHealth;
        this.ApplyDamage(0);
    }

    void FixedUpdate()
    {
        if (this.animatorObject != null)
        {
            this.isGrounded = Physics2D.OverlapCircle(GroundChecker.position, this.groundRadius, this.GroundLayer);
            this.animatorObject.SetBool("IsGrounded", this.isGrounded);

            if (this.animatorObject.GetCurrentAnimatorStateInfo(0).IsName("Stopped") ||
                this.animatorObject.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                float horizontal = Input.GetAxis("Horizontal");
                this.body.velocity = new Vector2(horizontal * this.WalkSpeed * Time.deltaTime, this.body.velocity.y);
                this.animatorObject.SetFloat("VelocityX", Mathf.Abs(this.body.velocity.x));

                if (!Mathf.Approximately(this.body.velocity.x, 0))
                {
                    this.ChangeDirection(this.body.velocity.x < 0 ? Direction.Left : Direction.Right);
                }
            }
        }
    }

    void Update()
    {
        // Check for keyboard input for the different actions
        // Nut only when we are on the ground
        if (this.isGrounded && !this.IsDead)
        {
            if (Input.GetButtonDown("Jump"))
            {
                this.animatorObject.SetTrigger("TriggerJump");
                this.body.AddForce(new Vector2(0, this.JumpPower));
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                this.TriggerAction("TriggerQuickAttack");
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                this.TriggerAction("TriggerAttack");
            }
            else if (Input.GetButtonDown("Fire3"))
            {
                this.TriggerAction("TriggerCast");
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                this.ApplyDamage(10);
            }
            else if (Input.GetKeyDown(this.SpellKey))
            {
                this.TriggerAction("TriggerSpell");
            }
        }
    }

    /// <summary>
    /// Reduce the health of the character by the specified amount
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>True if the character dies from this damage, False if it remains alive</returns>
    public bool ApplyDamage(int damage)
    {
        if (!this.IsDead)
        {
            // Update the health
            this.CurrentHealth = Mathf.Clamp(this.CurrentHealth - damage, 0, this.MaxHealth);
            this.animatorObject.SetInteger("Health", this.CurrentHealth);

            if (this.activeEffect != null)
            {
                // Stop playing the spell effect
                this.activeEffect.Stop();
            }

            if (damage != 0)
            {
                // Show the hurt animation
                this.TriggerAction("TriggerHurt");
            }

            if (this.CurrentHealth <= 0)
            {
                // Since the player is dead, remove the corpse
                StartCoroutine(this.DestroyAfter(2));
            }
        }

        return this.IsDead;
    }

    private void TriggerAction(string action)
    {
        this.animatorObject.SetTrigger(action);

        // Stop the character from moving while we do the animation
        this.body.velocity = new Vector2(0, this.body.velocity.y);
    }

    private void ChangeDirection(Direction newDirection)
    {
        if (this.currentDirection == newDirection)
        {
            return;
        }

        // Swap the direction of the sprites
        Vector3 scale = this.transform.localScale;
        scale.x = -scale.x;
        this.transform.localScale = scale;
        this.currentDirection = newDirection;
    }

    private void OnCastEffect()
    {
        // If we have an effect start it now, but only if casting a spell
        if (this.animatorObject.GetCurrentAnimatorStateInfo(0).IsName("Spell"))
        {
            if (this.Effect != null)
            {
                this.activeEffect = WeaponEffect.Create(this.Effect, this.EffectPoint);
            }
        }
    }

    private void OnCastComplete()
    {
        // Stop the active effect once we cast
        if (this.activeEffect != null)
        {
            this.activeEffect.Stop();
        }

        // Create the projectile
        WeaponProjectile projectile;
        if (this.animatorObject.GetCurrentAnimatorStateInfo(0).IsName("Spell"))
        {
            projectile = this.SpellProjectile;
        }
        else
        {
            projectile = this.Projectile;
        }

        if (projectile != null)
        {
            WeaponProjectile.Create(
                projectile,
                this,
                this.LaunchPoint,
                (this.currentDirection == Direction.Left ? -1 : 1));
        }
    }

    private IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        GameObject.Destroy(this.gameObject);
    }
}
