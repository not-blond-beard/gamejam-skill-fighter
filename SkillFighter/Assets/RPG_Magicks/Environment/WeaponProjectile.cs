using UnityEngine;
using System.Collections;

public abstract class WeaponProjectile : MonoBehaviour
{
    // Editor Properties
    [Header("Weapon")]
    public int Damage = 50;

    // Members
    protected int directionX { get; private set; }

    public static WeaponProjectile Create(WeaponProjectile instance, SpriteCharacterController owner, Transform launchPoint, int directionX)
    {
        WeaponProjectile projectile = GameObject.Instantiate<WeaponProjectile>(instance);

        // Prevent hitting the player who cast it
        Physics2D.IgnoreCollision(owner.GetComponent<Collider2D>(), projectile.GetComponent<Collider2D>());

        // Set the start position
        Vector2 position = launchPoint.position;
        projectile.transform.position = position;
        projectile.directionX = directionX;

        // Flip the sprite if necessary
        if (directionX < 0)
        {
            Vector3 scale = projectile.transform.localScale;
            scale.x = -scale.x;
            projectile.transform.localScale = scale;
        }

        return projectile;
    }

    protected virtual void Start()
    {
        // Get rid of the projectile after a while if it doesn't hit anything
        StartCoroutine(this.DestroyAfter(3));
    }

    protected virtual void Update()
    {
    }

    protected virtual void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject.Destroy(this.gameObject);

        // Apply damage to any character hit by this projectile
        SpriteCharacterController character = coll.transform.GetComponent<SpriteCharacterController>();
        if (character != null)
        {
            character.ApplyDamage(this.Damage);
        }
    }

    protected IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        GameObject.Destroy(this.gameObject);
    }
}
