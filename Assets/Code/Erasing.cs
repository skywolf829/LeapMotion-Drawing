using UnityEngine;
using System.Collections;

public class Erasing : MonoBehaviour {

    private const float lifeSpan = 0.07f;
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
    }
    void Update()
    {
        if(Time.time > spawnTime + lifeSpan)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider c) {
        GameObject thing = c.gameObject;
        if (thing && thing.tag == "DrawingObject")
        {
            Destroy(c.gameObject);
            Destroy(gameObject);
        }
    }
}
