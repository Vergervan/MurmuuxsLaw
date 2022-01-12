using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    public float followSpeed = 2f;
    [SerializeField, HideInInspector] private float minX;
    [SerializeField, HideInInspector] private float maxX;
    void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, Camera.main.transform.position.y, -10);
        Vector3 slerpedPos = Vector3.Slerp(Camera.main.transform.position, newPos, followSpeed * Time.deltaTime);
        slerpedPos.y = newPos.y;
        slerpedPos.z = newPos.z;
        slerpedPos.x = Mathf.Clamp(slerpedPos.x, minX, maxX);
        Camera.main.transform.position = slerpedPos;
    }
    public void SetHorizontalCameraBorders(float left, float right)
    {
        minX = left;
        maxX = right;
    }
}
