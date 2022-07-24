using UnityEngine;
using System.Collections;

public class BlastProjectile : WeaponProjectile
{

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Apply damage to any character hit by this blast
        SpriteCharacterController character = other.transform.GetComponent<SpriteCharacterController>();
        if (character != null)
        {
            character.ApplyDamage(this.Damage);
        }
    }
}
