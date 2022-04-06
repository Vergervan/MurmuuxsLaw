using UnityEngine;
[System.Serializable]
public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 2f;
    [SerializeField, HideInInspector] private float minX, maxX, minY, maxY;
    void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y, -10);
        Vector3 slerpedPos = Vector3.Slerp(Camera.main.transform.position, newPos, followSpeed * Time.deltaTime);
        slerpedPos.y = Mathf.Clamp(slerpedPos.y, minY, maxY);
        slerpedPos.z = newPos.z;
        slerpedPos.x = Mathf.Clamp(slerpedPos.x, minX, maxX);
        Camera.main.transform.position = slerpedPos;
    }
}
