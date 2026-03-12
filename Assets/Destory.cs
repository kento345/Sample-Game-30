using UnityEngine;

public class Destory : MonoBehaviour
{
    float timer = 0;
    float count = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = count;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
