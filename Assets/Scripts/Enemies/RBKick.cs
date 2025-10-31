using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBKick : MonoBehaviour
{
    Rigidbody2D rb;
    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    void Start()
    {
        Debug.Log($"BodyType={rb.bodyType}  isKinematic?={rb.isKinematic}  Constraints={rb.constraints}");
        rb.velocity = new Vector2(5f, 0f); // should immediately slide right
        // Or try a force:
        // rb.AddForce(Vector2.right * 200f, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        // Log speed so you can see if something zeros it each frame
        Debug.Log($"Speed: {rb.velocity}");
    }
}
