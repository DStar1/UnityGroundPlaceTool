using System;
using UnityEngine;
using Random=System.Random;

public class GroundPlacementController : MonoBehaviour
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

    private void Update()
    {
        HandleNewObjectHotkey();
        if (RandRotScaleIfRightClicked())
            randomRClickRotScaleFlag = randomRClickRotScaleFlag ? false : true;
        
        mouseWheelRotation %= 36f;

        if (currentPlaceableObject != null)
        {
            MoveCurrentObjectToMouse();
            if (RotScaleIfMiddleClicked())
                rotScaleMClickFlag = rotScaleMClickFlag ? false : true;
            RotateScaleFromMouseWheel(rotScaleMClickFlag);
            ReleaseIfLeftClicked();
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
                    currentPlaceableObject = Instantiate(placeableObjectPrefabs[i]);
                    currentPrefabIndex = i;
                    if (randomRClickRotScaleFlag)
                    {
                        scale = RandomSizeScale(minRandSize, maxRandSize);
                        mouseWheelRotation += RandomSizeScale(minRandRot / 10f, maxRandRot / 10f);
                    }
                }

                break;
            }
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

    private void ReleaseIfLeftClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left Click!");
            currentPlaceableObject = null;
        }
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