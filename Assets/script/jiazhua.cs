using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class jiazhua : MonoBehaviour,  IPointerClickHandler
{
    public ArticulationBody art;
    public int OpenSignal = 1;
    public float MoveSpeed = 1f;
    public Image connection_info_img;
    public void OnPointerClick(PointerEventData eventData)
    {
        OpenSignal++;
        OpenSignal = OpenSignal % 2;
        
    }

   

    // Update is called once per frame
    void Update()
    {
        if (OpenSignal==1)
        {
            var drive = art.xDrive;
            drive.target = drive.lowerLimit;
            //if (drive.target < drive.lowerLimit)
            //    drive.target = drive.lowerLimit;
            art.xDrive = drive;
            connection_info_img.GetComponent<Image>().color = Color.green;
        }
        if (OpenSignal==0)
        {
            var drive = art.xDrive;
            drive.target = drive.upperLimit;
            //if (drive.target < drive.lowerLimit)
            //    drive.target = drive.lowerLimit;
            art.xDrive = drive;
            connection_info_img.GetComponent<Image>().color = Color.red;

        }

    }
}