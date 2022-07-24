using UnityEngine;
using System.Collections;

public abstract class WeaponEffect : MonoBehaviour
{
    // Members
    private static Color? mainBackgroundColor;
    private static int backgroundColorChangers;
    protected Transform target { get; private set; }

    public static WeaponEffect Create(WeaponEffect instance, Transform target)
    {
        WeaponEffect effect = GameObject.Instantiate<WeaponEffect>(instance);
        effect.target = target;

        return effect;
    }

    /// <summary>
    /// Set the background color of the main camera.
    /// The color will be reset once all the effects that have changed it have stopped.
    /// </summary>
    /// <param name="newColor">The new background color to use</param>
    public static void SetBackgroundColor(Color newColor)
    {
        // Something is changing the environment color
        WeaponEffect.backgroundColorChangers++;

        // Set the new color
        Camera.main.backgroundColor = newColor;
    }

    private static void ResetBackgroundColor()
    {
        // Something stopped changing the color
        WeaponEffect.backgroundColorChangers--;

        if (WeaponEffect.backgroundColorChangers <= 0 && WeaponEffect.mainBackgroundColor.HasValue)
        {
            // Reset the color now that nothing is changing it
            Camera.main.backgroundColor = WeaponEffect.mainBackgroundColor.GetValueOrDefault();
        }
    }

    void Awake()
    {
        // Store the starting background color in case we change it later
        if (!WeaponEffect.mainBackgroundColor.HasValue)
        {
            WeaponEffect.mainBackgroundColor = Camera.main.backgroundColor;
        }
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    public virtual void Stop()
    {
        // Get rid of the object
        GameObject.Destroy(this.gameObject);

        // Remove any color effect we performed
        WeaponEffect.ResetBackgroundColor();
    }
}
