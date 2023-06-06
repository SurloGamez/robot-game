using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textType : MonoBehaviour
{
    public int type;
    Vector2 StartPos;
    float wave = 0;

    private void Start()
    {
        StartPos = transform.localPosition;
    }
    private void FixedUpdate()
    {

       

        if (type == 1)
        {
            wave += 10;
            transform.localPosition = new Vector2(transform.localPosition.x, StartPos.y + (0.25f * Mathf.Sin(wave * Mathf.Deg2Rad)));
        }

        if(type == 2 )
        {
            Vector2 shake = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            transform.localPosition = new Vector2(StartPos.x + shake.x, StartPos.y + shake.y);
        }

        if(type == 3)
        {
            wave += 5;
            float value = Mathf.Sin(wave * Mathf.Deg2Rad);
            transform.localScale = Vector2.one * (0.3f *  value + 0.7f);
            transform.localPosition = new Vector2(StartPos.x + (0.3f * -value + 0.7f) * 0.5f, StartPos.y - (0.3f * -value + 0.7f)* 0.5f);
        }
    }
}
