using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_move : MonoBehaviour
{
    public Transform tourCamera;
    #region ����ƶ�����
    /// <summary>
    /// ����ƶ��ٶ�
    /// </summary>
    public float moveSpeed = 1.0f;
    /// <summary>
    /// �����ת�ٶ�
    /// </summary>
    public float rotateSpeed = 90.0f;
    /// <summary>
    /// ����벻�ɴ����ı������С���루С�ڵ���0ʱ�ɴ�͸�κα��棩
    /// </summary>
    public float minDistance = 0.5f;
    #endregion

    #region �˶��ٶȺ���ÿ��������ٶȷ���
    private Vector3 direction = Vector3.zero;
    private Vector3 speedForward;
    private Vector3 speedBack;
    private Vector3 speedLeft;
    private Vector3 speedRight;
    private Vector3 speedUp;
    private Vector3 speedDown;
    /// <summary>
	/// ��ת�ӽ�ʱ���x��ת��
	/// </summary>
	private float xSpeed = 250.0f;
    /// <summary>
    /// ��ת�ӽ�ʱ���y��ת��
    /// </summary>
    private float ySpeed = 120.0f;
    /// <summary>
    /// y�����Ƕ�
    /// </summary>
    private int yMinLimit = -360;
    /// <summary>
    /// y����С�Ƕ�
    /// </summary>
    private int yMaxLimit = 360;
    /// <summary>
    /// �洢���x���ŷ����
    /// </summary>
    private float x = 0.0f;
    /// <summary>
    /// �洢�����euler��
    /// </summary>
    private float y = 0.0f;

    private Quaternion storeRotation; //�洢�������̬��Ԫ��
    /// <summary>
    /// �����x�᷽������
    /// </summary>
    private Vector3 cameraX;
    private Vector3 cameraY; //�����y�᷽������

    private Vector3 initScreenPos; //�м��հ���ʱ������Ļ����
    #endregion
    void Start()
    {
        if (tourCamera == null) tourCamera = gameObject.transform;
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        storeRotation = Quaternion.Euler(y + 0, x, 0);
        transform.rotation = storeRotation; //���������̬
                                            //��Ԫ����ʾһ����ת����Ԫ�����������൱�ڰ�������ת��Ӧ�Ƕȣ�Ȼ�����Ŀ�������λ�þ������λ����
    }
    void Update()
    {
        GetDirection();
        // ����Ƿ��벻�ɴ�͸�������
        RaycastHit hit;
        while (Physics.Raycast(tourCamera.position, direction, out hit, minDistance))
        {
            // ��ȥ��ֱ�ڲ��ɴ�͸������˶��ٶȷ���
            float angel = Vector3.Angle(direction, hit.normal);
            float magnitude = Vector3.Magnitude(direction) * Mathf.Cos(Mathf.Deg2Rad * (180 - angel));
            direction += hit.normal * magnitude;
        }
        tourCamera.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }
    private void GetDirection()
    {
        #region �����ƶ�
        // ��λ
        speedForward = Vector3.zero;
        speedBack = Vector3.zero;
        speedLeft = Vector3.zero;
        speedRight = Vector3.zero;
        speedUp = Vector3.zero;
        speedDown = Vector3.zero;
        // ��ȡ��������
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) speedForward = tourCamera.forward;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) speedBack = -tourCamera.forward;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) speedLeft = -tourCamera.right;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) speedRight = tourCamera.right;
        if (Input.GetKey(KeyCode.E)) speedUp = Vector3.up;
        if (Input.GetKey(KeyCode.Q)) speedDown = Vector3.down;
        direction = speedForward + speedBack + speedLeft + speedRight + speedUp + speedDown;
        #endregion

        #region ����Ҽ���ת����
        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            storeRotation = Quaternion.Euler(y + 0, x, 0);
            transform.rotation = storeRotation;
        }
        #endregion

        #region ������Ч��
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

        #region ����м�ƽ��
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

