using UnityEngine;
using Leap;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;

public class ColorSelector : MonoBehaviour {

    private Color[] colors = { Color.white, Color.red, Color.green, Color.blue, Color.black, Color.yellow,
        Color.grey, Color.cyan, Color.magenta};

    public GameObject prefab;
    private GameObject[] colorSelectors;

    private HandModel hand;

    private bool selectorsMade;
    private Vector3 FACING_CAMERA = new Vector3(350, 8, 153);

    
    public GameObject huePrefab, saturationPrefab, alphaPrefab;
    private GameObject hue, saturation, alpha;

    private GameObject leftHand, cam, lhc;

    // Use this for initialization
    void Start () {
        hand = transform.GetChild(2).GetComponent<HandModel>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        colorSelectors = new GameObject[8];
        selectorsMade = false;
        leftHand = transform.GetChild(2).gameObject;
        lhc = GameObject.FindGameObjectWithTag("HandController");
    }

    // Update is called once per frame
    void Update()
    {
        selector();
    }
    void selector()
    {
        Debug.Log(hand.palm.localEulerAngles);
        float difference = Vector3.Angle(hand.palm.localEulerAngles, FACING_CAMERA);

        if (difference < 8.0f && leftHand.activeSelf)
        {
            if (selectorsMade)
            {
                //Update the position, rotation, and color
                if (hue)
                {
                    hue.transform.position = hand.GetPalmPosition() + hand.GetPalmNormal() * 0.15f;
                    hue.transform.rotation = hand.GetPalmRotation();
                }
                if (alpha)
                {
                    alpha.transform.position = hand.GetPalmPosition() + hand.GetPalmNormal() * 0.15f;
                    alpha.transform.rotation = hand.GetPalmRotation();
                }
                if (saturation)
                {
                    saturation.transform.position = hand.GetPalmPosition() + hand.GetPalmNormal() * 0.15f;
                    saturation.transform.rotation = hand.GetPalmRotation();
                }
            }
            else
            {
                //Spawn in the light, 3 selectors, and sphere.
                //update each position, rotation, and color
                if (huePrefab)
                {
                    hue = (GameObject)Instantiate(huePrefab, hand.GetPalmPosition() + hand.GetPalmNormal() * 0.15f, hand.GetPalmRotation());
                }
                if (alphaPrefab)
                {
                    alpha = (GameObject)Instantiate(alphaPrefab);
                }
                if (saturationPrefab)
                {
                    saturation = (GameObject)Instantiate(saturationPrefab);
                }
                selectorsMade = true;
                lhc.BroadcastMessage("setInterface", true);
            }
        }
        else
        {
            if (selectorsMade)
            {
                if (!leftHand.activeSelf || difference >= 8.0f)
                {
                    if (hue)
                    {
                        Destroy(hue);
                    }
                    if (saturation)
                    {
                        Destroy(saturation);
                    }
                    if (alpha)
                    {
                        Destroy(alpha);
                    }
                    selectorsMade = false;
                }
                lhc.BroadcastMessage("setInterface", false);
            }
        }
    }
    void oldSelector()
    {
        float difference = Vector3.Angle(hand.palm.localEulerAngles, FACING_CAMERA);

        if (difference < 8.0f && leftHand.activeSelf)
        {
            if (selectorsMade)
            {
                for (int i = 0; i < colorSelectors.Length; i++)
                {
                    if (colorSelectors[i])
                    {
                        if (i < 5)
                        {
                            colorSelectors[i].transform.position = hand.fingers[i].bones[3].transform.position +
                                hand.fingers[i].GetBoneDirection(3) * 0.05f;
                        }
                        else
                        {
                            colorSelectors[i].transform.position = hand.palm.position - (hand.palm.position - hand.fingers[i % 5].bones[2].position) / 1.0f;

                        }
                    }
                    colorSelectors[i].transform.rotation = hand.GetPalmRotation();
                }

            }
            else
            {
                for (int i = 0; i < colorSelectors.Length; i++)
                {
                    colorSelectors[i] = Instantiate(prefab);
                    if (colorSelectors[i])
                    {
                        if (i < 5)
                        {
                            colorSelectors[i].transform.position = hand.fingers[i].bones[3].transform.position +
                                hand.fingers[i].GetBoneDirection(3) * 0.05f;
                        }
                        else
                        {
                            colorSelectors[i].transform.position = hand.palm.position - (hand.palm.position - hand.fingers[i % 5].bones[2].position) / 1.0f;

                        }
                    }
                    colorSelectors[i].transform.rotation = hand.GetPalmRotation();
                    colorSelectors[i].GetComponent<MeshRenderer>().material.color = colors[i];
                }
                selectorsMade = true;
            }
        }
        else
        {
            if (selectorsMade)
            {
                if (!leftHand.activeSelf || difference >= 8.0f)
                {
                    for (int i = 0; i < colorSelectors.Length; i++)
                    {
                        if (colorSelectors[i])
                        {
                            Destroy(colorSelectors[i]);
                        }
                    }
                    selectorsMade = false;
                }
            }
        }    
    }
}
