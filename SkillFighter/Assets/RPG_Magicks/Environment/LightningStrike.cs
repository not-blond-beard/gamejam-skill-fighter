using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningStrike : WeaponEffect
{
    // EditorProperties
    [Header("Strike")]
    public float StrikeHeight = 1;
    public int ControlPoints = 2;
    public float PatternChangeInterval = 0.05f;

    [Header("Effect")]
    public bool ChangeBackgroundColor = true;
    public Color BackgroundColor = Color.black;

    // Members
    private LineRenderer strikeLine;
    private float changeTime;

    protected override void Start()
    {
        base.Start();

        // Get the line renderer that draws the lightning
        this.strikeLine = GetComponent<LineRenderer>();

        // We need to hide it until we have set the control points, 
        // otherwise it will be drawn in the wrong place
        this.strikeLine.enabled = false;

        // Set the new background color if that was chosen
        if (this.ChangeBackgroundColor)
        {
            WeaponEffect.SetBackgroundColor(this.BackgroundColor);
        }
    }

    protected override void Update()
    {
        this.changeTime -= Time.deltaTime;

        if (this.changeTime <= 0)
        {
            // Update the strike pattern
            this.UpdateStrikePattern();
            this.changeTime = this.PatternChangeInterval;
        }
    }

    /// <summary>
    /// Stop the effect immediately
    /// </summary>
    public override void Stop()
    {
        base.Stop();

        // Hide the effect straight away
        this.strikeLine.enabled = false;
    }

    private void UpdateStrikePattern()
    {
        // Set the starting position
        Vector3[] points = new Vector3[this.ControlPoints + 2];
        points[0] = this.target.position;

        // Generate a random position for each point
        for (int i = 1; i < points.Length; i++)
        {
            float x = this.target.position.x + Random.Range(-0.05f, 0.05f);
            float y = ((float)i / (this.ControlPoints + 1)) * StrikeHeight;
            points[i] = new Vector3(x, y, 0);
        }

        // Set the points on the line renderer
        this.strikeLine.SetVertexCount(points.Length);
        for (int i = 0; i < points.Length; i++)
        {
            this.strikeLine.SetPosition(i, points[i]);
        }

        // Show the renderer
        this.strikeLine.enabled = true;
    }
}
