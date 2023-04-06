using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConveyorBelt : MonoBehaviour
{
    public Toggle toggle;
    /// <summary>
    /// �������
    /// </summary>
    private Rigidbody rigid;
    /// <summary>
    /// �ٶ�
    /// </summary>
    public float speed = 0.01f;
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        toggle.isOn = true;
    }
    private void FixedUpdate()
    {
        PhysicConveyors();
    }
    private void PhysicConveyors()
    {
        if (toggle.isOn)
        {
            //��¼�����ʼλ�ø���
            var pos = rigid.position;
            //������z�����귽���ƶ�������������
            rigid.position -= transform.right * speed * Time.deltaTime;
            //���巵��ԭ��λ�ã�����������z�ᷴ���ƶ�
            rigid.MovePosition(pos);
        }
    }
}
