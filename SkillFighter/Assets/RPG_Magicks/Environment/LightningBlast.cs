using UnityEngine;
using System.Collections;

public class LightningBlast : BlastProjectile
{
    // Members
    private LineRenderer blastLine;
    private BoxCollider2D blastCollider;

    protected override void Start()
    {
        base.Start();

        // Get the line renderer that draws the blast effect
        this.blastLine = this.GetComponent<LineRenderer>();

        // Get the collider used for collisions
        this.blastCollider = this.GetComponent<BoxCollider2D>();

        // Set the line renderer to draw in the correct world space
        Vector3 start = this.transform.position;
        Vector3 end = this.transform.position;
        end.x += this.directionX * this.blastCollider.size.x;

        this.blastLine.SetPosition(0, start);
        this.blastLine.SetPosition(1, end);

        // Get rid of the blast after a while if it doesn't hit anything
        StartCoroutine(this.DestroyAfter(0.5f));
    }
}
