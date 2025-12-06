using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchsCOntroller : MonoBehaviour
{
    [Header("女巫產生器")]
    [SerializeField] private GameObject[] Witchs;
    [SerializeField] private int standbyNumber;
    [SerializeField] private Transform CreatePoint;


    // Start is called before the first frame update
    void Start()
    {
        cerate();
    }

    void cerate()
    {
        Vector2 create_position = CreatePoint.position;

        for (int i = 0; i < standbyNumber; i++)
        {
            int random_witch = Random.Range(0, Witchs.Length);
            // print(Witchs[random_witch]);

            Instantiate(Witchs[random_witch], create_position, CreatePoint.rotation, CreatePoint);
            create_position.x -= 2;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
