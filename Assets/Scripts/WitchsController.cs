using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchsController : MonoBehaviour
{
    [Header("女巫生成資訊")]
    [SerializeField] private GameObject[] Witchs;
    [SerializeField] private int standbyNumber;
    [SerializeField] private Transform CreatePoint;

    private List<GameObject> WitchList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        cerate(standbyNumber);
    }

    void cerate(int number)
    {
        Vector2 create_position = CreatePoint.position;

        for (int i = 0; i < number; i++)
        {
            int random_witch = Random.Range(0, Witchs.Length);
            // print(Witchs[random_witch]);

            GameObject newWitch = Instantiate(Witchs[random_witch], create_position, CreatePoint.rotation, CreatePoint);
            WitchList.Add(newWitch);
            create_position.x -= 2;
        }
    }

    public void onward(GameObject remove_witch)
    {
        Debug.Log("前進");
        WitchList.Remove(remove_witch);

        for (int i = 0; i < WitchList.Count; i++)
        {
            Rigidbody2D rb = WitchList[i].GetComponent<Rigidbody2D>();

            if (rb != null)
            {

                Vector3 targetPosition = rb.position; // �ϥ� rb.position ������z��m

                targetPosition.x += 2;

                rb.MovePosition(targetPosition);

                WitchList[i].SendMessage("update_origin_position", targetPosition);

            }
            else
            {
                Debug.Log("can't get rigidbody");
            }
        }
    }
}
