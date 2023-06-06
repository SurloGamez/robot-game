using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStairCollider : MonoBehaviour
{
    [SerializeField] GameObject left;
    [SerializeField] GameObject right;

    public void SetEdgeCollider() 
    {

        BoxCollider2D lcol = left.GetComponent<BoxCollider2D>();
        BoxCollider2D rcol = right.GetComponent<BoxCollider2D>();

        Vector2 leftpos = new Vector2(lcol.bounds.max.x, lcol.bounds.max.y) - (Vector2)transform.position;
        Vector2 rightpos = new Vector2(rcol.bounds.min.x, rcol.bounds.max.y) - (Vector2)transform.position;



        Vector2[] points = new Vector2[2];
        points[0] = leftpos;
        points[1] = rightpos;

        GetComponent<EdgeCollider2D>().points = points;
    }
}
