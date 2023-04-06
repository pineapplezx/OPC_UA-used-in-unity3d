using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PutOut : MonoBehaviour, IPointerClickHandler
{
    public GameObject Gan;
    public Button GanOpen;
    private int num = 0;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Gan.name=="ÍÆ¸Ë") 
        {
            num = (++num) % 2;
            if (num == 1)
            {
                GanOpen.GetComponentInChildren<Text>().color = Color.red;
                var value = Gan.GetComponent<ArticulationBody>().xDrive;
                value.targetVelocity = 2.0f;
                Gan.GetComponent<ArticulationBody>().xDrive = value;
            }
            else
            {
                GanOpen.GetComponentInChildren<Text>().color = Color.green;
                var value = Gan.GetComponent<ArticulationBody>().xDrive;
                value.targetVelocity = 0.0f;
                Gan.GetComponent<ArticulationBody>().xDrive = value;
            }
        }
        else
        {

            num = (++num) % 2;
            if (num == 1)
            {
                GanOpen.GetComponentInChildren<Text>().color = Color.red;
                var value = Gan.GetComponent<ArticulationBody>().xDrive;
                value.target = value.lowerLimit;
                Gan.GetComponent<ArticulationBody>().xDrive = value;
            }
            else
            {
                GanOpen.GetComponentInChildren<Text>().color = Color.green;
                var value = Gan.GetComponent<ArticulationBody>().xDrive;
                value.target = 0.0f;
                Gan.GetComponent<ArticulationBody>().xDrive = value;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
