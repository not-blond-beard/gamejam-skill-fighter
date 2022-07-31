using UnityEngine;
using System.Collections;

public class Rotating : MonoBehaviour
{
    private Transform renderTransform;

    public void Start()
    {

        SpriteRenderer sprite = this.GetComponentInChildren<SpriteRenderer>();
        this.renderTransform = sprite.transform;
        
        float x = 3 * 100;
        Rigidbody2D body = this.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.AddForce(new Vector2(x, 0));
        }
    }

    public void Update()
    {
        this.renderTransform.Rotate(Vector3.forward, Time.deltaTime * 1000);
    }
}
