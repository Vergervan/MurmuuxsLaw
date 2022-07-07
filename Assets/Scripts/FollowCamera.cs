using UnityEngine;
[System.Serializable]
public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float zoom = 1f;
    private float _defFollowSpeed, _defZoom;
    [SerializeField, HideInInspector] private float minX, maxX, minY, maxY;
    private Camera _camera;

    private bool makeZoom = false;
    private float zoomLerpVal;

    private void Awake()
    {
        _defFollowSpeed = followSpeed;
        _defZoom = zoom;
        zoomLerpVal = _defZoom;
        _camera = Camera.main;
    }
    void Update()
    {
        if (makeZoom)
        {
            ZoomCamera();
        }

        Vector3 newPos = new Vector3(target.position.x, target.position.y, _camera.transform.position.z);
        Vector3 slerpedPos = Vector3.Slerp(_camera.transform.position, newPos, followSpeed * Time.deltaTime);
        slerpedPos.y = Mathf.Clamp(slerpedPos.y, minY, maxY);
        slerpedPos.z = newPos.z;
        slerpedPos.x = Mathf.Clamp(slerpedPos.x, minX, maxX);
        _camera.transform.position = slerpedPos;
    }

    private void ZoomCamera()
    {
        zoomLerpVal = Mathf.Lerp(zoomLerpVal, zoom, followSpeed * Time.deltaTime);
        _camera.orthographicSize = zoomLerpVal;
        if(zoomLerpVal == zoom)
        {
            makeZoom = false;
        }
    }

    public void SetTarget(Transform target) => this.target = target;

    public void SetFollowSpeed(float speed)
    {
        followSpeed = speed;
    }
    public void SetZoom(float zoom)
    {
        this.zoom = zoom;
        makeZoom = true;
    }
    public void ResetValues()
    {
        followSpeed = _defFollowSpeed;
        zoom = _defZoom;
    }
}
