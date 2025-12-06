using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witchs : MonoBehaviour
{
    [Header("祇甮砞﹚")]
    [SerializeField] private float maxDragDistance = 3f; // 程╈Σ禯瞒
    [SerializeField] private float forceMultiplier = 5f; // 秖计

    [Header("瓂格箇代")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryPointCount = 30;
    [SerializeField] private float trajectoryTimeStep = 0.1f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 dragPosition;
    private bool isDragging = false;
    private bool isLaunched = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // 祇甮玡玂繰ゎ
        startPosition = transform.position;

        if (trajectoryLine)
        {
            trajectoryLine.positionCount = trajectoryPointCount;
            trajectoryLine.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLaunched) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(mousePos, transform.position) < 1f)
            {
                isDragging = true;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = startPosition - dragPosition;

            // ╈Σ禯瞒
            if (direction.magnitude > maxDragDistance)
            {
                direction = direction.normalized * maxDragDistance;
            }

            transform.position = startPosition - direction;

            // 陪ボ瓂格
            if (trajectoryLine)
            {
                ShowTrajectory(direction * forceMultiplier);
                trajectoryLine.enabled = true;
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Launch();
        }
    }

    void Launch()
    {
        isDragging = false;
        isLaunched = true;

        Vector2 launchForce = (startPosition - (Vector2)transform.position) * forceMultiplier;

        rb.isKinematic = false;
        rb.AddForce(launchForce, ForceMode2D.Impulse);

        if (trajectoryLine)
        {
            trajectoryLine.enabled = false;
        }
    }

    void ShowTrajectory(Vector2 velocity)
    {
        Vector2 position = transform.position;

        for (int i = 0; i < trajectoryPointCount; i++)
        {
            float time = i * trajectoryTimeStep;
            Vector2 point = position + velocity * time + 0.5f * Physics2D.gravity * time * time;
            trajectoryLine.SetPosition(i, point);
        }
    }
}
