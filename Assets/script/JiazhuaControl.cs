using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiazhuaControl : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject zhua1;
    public GameObject zhua2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var drive = zhua1.GetComponent<ArticulationBody>().xDrive;
        drive.target = zhua2.GetComponent<ArticulationBody>().xDrive.target;

        zhua1.GetComponent<ArticulationBody>().xDrive = drive;
    }
}
