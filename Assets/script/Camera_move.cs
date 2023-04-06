using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_move : MonoBehaviour
{
    public Transform tourCamera;
    #region 相机移动参数
    /// <summary>
    /// 相机移动速度
    /// </summary>
    public float moveSpeed = 1.0f;
    /// <summary>
    /// 相机旋转速度
    /// </summary>
    public float rotateSpeed = 90.0f;
    /// <summary>
    /// 相机离不可穿过的表面的最小距离（小于等于0时可穿透任何表面）
    /// </summary>
    public float minDistance = 0.5f;
    #endregion

    #region 运动速度和其每个方向的速度分量
    private Vector3 direction = Vector3.zero;
    private Vector3 speedForward;
    private Vector3 speedBack;
    private Vector3 speedLeft;
    private Vector3 speedRight;
    private Vector3 speedUp;
    private Vector3 speedDown;
    /// <summary>
	/// 旋转视角时相机x轴转速
	/// </summary>
	private float xSpeed = 250.0f;
    /// <summary>
    /// 旋转视角时相机y轴转速
    /// </summary>
    private float ySpeed = 120.0f;
    /// <summary>
    /// y轴最大角度
    /// </summary>
    private int yMinLimit = -360;
    /// <summary>
    /// y轴最小角度
    /// </summary>
    private int yMaxLimit = 360;
    /// <summary>
    /// 存储相机x轴的欧拉角
    /// </summary>
    private float x = 0.0f;
    /// <summary>
    /// 存储相机的euler角
    /// </summary>
    private float y = 0.0f;

    private Quaternion storeRotation; //存储相机的姿态四元数
    /// <summary>
    /// 相机的x轴方向向量
    /// </summary>
    private Vector3 cameraX;
    private Vector3 cameraY; //相机的y轴方向向量

    private Vector3 initScreenPos; //中键刚按下时鼠标的屏幕坐标
    #endregion
    void Start()
    {
        if (tourCamera == null) tourCamera = gameObject.transform;
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        storeRotation = Quaternion.Euler(y + 0, x, 0);
        transform.rotation = storeRotation; //设置相机姿态
                                            //四元数表示一个旋转，四元数乘以向量相当于把向量旋转对应角度，然后加上目标物体的位置就是相机位置了
    }
    void Update()
    {
        GetDirection();
        // 检测是否离不可穿透表面过近
        RaycastHit hit;
        while (Physics.Raycast(tourCamera.position, direction, out hit, minDistance))
        {
            // 消去垂直于不可穿透表面的运动速度分量
            float angel = Vector3.Angle(direction, hit.normal);
            float magnitude = Vector3.Magnitude(direction) * Mathf.Cos(Mathf.Deg2Rad * (180 - angel));
            direction += hit.normal * magnitude;
        }
        tourCamera.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }
    private void GetDirection()
    {
        #region 键盘移动
        // 复位
        speedForward = Vector3.zero;
        speedBack = Vector3.zero;
        speedLeft = Vector3.zero;
        speedRight = Vector3.zero;
        speedUp = Vector3.zero;
        speedDown = Vector3.zero;
        // 获取按键输入
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) speedForward = tourCamera.forward;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) speedBack = -tourCamera.forward;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) speedLeft = -tourCamera.right;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) speedRight = tourCamera.right;
        if (Input.GetKey(KeyCode.E)) speedUp = Vector3.up;
        if (Input.GetKey(KeyCode.Q)) speedDown = Vector3.down;
        direction = speedForward + speedBack + speedLeft + speedRight + speedUp + speedDown;
        #endregion

        #region 鼠标右键旋转功能
        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            storeRotation = Quaternion.Euler(y + 0, x, 0);
            transform.rotation = storeRotation;
        }
        #endregion

        #region 鼠标滚轮效果
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= 60)
                Camera.main.fieldOfView += 2;
            if (Camera.main.orthographicSize <= 20)
                Camera.main.orthographicSize += 0.5F;
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView > 2)
                Camera.main.fieldOfView -= 2;
            if (Camera.main.orthographicSize >= 1)
                Camera.main.orthographicSize -= 0.5F;
        }
        #endregion

        #region 鼠标中键平移
        if (Input.GetMouseButtonDown(2))
        {
            cameraX = transform.right;
            cameraY = transform.up;

            initScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        }

        if (Input.GetMouseButton(2))
        {
            var mPosition = -0.005f * ((Input.mousePosition.x - initScreenPos.x) * cameraX + 0.5f * (Input.mousePosition.y - initScreenPos.y) * cameraY);
            initScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
            transform.position += mPosition;
        }
        #endregion
    }
    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}

