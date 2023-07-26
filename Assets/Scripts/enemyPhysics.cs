using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyPhysics : MonoBehaviour
{
    CameraFollow cam;
    [SerializeField] Vector2 RayAmount; // x for up and down, y for left and right
    [SerializeField] float skinWidth;
    [SerializeField] LayerMask ground;
    [SerializeField] float Speed;
    [SerializeField] float Friction;
    [SerializeField] float Gravity;
    [SerializeField] GameObject sparks;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject limbs;
    public float health = 5;
    enemyAnimationController anim;
    SpriteRenderer sr;
    GameObject player;
    int dir = 1;
    public Vector2 velocity;
    public float xinput = 0;
    Vector2 RaySpacing;
    BoxCollider2D col;
    Bounds bounds;
    bool grounded = false;
    Vector2 extraMoveAmount;
    [HideInInspector]public bool stunned = false;
    bool bounce = false;
    float sdconst = 10;
    float stunDuration = 0;
    bool animgrounded = false;
    bool pauseMovement = false;
    bool isDead = false;

    void Start()
    {
        
        player = FindObjectOfType<customController>().gameObject;
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<enemyAnimationController>();
        col = GetComponent<BoxCollider2D>();
        GetOrigins(Vector2.zero);
        CalculateRaySpacing();
        cam = Camera.main.GetComponent<CameraFollow>();
        dir = sr.flipX ? 1 :-1;
    }

    void FixedUpdate()
    {

        grounded = false;
        
        extraMoveAmount = Vector2.zero;

        updateStunnedCounter();
        


        Vector2 Add2Pos = Vector2.zero;

        rayOrigins origins = GetOrigins(Vector2.zero);
        UpdateYVel();
        CheckDescendSlope(ref velocity, origins);
        VerticalMovement(ref velocity, origins);
        Add2Pos += new Vector2(extraMoveAmount.x, velocity.y);

        origins = GetOrigins(Add2Pos);
        UpdateXVel();
        HorizontalMovement(ref velocity, origins);

        if(!pauseMovement) transform.Translate(new Vector2(velocity.x, extraMoveAmount.y) + Add2Pos, Space.World);

        sr.flipX = dir == 1;

        if (grounded && stunned)
        {
            stunned = false;
            stunDuration = 0;

            animgrounded = true;
        }

        anim.UpdateAnimation(
              stunned
            , stunDuration
            , animgrounded
            );

        animgrounded = grounded;
    }

    void UpdateYVel()
    {
        velocity.y -= Gravity;

        if (velocity.y <= -1.5) velocity.y = -1.5f;
    }

    void UpdateXVel()
    {

        if(!stunned) 
        {
            if(grounded)
            {
                xinput *= Friction;
            }
            else
            {
                xinput *= 0.96f;
            }
            
        }

        if(bounce)
        {
            xinput = -Mathf.Sign(xinput) * 2;
            velocity.y = 0.5f;
            anim.ForcePlay("stun anim");
            stunDuration = 0.5f;
        }
        velocity.x = xinput * Speed;
    }
    public struct rayOrigins
    {
        public Vector2 tl, tr, bl, br;
    }
    rayOrigins GetOrigins(Vector2 add2pos)
    {
        bounds = col.bounds;
        bounds.Expand(-skinWidth * 2);
        rayOrigins origins = new rayOrigins();
        origins.tl = new Vector2(bounds.min.x, bounds.max.y) + add2pos;
        origins.tr = new Vector2(bounds.max.x, bounds.max.y) + add2pos;
        origins.bl = new Vector2(bounds.min.x, bounds.min.y) + add2pos;
        origins.br = new Vector2(bounds.max.x, bounds.min.y) + add2pos;
        return origins;
    }

    void CalculateRaySpacing()
    {
        RaySpacing.x = bounds.size.x / (RayAmount.x - 1); //for vertical movement
        RaySpacing.y = bounds.size.y / (RayAmount.y - 1); //for horizontal movement
    }

    void HorizontalMovement(ref Vector2 velocity, rayOrigins origins)
    {
        bounce = false;
        if (velocity.x == 0)
        {
            return;
        }
        float dir = Mathf.Sign(velocity.x);
        Vector2 origin = dir == 1 ? origins.br : origins.bl;
        float dist = Mathf.Abs(velocity.x) + skinWidth;
        for (int i = 0; i < RayAmount.y; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2.up * i * RaySpacing.y) + origin, Vector2.right * dir, dist, ground);
            if (hit)
            {
                dist = hit.distance;
                if (i == 0)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (slopeAngle <= 80 && !stunned)
                    {
                        float dist2Slope = hit.distance - skinWidth;
                        velocity.x -= dist2Slope * dir;
                        ClimbSlope(ref velocity, slopeAngle);
                        velocity.x += dist2Slope * dir;
                        return;
                    }
                }

                if (stunned)
                {
                    bounce = true;
                }
                bounce = true;

            }

        }
        velocity.x = (dist - skinWidth) * dir;
    }

    void VerticalMovement(ref Vector2 velocity, rayOrigins origins)
    {
        float dir = Mathf.Sign(velocity.y);
        if (velocity.y == 0) dir = -1;
        Vector2 origin = dir == 1 ? origins.tl : origins.bl;
        float dist = Mathf.Abs(velocity.y) + skinWidth;
        for (int i = 0; i < RayAmount.x; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2.right * i * RaySpacing.x) + origin, Vector2.up * dir, dist, ground);
            if (hit)
            {
                if (dir == -1) grounded = true;
                dist = hit.distance;

            }
        }
        velocity.y = (dist - skinWidth) * dir;

        if (!grounded && velocity.y <= 0 && velocity.y >= -0.1f)
        {
            RaycastHit2D checkgroundleft = Physics2D.Raycast(origins.bl, Vector2.down, 0.4f, ground);
            RaycastHit2D checkgroundright = Physics2D.Raycast(origins.br, Vector2.down, 0.4f, ground);

            if (checkgroundleft || checkgroundright) grounded = true;
        }


    }

    void ClimbSlope(ref Vector2 velocity, float angle)
    {
        float dir = Mathf.Sign(velocity.x);
        float moveAmount = Mathf.Abs(velocity.x);
        extraMoveAmount.y = Mathf.Sin(angle * Mathf.Deg2Rad) * moveAmount;
        velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveAmount * dir;
    }

    void CheckDescendSlope(ref Vector2 velocity, rayOrigins origins)
    {
        if (velocity.y > 0 || velocity.x == 0) return;
        if (stunned) return;

        float dirx = Mathf.Sign(velocity.x);
        Vector2 origin = dirx == 1 ? origins.bl : origins.br;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.5f + skinWidth, ground);

        if (hit)
        {
            Vector2 slopeVec = -1 * Vector2.Perpendicular(hit.normal);
            if (hit.normal == Vector2.up || Mathf.Sign(hit.normal.x) != dirx) return;
            float dist2Slope = hit.distance - skinWidth;
            velocity.y += dist2Slope;
            DescendSlope(ref velocity, slopeVec);
            velocity.y -= dist2Slope;
        }


    }

    void DescendSlope(ref Vector2 velocity, Vector2 vec)
    {
        float moveAmount = Mathf.Abs(velocity.y) * velocity.x;
        extraMoveAmount.x = moveAmount * vec.x;
        velocity.y = moveAmount * vec.y;
    }

    public void ApplyForce(Vector2 vec, float sd)
    {
        if (isDead) return;
        health -= 1;
        if(health <= 0)
        {
            explode();
            return;
        }
        GameObject sparkClone = Instantiate(sparks, (Vector2)transform.position + (vec * 0.2f), Quaternion.Euler(0, 0, 0));
        sparkClone.transform.localScale = Vector3.one * 0.5f;
        Destroy(sparkClone, 0.9f);
        cam.CameraShake(0.2f, 2);
        sdconst = sd;
        stunDuration = sd;
        if (sd == 0) stunDuration = 0.02f;
        velocity = vec;
        xinput = vec.x;
        if(vec.x != 0) dir = (int)Mathf.Sign(vec.x);

    }
    void explode()
    {
        col.enabled = false;
        isDead = true;
        cam.CameraShake(0.5f, 5);
        pauseMovement = true;
        sr.enabled = false;
        GameObject explosionPre = Instantiate(explosion, transform.position, transform.rotation);
        explosionPre.transform.localScale = Vector3.one * 1f;
        GameObject limbsPre = Instantiate(limbs, transform.position, transform.rotation);
        Destroy(explosionPre, 0.9f);
        Destroy(limbsPre, 3);
    }
    void updateStunnedCounter()
    {

        if (stunDuration > 0)
        {
            stunDuration-= 0.02f;
            stunned = true;
        }
        else
        {
            stunned = false;
        }
    }


}
