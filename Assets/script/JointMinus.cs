using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JointMinus : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ArticulationBody art;
    public bool IsMove = false;
    public float MoveSpeed = 1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsMove = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMove)
        {
            var drive = art.xDrive;
            drive.target -= (MoveSpeed * Time.deltaTime)*0.1f;
            if (drive.target < drive.lowerLimit)
                drive.target = drive.lowerLimit;
            art.xDrive = drive;
        }
    }
}