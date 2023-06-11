using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceFollow : MonoBehaviour
{
    [SerializeField] float multiplier;
    [SerializeField] float max;
    Vector2 origin;
    Vector2 target;
    float zorigin;
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        zorigin = transform.position.z;
        player = FindObjectOfType<customController>().gameObject.transform;
       
    }

    
    void FixedUpdate()
    {
        target = player.position;
        Vector2 offset = ((target - origin) * multiplier);
        if (offset.magnitude >= max) { offset = offset.normalized * max; }
        Vector3 position = origin + offset;
        position.z = zorigin;
        transform.position = position;
    }
}
