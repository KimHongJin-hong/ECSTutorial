using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BVHTree : MonoBehaviour
{
    public Vector3 Upper;
    public Vector3 Lower;
    public Vector3 center;
    public Quaternion quaternion;
    public GameObject test;
    public Vector3 startPosition;

    public GameObject start;
    public GameObject end;
    private bool isStartDrag;
    private void Update()
    {
        var mainCamera = Camera.main;
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            var ray = mainCamera.ScreenPointToRay(startPosition);
            isStartDrag = Physics.Raycast(ray, out RaycastHit hit);
            if (isStartDrag)
            {
                startPosition = hit.point;
                start.transform.position = startPosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isStartDrag)
            {
                Vector2 endMousePosition = Input.mousePosition;

                var ray = mainCamera.ScreenPointToRay(endMousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 endPosition = hit.point;
                    end.transform.position = endPosition;
                    Upper = Vector3.Max(startPosition, endPosition);
                    Lower = Vector3.Min(startPosition, endPosition);
                    Vector3 extents = (Upper - Lower);
                    Vector3 halfExtents = extents * 0.5f;
                    Vector3 center = Lower + halfExtents;
                    test.transform.localScale = extents;
                    //test.transform.rotation = mainCamera.transform.rotation;
                    test.transform.position = center;
                }

            }
        }
    }
}
