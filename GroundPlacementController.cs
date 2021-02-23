using System;
using UnityEngine;
using Random=System.Random;
using UnityEngine.EventSystems;// Required when using Event data.
using UnityEngine.UI;

public class GroundPlacementController : MonoBehaviour//, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private GameObject[] placeableObjectPrefabs;

    private GameObject currentPlaceableObject;

    [SerializeField]
    private float mouseWheelRotation;
    private int currentPrefabIndex = -1;

    [SerializeField]
    private bool rotScaleMClickFlag = false;

    [SerializeField]
    private float scale = 13f;

    [SerializeField]
    private bool randomRClickRotScaleFlag = false;

    [SerializeField]
    private float minRandSize = 8f;
    [SerializeField]
    private float maxRandSize = 20f;
    [SerializeField]
    private float minRandRot = 0f;
    [SerializeField]
    private float maxRandRot = 360f;

    [SerializeField]
    private float dragTimeWait = .3f;
    private bool drag = false;
    private float timeCount = 0.0f;

    /*
    ** Main logic explained in comments
    */
    private void Update()
    {
        // Checks number key input to determine what asset or prefab to instantiate
        HandleNewObjectHotkey();
        // Handles the ability to drag and rapid place the current prefabs
        HandleDrag();
        // Flag to switch between random rotation and scale between
        //    (minRandSize-maxRandSize, minRandRot-maxRandRot)
        if (RandRotScaleIfRightClicked())
            randomRClickRotScaleFlag = randomRClickRotScaleFlag ? false : true;
        // Prevents overflow/large floats
        mouseWheelRotation %= 36f;
        if (currentPlaceableObject != null)
        {
            // Snaps object to mouse
            MoveCurrentObjectToMouse();
            // Flag to switch between scaling and rotating using mouse wheel
            if (RotScaleIfMiddleClicked())
                rotScaleMClickFlag = rotScaleMClickFlag ? false : true;
            // Rotates or scales with mouse wheel
            RotateScaleFromMouseWheel(rotScaleMClickFlag);
            // Releases object where left clicked
            drag = ReleaseIfLeftClicked();
        }
    }

    private void HandleDrag()
    {
        if (drag) {
            timeCount += Time.deltaTime;
            // Checks for left release to stop autoplacing
            if (Input.GetMouseButtonUp(0)) {
                Debug.Log("Left Released");
                timeCount = 0.0f;
                drag = false;
            }
            // Waits user modifiable float dragTimeWait till the next autoplace
            if (timeCount > dragTimeWait) {
                timeCount += Time.deltaTime;
                Debug.Log("Dragging...");
                timeCount = 0.0f;

                // Gets last placed object and places until left mouse released
                NewObject(placeableObjectPrefabs[currentPrefabIndex]);
                MoveCurrentObjectToMouse();
                if (RotScaleIfMiddleClicked())
                    rotScaleMClickFlag = rotScaleMClickFlag ? false : true;
                RotateScaleFromMouseWheel(rotScaleMClickFlag);
                currentPlaceableObject = null;
            }
        }
    }

    private void HandleNewObjectHotkey()
    {
        for (int i = 0; i < placeableObjectPrefabs.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + 1 + i))
            {
                if (PressedKeyOfCurrentPrefab(i))
                {
                    Destroy(currentPlaceableObject);
                    currentPrefabIndex = -1;
                }
                else
                {
                    if (currentPlaceableObject != null)
                    {
                        Destroy(currentPlaceableObject);
                    }
                    currentPrefabIndex = i;
                    NewObject(placeableObjectPrefabs[i]);
                }
                break;
            }
        }
    }

    private void NewObject(GameObject newPlaceableObjectPrefab)
    {
        currentPlaceableObject = Instantiate(newPlaceableObjectPrefab);
        if (randomRClickRotScaleFlag)
        {
            scale = RandomSizeScale(minRandSize, maxRandSize);
            mouseWheelRotation += RandomSizeScale(minRandRot / 10f, maxRandRot / 10f);
        }
    }

    private bool PressedKeyOfCurrentPrefab(int i)
    {
        return currentPlaceableObject != null && currentPrefabIndex == i;
    }

    private void MoveCurrentObjectToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            currentPlaceableObject.transform.position = hitInfo.point;
            currentPlaceableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            currentPlaceableObject.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void RotateScaleFromMouseWheel(bool rotScaleMClickFlag)
    {
        // Debug.Log(Input.mouseScrollDelta);
        if (rotScaleMClickFlag)
        {
            scale += Input.mouseScrollDelta.y;
            currentPlaceableObject.transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            mouseWheelRotation += Input.mouseScrollDelta.y;
            currentPlaceableObject.transform.Rotate(Vector3.up, mouseWheelRotation * 10f);
        }
    }

    private bool ReleaseIfLeftClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left Click!");
            currentPlaceableObject = null;
            return true;
        }
        return false;
    }

    private bool RotScaleIfMiddleClicked()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("Middle Click!");
            return true;
        }
        return false;
    }

    private bool RandRotScaleIfRightClicked()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Click!");
            return true;
        }
        return false;
    }

    private float RandomSizeScale(float minimum, float maximum)
    {
        Random rand = new Random();
        return (float)rand.NextDouble() * (maximum - minimum) + minimum;
    }
}