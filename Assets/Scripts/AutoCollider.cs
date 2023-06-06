using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCollider : MonoBehaviour
{
    public void AutoSetColliders(GameObject parent)
    {
        Transform[] children = parent.transform.GetComponentsInChildren<Transform>();
        foreach(Transform child in children)
        {
            BoxCollider2D col;
            child.gameObject.TryGetComponent<BoxCollider2D>(out col);

            SpriteRenderer sr;
            child.gameObject.TryGetComponent<SpriteRenderer>(out sr);

            if(col && sr)
            {
                col.offset = Vector2.zero;
                col.size = sr.size;
            }
           

        }
    }
}
