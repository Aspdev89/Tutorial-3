using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    public int hitcount = 1;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    void Update()
    {
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController e = other.collider.GetComponent<EnemyController>();
        // RubyController a = GetComponent<RubyController>();
        if (e != null)
        {
            e.Fix();

        }


        HardEnemyController f = other.collider.GetComponent<HardEnemyController>();
        if (f != null)
        {
            f.Fix();
        }

        UltimateEnemyController g = other.collider.GetComponent<UltimateEnemyController>();
        if (g != null)
        {
            g.Fix();

        }


        Destroy(gameObject);
    }
}