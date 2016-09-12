using UnityEngine;
using System.Collections;
using Leap.Unity;

public class Drawing : MonoBehaviour {

    private const float WIDTH = 0.03f;

    private bool pinching;
    private bool reset;
    private bool interfaceActive;

    private int everyOther = 0;

    private float minPinchDistance;
    private float maxPinchDistance;

    private float pinchStart;
    private float pinchToContinue;

    private Vector3 lastPos;
    
    
    public GameObject prefab;
    private HandModel hand;
    private GameObject l;

    Material mat;
    Color savedColor = Color.yellow;

	// Use this for initialization
	void Start () {
        minPinchDistance = 0.01f;
        maxPinchDistance = 0.12f;
        pinchStart = 0.8f;
        pinchToContinue = 0.6f;
        pinching = false;
        interfaceActive = false;
        hand = transform.GetChild(3).GetComponent<HandModel>();
        l = transform.GetChild(4).gameObject;

    }
	void setInterface(bool b)
    {
        interfaceActive = b;
    }

	// Update is called once per frame
	void FixedUpdate () {
        updatePinching();        
        draw2();
        updateLight();
    }

    void updatePinching()
    {
        Vector3 indexPosition = hand.fingers[1].GetBoneCenter(3);
        Vector3 thumbPosition = hand.fingers[0].GetBoneCenter(3);

        float distance = (indexPosition - thumbPosition).magnitude;
        float normalizedDistance = (distance - minPinchDistance) / (maxPinchDistance - minPinchDistance);
        float pinch = 1.0f - Mathf.Clamp01(normalizedDistance);

        if (!pinching && pinch > pinchStart)
        {
            pinching = true;
        }
        else if (pinching && pinch < pinchToContinue)
        {
            pinching = false;
        }
    }
    void changeColor(Color c)
    {
        savedColor = c;
    }

    void updateLight()
    {
        if (l)
        {
            l.transform.position = hand.fingers[1].bones[3].position;
            l.transform.rotation = hand.fingers[1].bones[3].rotation;
            l.GetComponent<Light>().color = savedColor;
        }
    }
    
    void draw1()
    {
        if (pinching)
        {
            Quaternion dir = Quaternion.LookRotation(hand.fingers[0].bones[3].GetComponent<Rigidbody>().velocity);
            Instantiate(prefab, hand.fingers[0].GetBoneCenter(3), dir);
            
        }
    }
    
    void draw2()
    {
        if (pinching && !interfaceActive)
        {
            /*
            *   Reset the position of the hand every time pinching stops, so that no objects are made starting
            *   from the last point pinched or  0, 0, 0.
            */
            if (!reset)
            {
                lastPos = hand.fingers[0].GetBoneCenter(3);
                reset = true;
            }
            else 
            {
                Vector3 currentPos = hand.fingers[0].GetBoneCenter(3);
                Vector3 currV = hand.fingers[0].bones[3].GetComponent<Rigidbody>().velocity;

                /*
                *   The direction the hand is moving toward.
                */
                Quaternion dir = Quaternion.LookRotation(currV);

                /*
                *   Create the prefab in the desired place with the rotation in the direction of the 
                *   hand's velocity.
                */
                GameObject c = (GameObject)Instantiate(prefab, 
                    new Vector3((lastPos.x + currentPos.x) / 2.0f,
                                (lastPos.y + currentPos.y) / 2.0f,
                                (lastPos.z + currentPos.z) / 2.0f),
                    dir);

                /*
                *   Rotate the object 90 degrees on its x axis
                */
                c.transform.Rotate(new Vector3(1, 0, 0), 90);

                /*
                *   And then set the localscale relative to the distance traveled by the hand.
                */
                c.transform.localScale = new Vector3(WIDTH, (currentPos - lastPos).magnitude / 2, WIDTH);

                lastPos = currentPos;

                
                mat = c.GetComponent<MeshRenderer>().material;
                mat.color = savedColor;
            }
        }
        else if(pinching && interfaceActive)
        {

        }
        else
        {
            reset = false;
            everyOther = 0;
        }
    }
}
