using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    public Button Play;
    public Button Stopbtn;
    public Button Connectbtn;
    public GameObject ConnectWindow;
    public GameObject ManualControl;
    public bool windowact = false;
    
    // Start is called before the first frame update

    void Start()
    {
        AddUIevent();
    }
    private void Update()
    {
        
    }

    private void AddUIevent()
    {
        Play.onClick.AddListener(onPlayClick);
        Stopbtn.onClick.AddListener(onStopbtnClick);
        Connectbtn.onClick.AddListener(onConnectbtnClick);
    }
    private void onPlayClick()
    {
        if (!windowact)
        {
            if (ConnectWindow.transform.position.y > 10f)
            {
                ConnectWindow.transform.position -= transform.up * 10;
            }
            windowact = !windowact;
            ManualControl.SetActive(false);
            GameObject.Find("Main Camera").GetComponent<Camera_move>().enabled = false;
            ConnectWindow.SetActive(true);
        }
    }
    private void onStopbtnClick()
    {
        if (windowact)
        {
            ManualControl.SetActive(true);
            GameObject.Find("Main Camera").GetComponent<Camera_move>().enabled = true;
            windowact = !windowact;
            ConnectWindow.SetActive(false);
        }
    }
    private void onConnectbtnClick()
    {

        if (windowact == false)
        {
            if (ConnectWindow.transform.position.y > 10f)
            {
                ConnectWindow.transform.position -= transform.up * 10;
            }
           
            ConnectWindow.SetActive(!windowact);
            ManualControl.SetActive(false);
            GameObject.Find("Main Camera").GetComponent<Camera_move>().enabled = false;
            windowact = !windowact;
            //GameObject.Find("btnrob1").SetActive(false);
        }
        else
        {

            if (ConnectWindow.transform.position.y > 10f)
            { 
                ConnectWindow.transform.position -= transform.up * 10;
                GameObject.Find("Main Camera").GetComponent<Camera_move>().enabled = false;
            }
            else
            {
                ConnectWindow.transform.position += transform.up * 10;
                GameObject.Find("Main Camera").GetComponent<Camera_move>().enabled = true;
            }
            // GameObject.Find("btnrob1").SetActive(true);
        }
    }
}
