using UnityEngine;
using System.Collections;
using Leap.Unity;

public class HandListener : MonoBehaviour
{
    private const float MAX_ANGLE_DIFFERENCE_FOR_INTERFACE = 40.0f;
    private const float MIN_ERASE_VELOCITY = 1.25f;
    private const float ERASE_SCALE = 8.0f;
    private const float DRAW_WIDTH = 0.02f;
    private const float MIN_PINCH_DISTANCE = 0.01f;
    private const float MAX_PINCH_DISTANCE = 0.08f;
    private const float PINCH_START = 0.8f;
    private const float PINCH_TO_CONTINUE = 0.6f;
    private const float SLIDER_HEIGHT = 1.27f;

    public GameObject LeapHandController, mainCamera;
    public GameObject drawingPrefab, huePrefab, saturationPrefab, alphaPrefab, rightIndexFingerLight;

    private GameObject leftHand, rightHand;
    private GameObject hue, saturation, alpha;

    private HandModel leftHandModel, rightHandModel;

    private Color savedColor = Color.white;

    private Vector3 lastRightHandPos;

    private float hueSliderHeightPercentage;

    private bool rightHandIndexPinching, leftHandInterfaceActive, drawingPositionReset, rightIndexLightEnabled;

    void Start()
    {
        if (LeapHandController)
        {
            leftHandModel = LeapHandController.transform.GetChild(2).GetComponent<HandModel>();
            rightHandModel = LeapHandController.transform.GetChild(3).GetComponent<HandModel>();
            leftHand = LeapHandController.transform.GetChild(2).gameObject;
            rightHand = LeapHandController.transform.GetChild(3).gameObject;
        }
    }

    void Update()
    {
        if (leftHandModel)
        {
            updateLeftHand();

        }
        if (rightHandModel)
        {
            updateRightHand();
        }
    }

    void updateLeftHand()
    {
        Vector3 upCam = mainCamera.transform.up;
        float difference = Vector3.Angle(leftHandModel.GetPalmNormal(), upCam);
        
        if (difference < MAX_ANGLE_DIFFERENCE_FOR_INTERFACE && leftHand.activeSelf)
        {
            if (leftHandInterfaceActive)
            {
                //Update the position, rotation, and color
                if (hue && hue.activeSelf)
                {
                    hue.transform.position = leftHandModel.GetPalmPosition() + leftHandModel.GetPalmNormal() * 0.15f;
                    hue.transform.rotation = leftHandModel.GetPalmRotation();
                }
                if (alpha && alpha.activeSelf)
                {
                    alpha.transform.position = leftHandModel.GetPalmPosition() + leftHandModel.GetPalmNormal() * 0.15f;
                    alpha.transform.rotation = leftHandModel.GetPalmRotation();
                }
                if (saturation && saturation.activeSelf)
                {
                    saturation.transform.position = leftHandModel.GetPalmPosition() + leftHandModel.GetPalmNormal() * 0.15f;
                    saturation.transform.rotation = leftHandModel.GetPalmRotation();
                }
            }
            else
            {
                //Spawn in the light, 3 selectors, and sphere.
                //update each position, rotation, and color
                if (hue)
                {
                    hue.transform.position = leftHandModel.GetPalmPosition() + leftHandModel.GetPalmNormal() * 0.15f;
                    hue.transform.rotation = leftHandModel.GetPalmRotation();
                    hue.SetActive(true);
                }

                else if (huePrefab)
                {
                    hue = (GameObject)Instantiate(huePrefab, 
                        leftHandModel.GetPalmPosition() + leftHandModel.GetPalmNormal() * 0.15f,
                        leftHandModel.GetPalmRotation());
                    hueSliderHeightPercentage = 0.5f;
                    updateColorSelected();
                }

                if (alpha)
                {
                    alpha.SetActive(true);
                }

                else if (alphaPrefab)
                {
                    alpha = (GameObject)Instantiate(alphaPrefab, 
                        leftHandModel.GetPalmPosition() + leftHandModel.GetPalmNormal() * 0.15f,
                        leftHandModel.GetPalmRotation());
                }

                if (saturation)
                {
                    saturation.SetActive(true);
                }

                else if (saturationPrefab)
                {
                    saturation = (GameObject)Instantiate(saturationPrefab,
                        leftHandModel.GetPalmPosition() + leftHandModel.GetPalmNormal() * 0.15f,
                        leftHandModel.GetPalmRotation());
                }
                leftHandInterfaceActive = true;
            }
        }
        else
        {
            if (leftHandInterfaceActive)
            {
                if (!leftHand.activeSelf || difference >= MAX_ANGLE_DIFFERENCE_FOR_INTERFACE)
                {
                    if (hue)
                    {
                        hue.SetActive(false);
                    }
                    if (saturation)
                    {
                        saturation.SetActive(false);
                    }
                    if (alpha)
                    {
                        alpha.SetActive(false);
                    }
                    leftHandInterfaceActive = false;
                }
            }
        }
    }

    void updateRightHand()
    {
        updateRightIndexPinching();
        updateLight();
        rightHandActions();
    }

    void updateRightIndexPinching()
    {
        if (!rightHand.activeSelf)
        {
            rightHandIndexPinching = false;
        }
        else
        {
            Vector3 indexPosition = rightHandModel.fingers[1].GetBoneCenter(3);
            Vector3 thumbPosition = rightHandModel.fingers[0].GetBoneCenter(3);

            float distance = (indexPosition - thumbPosition).magnitude;
            float normalizedDistance = (distance - MIN_PINCH_DISTANCE) / (MAX_PINCH_DISTANCE - MIN_PINCH_DISTANCE);
            float pinch = 1.0f - Mathf.Clamp01(normalizedDistance);

            if (!rightHandIndexPinching && pinch > PINCH_START)
            {
                rightHandIndexPinching = true;
            }
            else if (rightHandIndexPinching && pinch < PINCH_TO_CONTINUE)
            {
                rightHandIndexPinching = false;
            }
        }
    }

    void updateLight()
    {
        if (rightHandIndexPinching)
        {
            if (rightIndexLightEnabled)
            {
                rightIndexFingerLight.SetActive(false);
                rightIndexLightEnabled = false;
            }
        }
        else
        {
            if (rightIndexLightEnabled)
            {
                rightIndexFingerLight.transform.position = rightHandModel.fingers[1].bones[3].position;
                rightIndexFingerLight.transform.rotation = rightHandModel.fingers[1].bones[3].rotation;
                rightIndexFingerLight.GetComponent<Light>().color = savedColor;
            }

            else
            {
                rightIndexFingerLight.SetActive(true);
                rightIndexLightEnabled = true;
                rightIndexFingerLight.transform.position = rightHandModel.fingers[1].bones[3].position;
                rightIndexFingerLight.transform.rotation = rightHandModel.fingers[1].bones[3].rotation;
                rightIndexFingerLight.GetComponent<Light>().color = savedColor;
            }
        }        
    }
    

    void rightHandActions()
    {
        if (leftHandInterfaceActive)
        {
            if (rightHandIndexPinching)
            {
                moveHueSlider();
                updateColorSelected(); 
            }
            drawingPositionReset = false;
        }
        else
        {
            if (rightHandIndexPinching)
            {
                draw();
            }
            else if(rightHandModel.palm.gameObject.GetComponent<Rigidbody>().velocity.magnitude > MIN_ERASE_VELOCITY)
            {
                erase();
            }
            else
            {
                drawingPositionReset = false;
            }
        }
    }

    void draw()
    {
        if (!drawingPositionReset)
        {
            lastRightHandPos = rightHandModel.fingers[0].GetBoneCenter(3);
            drawingPositionReset = true;
        }
        else if(drawingPrefab)
        {
            Vector3 currentPos = rightHandModel.fingers[0].GetBoneCenter(3);
            Vector3 currV = rightHandModel.fingers[0].bones[3].GetComponent<Rigidbody>().velocity;
            
            Quaternion dir = Quaternion.LookRotation(currV);
            
            GameObject currentCylinder = (GameObject)Instantiate(drawingPrefab,
                new Vector3((lastRightHandPos.x + currentPos.x) / 2.0f,
                            (lastRightHandPos.y + currentPos.y) / 2.0f,
                            (lastRightHandPos.z + currentPos.z) / 2.0f),
                dir);

            currentCylinder.transform.Rotate(new Vector3(1, 0, 0), 90);
            currentCylinder.transform.localScale = new Vector3(DRAW_WIDTH, (currentPos - lastRightHandPos).magnitude / 2, DRAW_WIDTH);

            lastRightHandPos = currentPos;

            currentCylinder.GetComponent<MeshRenderer>().material.color = savedColor;
        }
    }

    void moveHueSlider()
    {
        Vector3 averagePinchPoint = (mainCamera.GetComponent<Camera>().WorldToScreenPoint(rightHandModel.fingers[1].bones[3].transform.position) +
            mainCamera.GetComponent<Camera>().WorldToScreenPoint(rightHandModel.fingers[0].bones[3].transform.position)) / 2.0f;
        int screenHeight = Screen.height;
        float percentageOfHeight = Mathf.Clamp01(averagePinchPoint.y / screenHeight);
        if (hue)
        {
            hue.transform.GetChild(0).transform.localPosition = 
                new Vector3(0,-(SLIDER_HEIGHT * 2.0f * percentageOfHeight) + SLIDER_HEIGHT,0);
        }
        hueSliderHeightPercentage = percentageOfHeight;
    }

    void updateColorSelected()
    {
        Texture2D t = hue.GetComponent<SpriteRenderer>().sprite.texture;
        if (t)
        {
            savedColor = t.GetPixel(0, (int)(t.height - (hueSliderHeightPercentage * t.height)));
        }
    }

    public void erase()
    {
        if (!drawingPositionReset)
        {
            lastRightHandPos = rightHandModel.GetPalmPosition();
            drawingPositionReset = true;
        }
        else if(drawingPrefab)
        {
            Vector3 currentPos = rightHandModel.GetPalmPosition();
            Vector3 currV = rightHandModel.palm.GetComponent<Rigidbody>().velocity;

            Quaternion dir = Quaternion.LookRotation(currV);

            GameObject currentCylinder = (GameObject)Instantiate(drawingPrefab,
                new Vector3((lastRightHandPos.x + currentPos.x) / 2.0f,
                            (lastRightHandPos.y + currentPos.y) / 2.0f,
                            (lastRightHandPos.z + currentPos.z) / 2.0f),
                dir);

            currentCylinder.transform.Rotate(new Vector3(1, 0, 0), 90);
            currentCylinder.transform.localScale = new Vector3(DRAW_WIDTH * ERASE_SCALE, 
                (currentPos - lastRightHandPos).magnitude / 2, DRAW_WIDTH * ERASE_SCALE);

            lastRightHandPos = currentPos;

            currentCylinder.GetComponent<MeshRenderer>().enabled = false;
            currentCylinder.tag = "ErasingObject";
            currentCylinder.GetComponent<Collider>().isTrigger = true;
            currentCylinder.AddComponent<Rigidbody>();
            currentCylinder.AddComponent<Erasing>();
        }
    }    
}