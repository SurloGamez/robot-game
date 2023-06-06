using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform followPos;
    Vector2 ShakeAmount;
    customController player;
    Vector2 currentpos;

    void Start()
    {
        player = FindObjectOfType<customController>();
        followPos = player.transform;
        currentpos = transform.position;
    }

    void FixedUpdate()
    {

        currentpos = Vector2.Lerp(currentpos, (Vector2)followPos.position + (Vector2.up * 2), 0.1f);

        transform.position = new Vector3(currentpos.x + ShakeAmount.x, currentpos.y + ShakeAmount.y, -10);

       
    }


    public void CameraShake()
    {
        StopAllCoroutines();
        StartCoroutine(ScreenShake(0.015f, 0.3f));
    }

    IEnumerator ScreenShake(float wait, float intensity)
    {
        ShakeAmount = Vector2.zero;

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(wait);
            ShakeAmount = Vector2.up * intensity;

            yield return new WaitForSeconds(wait);
            ShakeAmount = Vector2.right * intensity;

            yield return new WaitForSeconds(wait);
            ShakeAmount = Vector2.down * intensity;

            yield return new WaitForSeconds(wait);
            ShakeAmount = Vector2.left * intensity;
        }
        ShakeAmount = Vector2.zero;

    }
}
