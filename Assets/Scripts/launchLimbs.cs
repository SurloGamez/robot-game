using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launchLimbs : MonoBehaviour
{
    Vector2 vel;
    [SerializeField] Vector2 XvelRange;
    [SerializeField] Vector2 YvelRange;
    int dir;
    float power;
    // Start is called before the first frame update
    void Start()
    {
        vel.x = Random.Range(XvelRange.x, XvelRange.y);
        vel.y = Random.Range(YvelRange.x, YvelRange.y);
        dir = (Random.Range(0, 2) * 2) - 1;
        power = Random.Range(5, 10);
    }

    void FixedUpdate()
    {
        transform.Translate(vel, Space.World);
        vel.y -= 0.02f;
        vel.x *= 0.995f;
        transform.rotation *= Quaternion.Euler(0, 0, dir * power) ;
    }
}
