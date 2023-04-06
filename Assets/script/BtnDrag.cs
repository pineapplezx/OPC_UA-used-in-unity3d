using Opc.Ua;
using Opc.Ua.Client;
using OPCClientInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BtnDrag : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    public Text CloneTxt;
    public GameObject window;
    public Button Selection;

    // public Color NormalColor = new (100f/255,100f/255,100f/255,0f/255);
    //public Color SelectColor = new(150f / 255, 150f / 255, 150f / 255, 50f / 255);
  // private Dictionary<GameObject, GameObject> BindJoint = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        CloneTxt = ((GameObject)Instantiate(Resources.Load("Prefabs/CloneText"))).GetComponent<Text>();
        CloneTxt.transform.SetParent(window.transform, false);
        CloneTxt.alignment = TextAnchor.MiddleCenter;
        CloneTxt.transform.SetAsLastSibling();
        CloneTxt.text = GetComponentInChildren<Text>().text;
        CloneTxt.transform.localScale = Vector3.one;
        CloneTxt.transform.position = eventData.position;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(window.GetComponent<RectTransform>(),
            eventData.position, eventData.pressEventCamera, out Vector3 target))
        { CloneTxt.transform.position = target; }
        List<RaycastResult> raycastResults = new ();
     
        EventSystem.current.RaycastAll(eventData, raycastResults);
        for (int i=0;i<raycastResults.Count;i++)
        {
            if (raycastResults[i].gameObject.name.Equals("Bind",StringComparison.OrdinalIgnoreCase))
            {
                
                var Btn = raycastResults[i].gameObject.GetComponent<Button>();
                if (Selection!=null)
                {
                    if (Selection.gameObject.GetInstanceID()!=Btn.gameObject.GetInstanceID())
                    {
                        //ColorBlock color = Selection.colors;
                        //color.normalColor = NormalColor;
                        //Selection.colors = color;
                        Selection = null;
                    }
                }
                //ColorBlock colors = Btn.colors;
                //colors.normalColor = SelectColor;
                //Btn.colors = colors;
                Selection = Btn;
                return;
            }
        }
        if(Selection!=null)
        {
            //ColorBlock colors = Selection.colors;
            //colors.normalColor = NormalColor;
            //Selection.colors = colors;
            Selection = null;
        }
    }
    //public Dictionary<GameObject, GameObject> GetDic()
    //{
    //    return BindJoint;
    //}
    public virtual void OnEndDrag(PointerEventData eventData)
    {
       List<RaycastResult> raycastResults = new ();
        EventSystem.current.RaycastAll(eventData, raycastResults);
      
        for (int i =0;i<raycastResults.Count;i++)
        {
            if(raycastResults[i].gameObject.name.Equals("Bind", StringComparison.OrdinalIgnoreCase))
            {
                var txt = raycastResults[i].gameObject.GetComponentInChildren<Text>();
                txt.text = CloneTxt.text;
            
               
                //bool changebind = BindJoint.ContainsKey(raycastResults[i].gameObject.transform.parent.gameObject);
                //if (changebind)
                //{
                //    GameObject child = new GameObject();
                //    child = GameObject.Find(txt.text);
                //    BindJoint[raycastResults[i].gameObject.transform.parent.gameObject] = child;
               
                //}
                //else
                //{
                //    GameObject child = new GameObject();
                //    child = GameObject.Find(txt.text);
                //    BindJoint.Add(raycastResults[i].gameObject.transform.parent.gameObject, child);
                //}

            }
        }
        if(Selection!=null)
        {
            //ColorBlock colors = Selection.colors;
            //colors.normalColor = NormalColor;
            //Selection.colors = colors;
            Selection = null;
        }
       Destroy(CloneTxt.gameObject);
    }
 
   //Start is called before the first frame update
    void Start()
    {
        window = GameObject.Find("ConnectWindow");
      //  BindJoint = new();
    }

    // Update is called once per frame
    void Update()
    {
        //if (BindJoint == null) return;
        //foreach (var key in BindJoint.Keys)
        //{
        //    if (key.transform.GetChild(2) != null)
        //    {
        //        var drive = BindJoint[key].GetComponent<ArticulationBody>().xDrive;
        //        float x = Convert.ToSingle(key.transform.GetChild(2).GetComponent<Text>().text);
        //        drive.target = x;
        //        BindJoint[key].GetComponent<ArticulationBody>().xDrive = drive;


        //    }
        //}
    }
}
