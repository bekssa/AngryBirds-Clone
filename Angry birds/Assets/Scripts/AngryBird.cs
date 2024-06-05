using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryBird : MonoBehaviour
{
    
    private Rigidbody2D _rb;
    private CircleCollider2D _cc;

    private bool _hasBeenLaunched;
    private bool _shouldFaceVelDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _cc = GetComponent<CircleCollider2D>();
    }

    private void Start()
    { 
        _rb.isKinematic = true;
        _cc.enabled = false;
    }
    private void FixedUpdate()
    {
        if (_hasBeenLaunched && _shouldFaceVelDirection) 
        {
            transform.right = _rb.velocity;
        }
    }
    public void LaunchBird(Vector2 direction , float force) 
    {
        
        _rb.isKinematic = false;
        _cc.enabled = true;

        //apply the force

        _rb.AddForce(direction * force, ForceMode2D.Impulse);
        _hasBeenLaunched = true;
        _shouldFaceVelDirection = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _shouldFaceVelDirection = false;
    }
}
