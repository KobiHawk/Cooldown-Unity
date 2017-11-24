using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {



    public int speed = 10;
    public int replicateDelay = 200;
    Rigidbody2D rb;
    public int replicateTimer = 0; // used to prevent lenses from being placed too close, time in frames until bullet can replicate again
    //private float refractionIndex = 1.33f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if(replicateTimer > 0)
        {
            replicateTimer--;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
        else if ((collision.gameObject.tag == "Lens") && (replicateTimer == 0))
        {
            Vector2 normal = getNormal(rb.velocity);

            BulletController newBullet = Instantiate(this) as BulletController;

            rb.velocity = new Vector2(rb.velocity.normalized.x + normal.x * 0.1f, rb.velocity.normalized.y + normal.y * 0.1f);
            newBullet.rb.velocity = new Vector2(rb.velocity.normalized.x + normal.x * -0.1f, rb.velocity.normalized.y + normal.y * -0.1f);

            Vector2 temp = rb.velocity.normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(temp * speed);

            temp = newBullet.rb.velocity.normalized;
            newBullet.rb.velocity = Vector2.zero;
            newBullet.rb.AddForce(temp * speed);


            replicateTimer = replicateDelay;
            newBullet.replicateTimer = replicateDelay;
        }
    }

    /*
     * Called by PlayerController.cs in the fire() function
     */
    public void beginMovement(Vector3 direction)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(direction * speed);
    }

    //Finds the normal (perpendicular) line of the vector, used to skew trajectory of projectile velocity when passing through lens
    private Vector2 getNormal(Vector2 vector)
    {
        Vector2 result = vector;
        result.x = result.y;
        result.y = -vector.x;

        return result.normalized;
    }
}
