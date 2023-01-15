using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float smoothSpeed = 0.125f; //摄像机平滑移动的速度
    public Vector3 offset = new Vector3(0f, 10f, -10f); //摄像机与目标物体的默认距离
    public float lookAtHeight = 1f; //相机看向目标物体的高度
    public float angle = 45f; //斜俯视角度
    private Transform target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform; //在场景中找到tag为Player的物体
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset; //计算出相机应该移动到的位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); //平滑移动
        transform.position = smoothedPosition;

        Vector3 lookAtPosition = target.position + new Vector3(0f, lookAtHeight, 0f); //计算相机看向的位置
        transform.LookAt(lookAtPosition); //让相机看向该位置
        transform.rotation = Quaternion.Euler(new Vector3(angle, transform.rotation.eulerAngles.y, 0f)); //锁定相机的旋转角度
    }
}
