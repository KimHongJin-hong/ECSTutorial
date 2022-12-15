using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    public static Camera Instance;

    private void Awake()
    {
        Instance = GetComponent<Camera>();
    }

    public static Vector3 ScreenToViewportPoint(Vector3 screenPoint)
    {
        return new Vector3(screenPoint.x / Screen.width, screenPoint.y / Screen.height, screenPoint.z);
    }
}
