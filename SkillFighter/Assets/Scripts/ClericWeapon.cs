using UnityEngine;
using System.Collections;

public class ClericWeapon : MonoBehaviour
{
    protected int directionX { get; private set; }

    public static ClericWeapon Create(ClericWeapon instance, Transform launchPoint, int directionX)
    {
        ClericWeapon weapon = GameObject.Instantiate<ClericWeapon>(instance);

        // Set the start position
        Vector2 position = launchPoint.position;
        weapon.transform.position = position;
        weapon.directionX = directionX;

        // Flip the sprite if necessary
        if (directionX < 0)
        {
            Vector3 scale = weapon.transform.localScale;
            scale.x = -scale.x;
            weapon.transform.localScale = scale;
        }

        return weapon;
    }

    private void Start()
    {
        StartCoroutine(this.DestroyAfter(60));
    }
    
    private IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        GameObject.Destroy(this.gameObject);
    }
}
