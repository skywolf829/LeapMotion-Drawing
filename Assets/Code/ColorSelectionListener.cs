using UnityEngine;
using System.Collections;

public class colorSelectionListener : MonoBehaviour {

    GameObject lhc;

	// Use this for initialization
	void Start () {
        lhc = GameObject.FindGameObjectWithTag("HandController");
	}

    void OnCollisionEnter(Collision c)
    {
        oldCollisionDetection(c);
    }
    void oldCollisionDetection(Collision c)
    {
        if (c.collider.tag == "rightFinger")
        {
            if (lhc)
            {
                lhc.BroadcastMessage("changeColor", gameObject.GetComponent<MeshRenderer>().material.color);
            }
        }
    }
}
