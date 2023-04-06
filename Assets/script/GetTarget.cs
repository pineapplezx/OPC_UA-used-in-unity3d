using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetTarget : MonoBehaviour
{
    public GameObject Joint;
    public GameObject Show;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var value = Joint.GetComponent<ArticulationBody>().xDrive.target;

        Show.GetComponent<Text>().text = value.ToString();
    }
}
