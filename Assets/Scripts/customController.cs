using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class customController : MonoBehaviour
{
    public bool inControl = true;
    [SerializeField] LayerMask cameraLockLayer;
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask hurtLayer;
    [SerializeField] float skinWidth;
    [SerializeField] Vector2 RayAmount; // x for up and down, y for left and right
    [SerializeField] float Speed;
    [SerializeField] float Friction;
    [SerializeField] float Gravity;
    [SerializeField] float jumpForce;
    [SerializeField] GameObject trail;
    [SerializeField] float sideChargeWait;
    [SerializeField] float uppercutWait;
    [SerializeField] float groundPoundWait;
    [SerializeField] coolDownBar sideChargeBar;
    [SerializeField] coolDownBar uppercutBar; 
    [SerializeField] coolDownBar groundSmashBar;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject limbs;
    [SerializeField] Sprite chargeTrail;

    [HideInInspector]public bool isDead;

    CameraFollow cam;
    CheckPointSystem CPsystem;
    Vector2 respawnPos;

    KeyCode attackButton;
    KeyCode jumpButton;
    int attackButtonCounter = 0;
    float sideChargecd = 0;
    float uppercutcd = 0;
    float groundPoundcd = 0;

    p_animationController anim;
    SpriteRenderer sr;
   

    [HideInInspector] public Vector2 velocity;
    Vector2 extraMoveAmount;

    [HideInInspector] public Vector2 input = new Vector2();
    [HideInInspector] public Vector2 smoothInput = new Vector2();
    int dir = 1;
    int jumpCount = 2;
    bool jumpPressed = false;
    bool rolling;
    bool walljumped;
    int chargeCounter = 0; // if greater than 0 that means currently in attack
    Vector2 chargeDir; // for attacking sideways
    

    Vector2 RaySpacing;
    BoxCollider2D col;
    Bounds bounds;
    bool jumpButtonDown;
    bool grounded = false;
    bool walled = false;
    float wallJumpDir = 0;
    bool downslope;
    float slopeVector;
    float chargeVelocity;
    float groundPoundCooldown = 0; //this is the time it takes to exit the iron man position
    float wallJumpcontrolcd = 0; //time it takes to gain control over player again
    float wallJumpTimer = 0; //time is takes to wall jump again
    bool wallSliding = false;
    bool inwalljumpanimation = false;

    bool pauseMovement = false;

    int j, k = 0;
    void Start()
    {
        isDead = false;
        pauseMovement = false;

        cam = FindObjectOfType<CameraFollow>();
        anim = GetComponent<p_animationController>();
        sr = GetComponent<SpriteRenderer>();
        //CPsystem = FindObjectOfType<CheckPointSystem>();
        attackButton = KeyCode.X;
        jumpButton = KeyCode.Z;

        col = GetComponent<BoxCollider2D>();
        GetOrigins(Vector2.zero);
        CalculateRaySpacing();

        respawnPos = transform.position;
      
    }


    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        GetInput();

        grounded = false;
        extraMoveAmount = Vector2.zero;
        downslope = false;

        Vector2 Add2Pos = Vector2.zero;

        rayOrigins origins = GetOrigins(Vector2.zero);
        if(chargeCounter <= 0 || chargeDir.y == 1) UpdateYVel();
        if (chargeCounter <= 0) CheckDescendSlope(ref velocity, origins);
        VerticalMovement(ref velocity, origins);
        Add2Pos += new Vector2(extraMoveAmount.x, velocity.y);
        //if (!pauseMovement) transform.Translate(new Vector2(extraMoveAmount.x, velocity.y), Space.World);

        origins = GetOrigins(Add2Pos);
        if (chargeCounter <= 0) UpdateXVel();
        if (chargeCounter > 0 && chargeDir == Vector2.down || groundPoundCooldown > 0)
        {
            velocity.x = 0; 
        }
        CheckWalled(origins);

        HorizontalMovement(ref velocity, origins);
        if(!pauseMovement) transform.Translate(new Vector2(velocity.x, extraMoveAmount.y) + Add2Pos, Space.World);

        if (velocity.x > 0.03f) dir = 1;
        if (velocity.x < -0.03f) dir = -1;
        sr.flipX = dir == -1;

        walljumped = false;
        if (wallJumpcontrolcd > 0) wallJumpcontrolcd -= 0.02f;

        if(chargeCounter > 0)
        {
            wallJumpcontrolcd = 0;
        }

        wallJumpTimer -= 0.02f;
        if (grounded)
        {
            jumpCount = 2;
            rolling = false;
            wallJumpcontrolcd = 0;
            wallJumpTimer = 0;

            wallSliding = false;
            inwalljumpanimation = false;
        }

        if(walled && !walljumped && !grounded && velocity.y <= 0) //wall sliding
        {
            wallSliding = true;
        }

        if (jumpButtonDown && groundPoundCooldown <= 0 && (jumpCount > 0 || walled) && !jumpPressed && chargeCounter <= 0 && wallJumpTimer <= 0)
        {
            if (walled && !grounded) // wall jump
            {
                smoothInput.x = wallJumpDir * 5;
                jumpCount = 1;
                walljumped = true;
                wallJumpcontrolcd = 1;
                wallJumpTimer = 0.5f;
                inwalljumpanimation = true;
            }

            jumpPressed = true;
            velocity.y = jumpForce;
            grounded = false;
            jumpCount--;

            if (jumpCount == 0) rolling = true; // jumpcount has to be EXACTLY 0
        }

        if (wallSliding) rolling = false;

        checkCameraLocks();

        CheckAttack();
        UpdateUI();
        CheckTouchingDeath(col.offset + (Vector2)transform.position, col.size);

        anim.UpdateAnimation(
            Mathf.Abs(velocity.x) > 0.05f
            ,grounded 
            ,velocity.y > 0.1f && chargeCounter <= 0 && !walljumped
            ,velocity.y < -0.2f && chargeCounter <= 0
            ,rolling
            ,attackButtonCounter == 1 && chargeCounter > 0
            ,chargeDir.x 
            ,chargeDir.y 
            ,chargeCounter <= 0
            ,walljumped
            ,walled && !walljumped && !grounded && velocity.y <= 0
            ,chargeCounter > 0
            ,inwalljumpanimation
            );
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

        if(walled && velocity.y <= 0 && (input.x == 0 || -input.x == wallJumpDir))
        {
            velocity.y *= 0.9f;
        }

        if (velocity.y <= -1.5) velocity.y = -1.5f;
    }

    void UpdateXVel()
    {
        velocity.x = smoothInput.x * Speed;
        if (downslope) velocity.x *= slopeVector;
    }

    void GetInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(jumpButton))
        {
            jumpButtonDown = true;
        }
        else
        {
            jumpPressed = false;
            jumpButtonDown = false;
        }
        if (Input.GetKey(attackButton) && groundPoundCooldown <= 0) attackButtonCounter += 1; else attackButtonCounter = 0;

        if(!inControl)
        {
            input = Vector2.zero;
            attackButtonCounter = 0;
            jumpButtonDown = false;
            chargeCounter = 0;
        }


        //doesnt have to do with player inputs
        if (groundPoundCooldown > 0) input = Vector2.zero;  

        if (chargeCounter > 0) input.x = 0;

        if(wallJumpcontrolcd <= 0)
        {
            if (grounded)
            {
                smoothInput.x += input.x * 1.4f;
                smoothInput.x *= Friction;
            }
            else
            {
                smoothInput.x += input.x * 0.24f;
                smoothInput.x *= 0.96f;
            }
        }
      

        if (Mathf.Abs(smoothInput.x) < 0.02f) smoothInput.x = 0;
       
        if (chargeCounter > 0 || groundPoundCooldown > 0) smoothInput.x = 0;

        if (Mathf.Abs(smoothInput.x) <= 0.001f ) smoothInput.x = 0;
    }

    void CheckAttack()
    {
        //updates all the cool down variables
        if(groundPoundCooldown > 0) groundPoundCooldown -= 0.02f; //this is the animation one, not the actual attack
        sideChargecd -= 0.02f; if (sideChargecd < 0) { sideChargecd = 0; }
        uppercutcd -= 0.02f; if (uppercutcd < 0) { uppercutcd = 0; }
        groundPoundcd -= 0.02f; if (groundPoundcd < 0) { groundPoundcd = 0; }

        if (attackButtonCounter == 1 && chargeCounter <= 0) // this means this is the first frame attack button is pressed (get the attack key down)
        {
            bool endAttack = true;

            if (input.y > 0 && uppercutcd <= 0) //uppercut
            {
                chargeDir = Vector2.up;
                velocity.y = jumpForce;
                uppercutcd = uppercutWait;
                endAttack = false;
            }
            else if (input.y < 0 && groundPoundcd <= 0 && jumpCount <= 0) //ground strike
            {
                chargeDir = Vector2.down;
                groundPoundcd = groundPoundWait;
                endAttack = false;
            }
            else if (input.y == 0 && sideChargecd <= 0) //side charge
            {
                if (input.x == 0) chargeDir = Vector2.right * dir;
                else chargeDir = Vector2.right * input.x;

                sideChargecd = sideChargeWait;
                endAttack = false;
            }
            
            

            if (endAttack) {  return; }

            rolling = false;
            chargeVelocity = 1;
            chargeCounter = 30;
            
        }

        if (chargeCounter == 0) // when attack ends
        {
            chargeCounter -= 1;

            if (chargeDir == Vector2.down) groundPoundCooldown = 0.5f;
        }

        if (chargeCounter > 0) // during attack
        {
            if(chargeDir.x != 0) // this means going side ways
            {
                /*if(chargeCounter <= 19 && chargeCounter % 2 != 0 && Mathf.Abs(velocity.x) >= 0.4f) SpawnTrail();*/ // this just makes trails

                velocity = chargeDir * chargeVelocity;
                chargeCounter -= 1;
                chargeVelocity *= 0.95f;
            }

            if (chargeDir == Vector2.down) // this means going down
            {
                velocity = chargeDir;

                if (grounded) chargeCounter = 0;
            }

            if (chargeDir == Vector2.up) // this means going up
            {
                if (velocity.y <= 0) chargeCounter = 0;
                velocity.x *= 0.95f;
            }
          
        }
    }

    void SpawnTrail()
    {
        GameObject trailPre = Instantiate(trail, transform.position + Vector3.forward, transform.rotation);
        SpriteRenderer trailsr = trailPre.GetComponent<SpriteRenderer>();
        trailsr.sprite = chargeTrail;
        trailsr.flipX = sr.flipX;
        Destroy(trailPre, 1);
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
                if(dir == -1) grounded = true;
                dist = hit.distance;

            }
        }
        velocity.y = (dist - skinWidth) * dir;
        
        if(!grounded && velocity.y <= 0 && velocity.y >= -0.1f)
        {
            RaycastHit2D checkgroundleft = Physics2D.Raycast(origins.bl, Vector2.down, 0.4f, ground);
            RaycastHit2D checkgroundright = Physics2D.Raycast(origins.br, Vector2.down, 0.4f, ground);

            if (checkgroundleft || checkgroundright) grounded = true;
        }
       
       
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
                if(i == 0)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if(slopeAngle > 80)
                    {
                        smoothInput.x = 0;

                    }

                    if (slopeAngle <= 80 && chargeCounter <= 0)
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

    void CheckWalled(rayOrigins origins)
    {
        walled = false;

        RaycastHit2D topleft = Physics2D.Raycast((origins.tl + origins.bl) * 0.5f, Vector2.left, 0.05f, ground);
        //RaycastHit2D bottomleft = Physics2D.Raycast(origins.bl, Vector2.left, 0.05f, ground);
        RaycastHit2D topright = Physics2D.Raycast((origins.tr + origins.br) *0.5f, Vector2.right, 0.05f, ground);
       // RaycastHit2D bottomright = Physics2D.Raycast(origins.br, Vector2.right, 0.05f, ground);

        if (topleft && Mathf.Abs(topleft.normal.y) <= 0.05f /*|| bottomleft*/) { wallJumpDir = 1; walled = true; }
        else if (topright && Mathf.Abs(topright.normal.y) <= 0.05f /*|| bottomright*/) { wallJumpDir = -1; walled = true; }

        if (walled && wallJumpDir == input.x)
        {
            
            walled = false;
            rolling = false;
        }

         
    }

    void ClimbSlope(ref Vector2 velocity, float angle)
    {
        float dir = Mathf.Sign(velocity.x);
        float moveAmount = Mathf.Abs(velocity.x);
        extraMoveAmount.y = Mathf.Sin(angle * Mathf.Deg2Rad) * moveAmount;
        velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveAmount * dir;
    }

    public struct rayOrigins
    {
        public Vector2 tl, tr, bl, br;
    }

    void UpdateUI()
    {
        sideChargeBar.variable = sideChargecd / sideChargeWait;
        uppercutBar.variable = uppercutcd / uppercutWait;
        groundSmashBar.variable = groundPoundcd / groundPoundWait;

    }

    void CheckTouchingDeath(Vector2 origin, Vector2 size)
    {
        Collider2D col = Physics2D.OverlapBox(origin, size, 0, hurtLayer);
        if (col != null && !pauseMovement)
        {
            isDead = true;
            pauseMovement = true;
            sr.enabled = false;
            cam.CameraShake();
            GameObject explosionPre = Instantiate(explosion, transform.position, transform.rotation);
            explosionPre.transform.localScale = Vector3.one * 1.25f;
            GameObject limbsPre = Instantiate(limbs, transform.position, transform.rotation);
            Destroy(explosionPre, 0.9f);
            Destroy(limbsPre, 3);
            StartCoroutine(waitToRespawn(respawnPos));
        }
    }

    IEnumerator waitToRespawn(Vector2 pos)
    {
        yield return new WaitForSeconds(2);
        Respawn(pos);
    }

    public void Respawn(Vector2 pos)
    {
        transform.position = pos;
        dir = 1;
        velocity = Vector2.zero;
        smoothInput = Vector2.zero;
        grounded = false;
        walled = false;
        wallJumpDir = 0;
        rolling = false;
        attackButtonCounter = 0;
        sideChargecd = 0;
        uppercutcd = 0;
        groundPoundcd = 0;
        groundPoundCooldown = 0;
        chargeCounter = 0;
        extraMoveAmount = Vector2.zero;
        anim.Reset();
        sr.enabled = true;
        pauseMovement = false;
        StartCoroutine(respawnFade());
        isDead = false;
    }

    IEnumerator respawnFade()
    {
        for(int i = 0; i < 4; i++)
        {
            sr.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(0.2f);
            sr.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.2f);
        }
      
    }
    //block (3.5, 2) = camera 1
    void checkCameraLocks()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, Vector2.one, 0, cameraLockLayer);
        if (!hit)
        {
            j++;
            k = 0;
            if (j == 1) cam.inLock = false;

            
        }
        else
        {
            j = 0;
            k++;
            if (k == 1)
            {
                BoxCollider2D box = hit.gameObject.GetComponent<BoxCollider2D>();
                Vector2 size = box.size;
                Vector2 pos = box.offset + (Vector2)hit.gameObject.transform.position;
                cam.followPos = pos;
                cam.targetZoomAmount = size.x / 3.5f;
                cam.inLock = true;
            }
        }
        
       
    }
}
