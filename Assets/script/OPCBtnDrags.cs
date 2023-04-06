using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OPCBtnDrags : BtnDrag
{
    //private Dictionary<string, GameObject> dicOpcItem = null;
    //private float value;
    //private List <bool> isChange=null;
    /// <summary>
    /// ��קʱ�����ťλ��λ��IO��ť��
    /// ��Ҫ�ж϶�ӦIO���Ƿ��������
    /// �������������������ݽ���
    /// </summary>
    /// <param name="eventData"></param>
    //private void SubCallBack(string arg1,MonitoredItem arg2, MonitoredItemNotificationEventArgs arg3)
    //{
    //    foreach (KeyValuePair<string, GameObject> a in dicOpcItem)
    //    { // if (!arg2.DisplayName.Contains("ns=3;i=1001")) return;

    //        MonitoredItemNotification notification = arg3.NotificationValue as MonitoredItemNotification;
    //        if (notification != null)
    //        {

    //            var bValue = (float)notification.Value.WrappedValue.Value;

    //            // if (go == null) return;
    //            value = bValue;
                
    //        }
    //    }
    //}
    //private void Start()
    //{
    //    dicOpcItem = new Dictionary<string, GameObject>();
    //}
    //void Update()
    //{
    //    if (dicOpcItem == null)
    //        return;
    //    foreach (KeyValuePair<string, GameObject> a in dicOpcItem)
    //    {
    //        // var opcItem = dicOpcItem["ns=3;i=1001"];
    //        var opcItem = dicOpcItem[a.Key];
    //        var child = opcItem.transform.GetChild(2);
    //        child.GetComponent<Text>().text = value.ToString();
    //    }
          
    //}
    public override void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject.name.Equals("Bind", StringComparison.OrdinalIgnoreCase))
            {
               
                var txt =   raycastResults[i].gameObject.GetComponentInChildren<Text>();

                if (string.IsNullOrEmpty(txt.text))
                {
                    txt.text = CloneTxt.text;
                  // var KEY = txt.transform.parent.Find("ID").GetComponent<Text>().text;
                   // dicOpcItem.Add(KEY, GameObject.Find(txt.text));//�ֵ��ID�͹ؽ�����
                   
                    GetComponentInChildren<Text>().text = String.Empty;
                }
                else
                {
                    //���ݲ�Ϊ�գ����ཻ��ֵ
                    var NameTxt = txt.text;
                    txt.text = CloneTxt.text;
                
                  //  var KEY = txt.transform.parent.Find("ID").GetComponent<Text>().text;
                   // dicOpcItem[KEY] = GameObject.Find(txt.text);
                    GetComponentInChildren<Text>().text = NameTxt;
                }
                Destroy(CloneTxt.gameObject);
                return;
            }
        }
        GetComponentInChildren<Text>().text = String.Empty;
        Destroy(CloneTxt.gameObject);
    }
}
