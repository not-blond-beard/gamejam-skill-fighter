using UnityEngine;
using System.Collections;

public class HorizontalProjectile : WeaponProjectile
{
    // Editor Properties
    [Header("Projectile")]
    public int Speed = 100;
    public int RotationSpeed = 0;

    // Members
    private Transform renderTransform;

    protected override void Start()
    {
        base.Start();

        // Get the sprite from the child components so that we can rotate it even if we have a slider joint
        SpriteRenderer sprite = this.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            this.renderTransform = sprite.transform;
        }

        // Get the slider joint that prevents any Y movement
        SliderJoint2D joint = this.GetComponent<SliderJoint2D>();
        if (joint != null)
        {
            joint.anchor = new Vector2(this.transform.position.x, -this.transform.position.y);
        }

        // Give it some velocity
        float x = directionX * this.Speed;
        Rigidbody2D body = this.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.AddForce(new Vector2(x, 0));
        }
    }

    protected override void Update()
    {
        base.Update();

        // If we are a rotating projectile, then rotate the rendering part
        if (this.RotationSpeed != 0 && this.renderTransform != null)
        {
            this.renderTransform.Rotate(Vector3.forward, Time.deltaTime * -this.RotationSpeed);
        }
    }
}
