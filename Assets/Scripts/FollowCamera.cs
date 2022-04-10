using UnityEngine;
[System.Serializable]
public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float zoom = 0f;
    private float _defFollowSpeed, _defZoom;
    [SerializeField, HideInInspector] private float minX, maxX, minY, maxY;
    private void Awake()
    {
        _defFollowSpeed = followSpeed;
        _defZoom = zoom;
    }
    void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y, Camera.main.transform.position.z / (zoom == 0f ? 1f : zoom));
        Vector3 slerpedPos = Vector3.Slerp(Camera.main.transform.position, newPos, followSpeed * Time.deltaTime);
        slerpedPos.y = Mathf.Clamp(slerpedPos.y, minY, maxY);
        slerpedPos.z = newPos.z;
        slerpedPos.x = Mathf.Clamp(slerpedPos.x, minX, maxX);
        Camera.main.transform.position = slerpedPos;
    }

    public void SetFollowSpeed(float speed)
    {
        followSpeed = speed;
    }
    public void SetZoom(float zoom)
    {
        this.zoom = zoom;
    }
    public void ResetValues()
    {
        followSpeed = _defFollowSpeed;
        zoom = _defZoom;
    }
}
