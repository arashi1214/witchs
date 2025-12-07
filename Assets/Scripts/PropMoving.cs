using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropMoving : MonoBehaviour
{
    public float speed = 5;
    public float distance = 2;
    private float StartPoint;
    private float EndPoint;
    private SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        StartPoint = transform.position.x;
        EndPoint = transform.position.x - distance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void FixedUpdate()
    {
        if (transform.position.x >= StartPoint || transform.position.x <= EndPoint)
        {
            speed = speed * -1;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        float moveAmount = speed * Time.deltaTime;

        Vector3 currentPosition = transform.position;
        currentPosition.x += moveAmount;
        transform.position = currentPosition;
    }
}
