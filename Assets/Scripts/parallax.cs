using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{

    [SerializeField] int Layer;
    Vector3 origin;
    Vector2 startsize;
    Camera cam;
    //negative value for foreground, positive is background
    render3D script;
    void Start()
    {
        origin = transform.position;
        startsize = transform.localScale;
        script = FindObjectOfType<render3D>();
        cam = Camera.main;

        float sizeMult = 1 - (Layer * script.parallaxMultiplier);
        transform.localScale *= sizeMult;

       
    }
    void FixedUpdate()
    {
        Vector2 pos = (Layer * script.parallaxMultiplier * (Vector2)(cam.transform.position - transform.position)) + (Vector2)origin;
        transform.position = new Vector3(pos.x, pos.y, Layer + origin.z);

    }
}
