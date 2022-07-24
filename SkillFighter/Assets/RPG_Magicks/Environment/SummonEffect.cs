using UnityEngine;
using System.Collections;

public class SummonEffect : WeaponEffect
{

    protected override void Start()
    {
        base.Start();

        this.transform.position = this.target.position;
    }
}
