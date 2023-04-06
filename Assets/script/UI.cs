using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UI : MonoBehaviour
{
    public Button Button1;
 
    private bool show=false;
  
    public new Transform transform;
    


    // Start is called before the first frame update
    void Start()
    {
        transform.gameObject.SetActive(false);
        Button1.onClick.AddListener(OnBtn1Click);
    }


    private void OnBtn1Click()
    {
        show= !show;
        transform.gameObject.SetActive(show);
    }

    

    // Update is called once per frame
    void Update()
    {

    }
}