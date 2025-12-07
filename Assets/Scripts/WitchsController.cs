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
    Vector2 create_position;

    // Start is called before the first frame update
    void Start()
    {
        create_position = CreatePoint.position;
        cerate(standbyNumber);
        
    }

    void cerate(int number)
    {
        

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

        if(WitchList.Count <= 5){
            create_position.x = WitchList[WitchList.Count-1].transform.position.x -2;
            cerate(5);
        }


        for (int i = 0; i < WitchList.Count; i++)
        {
            Rigidbody2D rb = WitchList[i].GetComponent<Rigidbody2D>();

            if (rb != null)
            {

                Vector3 targetPosition = rb.position;

                if (targetPosition.x +2 < -4)
                {
                    targetPosition.x += 2;
                    rb.MovePosition(targetPosition);
                    WitchList[i].SendMessage("update_origin_position", targetPosition);
                }
                else
                {
                    Debug.Log(targetPosition.x);
                }



            }
            else
            {
                Debug.Log("can't get rigidbody");
            }
        }
    }
}
