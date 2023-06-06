using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class coolDownBar : MonoBehaviour
{


    public float variable;
    [SerializeField] Color ready;
    [SerializeField] Color onCooldDown;
    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    void FixedUpdate()
    {
        transform.localScale = new Vector3(1 - variable, transform.localScale.y, 1);
        if(variable == 0)// ready
        {
            image.color = ready;
        }
        else
        {
            image.color = onCooldDown;
        }
        
    }
}
