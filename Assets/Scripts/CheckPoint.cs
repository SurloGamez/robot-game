using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    CheckPointSystem system;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] int ID;
    BoxCollider2D col;
    void Start()
    {
        system = transform.parent.GetComponent<CheckPointSystem>();
        col = GetComponent<BoxCollider2D>();
    }

    
    void FixedUpdate()
    {
        if(system.currentCheckPointNum == ID)
        {
            Collider2D collider = Physics2D.OverlapBox(transform.position, col.size, 0, playerLayer);
            if (collider)
            {
                system.currentCheckpointPos = (Vector2)transform.position - (Vector2.up * (col.size.y / 2)) + (Vector2.up * 2);
                system.currentCheckPointNum++;
            }
        }
      
    }
}
