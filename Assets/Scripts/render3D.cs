using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class render3D : MonoBehaviour
{
    [SerializeField] int mainIndex;
    [SerializeField] Transform main;
    [SerializeField] Color color;
    public float parallaxMultiplier;
    [SerializeField] Material material;
    [SerializeField] int numberOfLayers;
    List<Transform> foreground = new List<Transform>();
    Vector2 foregroundOrigin;
    Camera cam;
    AutoCollider setCol;
    void Start()
    {
        cam = Camera.main;
        GameObject main = transform.GetChild(0).gameObject;
        setCol = GetComponent<AutoCollider>();

        //spawn clones - orders dont matter yet, literally is just spawning enough clones
        int x = -mainIndex;
        for (int k = 0; k < numberOfLayers; k++)
        {
            GameObject clone = Instantiate(main, transform);
            clone.name = "clone: " + -x;
            x++;
            if (x == 0) x++;
            foreground.Add(clone.transform);
        }

        //insert main 
        main.transform.SetSiblingIndex(mainIndex);
        foreground.Insert(mainIndex, main.transform);
       
       
        foregroundOrigin = transform.position;
        float sizeMult = 1;
        for(int i = mainIndex; i < foreground.Count; i++)
        {
            foreground[i].transform.localScale *= sizeMult;
            sizeMult += parallaxMultiplier;
        }
        sizeMult = 1;
        for(int i = mainIndex; i >= 0; i--)
        {
            foreground[i].transform.localScale *= sizeMult;
            sizeMult -= parallaxMultiplier;
        }

        int j = mainIndex ;
        for(int i = 0; i < foreground.Count; i++)
        {
            foreground[i].position = new Vector3(foreground[i].position.x, foreground[i].position.y, j + transform.position.z);
            j--;
            if (i != foreground.Count - 1)
            {
                SpriteRenderer[] renders = foreground[i].GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer render in renders)
                {
                    render.material = material;
                    render.color = color;
                }
            }
            if(i != mainIndex)
            {
                Collider2D[] cols = foreground[i].GetComponentsInChildren<Collider2D>();
                foreach (Collider2D col in cols)
                {
                    Destroy(col);
                }
            }
            

        }
        //box collider first
        setCol.AutoSetColliders(main);
      
        //then stairs
        EdgeCollider2D[] Ecols = main.transform.GetComponentsInChildren<EdgeCollider2D>();
        foreach (EdgeCollider2D col in Ecols)
        {
            col.gameObject.GetComponent<AutoStairCollider>().SetEdgeCollider();
        }
    }

    void FixedUpdate()
    {
        renderLevel();
    }

    void renderLevel()
    {
        int k = 0;
        for (int i = mainIndex; i < foreground.Count; i++)
        {
            Vector2 newpos = (k * -parallaxMultiplier * (Vector2)(cam.transform.position - transform.position)) + foregroundOrigin;
            foreground[i].position = new Vector3(newpos.x, newpos.y, foreground[i].position.z);
            k++;
        }
        k = 1;
        for (int i = mainIndex - 1; i >= 0; i--)
        {
            Vector2 newpos = (k * parallaxMultiplier * (Vector2)(cam.transform.position - transform.position)) + foregroundOrigin;
            foreground[i].position = new Vector3(newpos.x, newpos.y, foreground[i].position.z);
            k++;
        }
    }
}
