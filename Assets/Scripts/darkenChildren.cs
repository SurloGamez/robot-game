using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class darkenChildren : MonoBehaviour
{
    [SerializeField] Color color;
    SpriteRenderer[] sprites;
    // Start is called before the first frame update
    void Start()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer sr in sprites)
        {
            sr.color *= color;
        }
    }

  
}
