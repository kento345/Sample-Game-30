using System.Threading;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabes;
    private Vector3 pos;
    float timer = 0f;
    float SpawnTime = 1f;

    void Start()
    {
        pos = new Vector3(15,1,15);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
    }

    void SpawnItem()
    {
        int i = Random.Range(0, itemPrefabes.Length);
        Instantiate(itemPrefabes[i], pos, Quaternion.identity);
    }
}
