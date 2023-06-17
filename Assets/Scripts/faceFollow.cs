using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceFollow : MonoBehaviour
{
    //figure out why when reloading the scene, face starts higher.
    [SerializeField] float multiplier;
    [SerializeField] float max;
    [SerializeField] bool isMustache = false;
    [SerializeField] Animator anim;
    public bool isTalking = false;
    Vector2 moveby;
    Vector2 origin;
    Vector2 target;
    float zorigin;
    Transform player;
    float count = 0;
  
    void Start()
    {
        origin = transform.position;
        zorigin = transform.position.z;
        player = FindObjectOfType<customController>().gameObject.transform;
       
    }

    
    void FixedUpdate()
    {
            if (isMustache)
            {
                if (isTalking)
                {
                    count += 1;
                    moveby = Vector2.down * Mathf.Sin(count);

                }

            }
            else
            {
                count = 0;
                moveby = Vector2.zero;
            }
            target = player.position;
            Vector2 offset = ((target - origin) * multiplier);
            if (offset.magnitude >= max) { offset = offset.normalized * max; }
            Vector3 position = origin + offset;
            position.y += moveby.y * 0.05f;
            position.z = zorigin;
            transform.position = position;
        
    }
}
