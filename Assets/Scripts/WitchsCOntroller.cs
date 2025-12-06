using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchsController : MonoBehaviour
{
    [Header("女巫產生器")]
    [SerializeField] private GameObject[] Witchs;
    [SerializeField] private int standbyNumber;
    [SerializeField] private Transform CreatePoint;

    private List<GameObject> WitchList = new List<GameObject>();

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

            GameObject newWitch = Instantiate(Witchs[random_witch], create_position, CreatePoint.rotation, CreatePoint);
            WitchList.Add(newWitch);
            create_position.x -= 2;
        }
    }

    public void onward()
    {
        Debug.Log("前進 (物理)");

        for (int i = 0; i < WitchList.Count; i++)
        {
            // 1. 取得 Rigidbody 元件
            Rigidbody2D rb = WitchList[i].GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // 2. 計算目標位置
                Vector3 targetPosition = rb.position; // 使用 rb.position 獲取物理位置
                targetPosition.x += 2;

                // 3. 使用 MovePosition 函式告訴物理引擎移動
                rb.MovePosition(targetPosition);
                Debug.Log("新物理位置: " + targetPosition);
            }
            else
            {
                Debug.Log("can't get rigidbody");
            }
        }
    }
}
