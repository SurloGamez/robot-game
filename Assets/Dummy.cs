using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField] Vector2 RayAmount; // x for up and down, y for left and right
    [SerializeField] float skinWidth;
    [SerializeField] LayerMask ground;
    [SerializeField] float Gravity;
    public Vector2 velocity;
    Vector2 RaySpacing;
    BoxCollider2D col;
    Bounds bounds;
    bool grounded = false;
    Vector2 extraMoveAmount;
    bool downslope = false;
    float slopeVector = 0;
    void Start()
    {

        col = GetComponent<BoxCollider2D>();
        GetOrigins(Vector2.zero);
        CalculateRaySpacing();
    }

    void FixedUpdate()
    {
        grounded = false;
        extraMoveAmount = Vector2.zero;

        Vector2 Add2Pos = Vector2.zero;

        rayOrigins origins = GetOrigins(Vector2.zero);
        UpdateYVel();
        CheckDescendSlope(ref velocity, origins);
        VerticalMovement(ref velocity, origins);
        Add2Pos += new Vector2(extraMoveAmount.x, velocity.y);

        origins = GetOrigins(Add2Pos);
        HorizontalMovement(ref velocity, origins);

        transform.Translate(new Vector2(velocity.x, extraMoveAmount.y) + Add2Pos, Space.World);
    }

    void UpdateYVel()
    {
        if (velocity.y > 0)
        {
            velocity.y -= Gravity;
        }
        else
        {
            velocity.y -= Gravity * 1.5f;
        }

        if (velocity.y <= -1.5) velocity.y = -1.5f;
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
                    //if (slopeAngle > 80)
                    //{
                    //    smoothInput.x = 0;

                    //}

                    if (slopeAngle <= 80)
                    {
                        float dist2Slope = hit.distance - skinWidth;
                        velocity.x -= dist2Slope * dir;
                        ClimbSlope(ref velocity, slopeAngle);
                        velocity.x += dist2Slope * dir;
                        return;
                    }
                }

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
            downslope = true;
            slopeVector = Mathf.Abs(slopeVec.x);
        }


    }

    void DescendSlope(ref Vector2 velocity, Vector2 vec)
    {
        float moveAmount = Mathf.Abs(velocity.y) * velocity.x;
        extraMoveAmount.x = moveAmount * vec.x;
        velocity.y = moveAmount * vec.y;
    }

}
