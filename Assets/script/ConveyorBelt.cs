using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConveyorBelt : MonoBehaviour
{
    public Toggle toggle;
    /// <summary>
    /// 刚体组件
    /// </summary>
    private Rigidbody rigid;
    /// <summary>
    /// 速度
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
            //记录刚体初始位置刚体
            var pos = rigid.position;
            //刚体向z轴坐标方向移动，不带动物体
            rigid.position -= transform.right * speed * Time.deltaTime;
            //刚体返回原来位置，带动物体向z轴反向移动
            rigid.MovePosition(pos);
        }
    }
}
