using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float parallaxEffect;

    private float length;
    private float xPosition;

    void Start()
    {
        cam = GameObject.Find("Main Camera");
        if(GetComponent<SpriteRenderer>())
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        else 
            length = 0;
        xPosition = transform.position.x;


    }

    
    void LateUpdate()
    {
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        if (distanceMoved > xPosition + length)
        {
            xPosition = xPosition + length;
        }
    }
}
