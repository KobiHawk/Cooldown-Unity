using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


    enum Abilities { Shoot, Line, Accelerate };

    private Rigidbody2D rb;
    private Abilities currAbility = Abilities.Shoot;

    public int speed = 10;
    public int fireDelay = 10; // number of frames between shots;
    public int accelerateDuration = 200; // number of frames accelerate lasts
    public int accelerateStrength = 2; // amount that accelerate speeds your fire rate
    private int currFireDelay = 0;
    private int currAccelerateDuration = 0;

    public BulletController bullet;
    public LineRenderer lineRenderer;
    public EdgeCollider2D lineHitbox;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //lineRenderer = GetComponent<LineRenderer>();
    }

	void FixedUpdate ()
    {
        move();      
    }

    void Update()
    {
        checkForInput();
        if(currAbility == Abilities.Shoot && Input.GetButton("Fire1") && currFireDelay == 0)
        {
            fire();
            currFireDelay = fireDelay;
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            switch(currAbility)
            {
                case Abilities.Line:
                    StartCoroutine("drawLine");
                    break;

                    /*
                case Abilities.Accelerate:
                    fireDelay /= accelerateStrength;
                    break;
                    */ // we want to use this ability as soon as it's pressed
            }
        }
        if(currFireDelay > 0)
        {
            currFireDelay--;
        }
        if(currAccelerateDuration > 0)
        {
            currAccelerateDuration--;
            if(currAccelerateDuration == 0)
            {
                fireDelay *= accelerateStrength;
            }
        }
    }

    void move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        rb.AddForce(transform.right * moveHorizontal * speed);
        rb.AddForce(transform.up * moveVertical * speed);
    }

    void fire()
    {
        BulletController newBullet = Instantiate(bullet, transform.position, Quaternion.identity) as BulletController;

        Vector3 sp = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = (Input.mousePosition - sp).normalized;
        newBullet.beginMovement(dir);
    }

    private void checkForInput()
    {
        if (Input.GetKeyDown("1"))
        {
            if (currAbility != Abilities.Line)
            {
                currAbility = Abilities.Line;
            }
            else // if we already had that ability selected, cancel it, go back to default shooting
            {
                currAbility = Abilities.Shoot;
            }
        }
        if(Input.GetKeyDown("2"))
        {
            fireDelay /= accelerateStrength;
            currAccelerateDuration = accelerateDuration;
        }
    }

    /*
     * On mouse click, set the start of the line
     * Then, while the mouse is held, continue drawing a line to the mouse cursor
     * When the button is released, set the end of the line and exit the loop
     */
    IEnumerator drawLine()
    {
        bool mouseIsHeld = true;

        Vector3 v3 = Input.mousePosition;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        v3.z = -2;
        Vector2 firstPoint = new Vector2(v3.x, v3.y);

        lineRenderer.SetPosition(0, v3);

        while (mouseIsHeld)
        {
            v3 = Input.mousePosition;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            v3.z = -2;

            lineRenderer.SetPosition(1, v3);
            if (Input.GetButtonUp("Fire1"))
            {
                //update hitbox for line
                Vector2[] newHitbox;
                newHitbox = lineHitbox.points;
                newHitbox[0] = firstPoint;
                newHitbox[1] = new Vector2(v3.x, v3.y);
                lineHitbox.points = newHitbox;

                mouseIsHeld = false;
                currAbility = Abilities.Shoot; // After placing a line, the ability should go on cooldown
            }
            yield return null;
        }
    }
}
