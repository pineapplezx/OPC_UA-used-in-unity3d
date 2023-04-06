using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OPCClientInterface;
using Battlehub.UIControls;
using System;
using System.Linq;
using Opc.Ua.Client;
using Opc.Ua;

public class connect_plc : MonoBehaviour
{
    public Button ConnectBtn;
    public Button BackBtn;
    public Transform Goview;
    public Transform OPCView;
    //  public Text errorText;
    public InputField URLInput;
    private bool isConnect = false;
    private ClientLogIn clientLogIn;
    private OPCUAClient opcuaClient;
    private OPCUAClientEX opcuaClientEX;


    private bool isChange;

    //private Dictionary<object, Dictionary<string,GameObject>> dicOpcItem = null;

    private Dictionary<string, GameObject> dicOpcItem = null;
    //private Dictionary<string, (string, object)> dicOPCData = null;
    private Dictionary<string, (string, object)> dicOPCData = null;
    private Dictionary<string, GameObject> dicGo = null;
    private void SubCallBack(string arg1, MonitoredItem arg2, MonitoredItemNotificationEventArgs arg3)
    {
        #region 字典方法
        MonitoredItemNotification notification = arg3.NotificationValue as MonitoredItemNotification;
        if (notification != null)
        {
            var bValue = notification.Value.WrappedValue.Value;
            if (dicOPCData.ContainsKey(arg2.DisplayName))
            {
                dicOPCData[arg2.DisplayName] = (notification.Value.WrappedValue.TypeInfo.ToString(), bValue);
            }
            else
            {
                dicOPCData.Add(arg2.DisplayName, (notification.Value.WrappedValue.TypeInfo.ToString(), bValue));
            }
        }
        isChange = true;
        #endregion


    }

    // Start is called before the first frame update
    void Start()
    {
        // errorText.gameObject.SetActive(false);

        UIEventBind();
        TreeEventBind();
        LoadGoEvent();
        //dicOpcItem =new Dictionary<object, Dictionary<string, GameObject>>();
        dicOpcItem = new Dictionary<string, GameObject>();
        dicOPCData = new Dictionary<string, (string, object)>();
        dicGo = new Dictionary<string, GameObject>();
    }
    // Update is called once per frame
    void Update()
    {
        #region 字典回调方法
        if (isChange)
        {
            foreach (var key in dicOpcItem.Keys)
            {
                if (!dicOPCData.ContainsKey(key)) continue;

                var type = dicOPCData[key].Item1;
                var value = dicOPCData[key].Item2;

                var opcItem = dicOpcItem[key];
                if (value != null)
                    opcItem.transform.GetChild(2).GetComponent<Text>().text = value.ToString();
            }
            #endregion
            //遍历字典方法
            //foreach (var key in dicOpcItem.Keys)
            //{
            //    var opcList=opcuaClientEX.AfterNodeSelect(key);
            //    var childItem = dicOpcItem[key];
            //    if (childItem == null|| opcList==null) continue;

            //    foreach (var opc in opcList)
            //    {
            //        if (!childItem.ContainsKey(opc.NodeId.ToString())) continue;

            //        var opcItem = childItem[opc.NodeId.ToString()];
            //        if(opc.Value!=null)
            //        opcItem.transform.GetChild(2).GetComponent<Text>().text = opc.Value.ToString();
            //    }
            //}
        }
        foreach (var key in dicGo.Keys)
        {
            var jointeach = opcuaClientEX.GetData(key);

            var drive = dicGo[key].GetComponent<ArticulationBody>().xDrive;
        
               if( jointeach.Value.ToString()=="False")
            {
                
                drive.target = drive.lowerLimit;
                dicGo[key].GetComponent<ArticulationBody>().xDrive = drive;
                continue;
            }

            if (jointeach.Value.ToString() == "True")
            {

                drive.target = drive.upperLimit;
                dicGo[key].GetComponent<ArticulationBody>().xDrive = drive;
                continue;
            }
            float x = Convert.ToSingle(jointeach.Value);
            drive.target = x;
            dicGo[key].GetComponent<ArticulationBody>().xDrive = drive;

        }
    }


  
    private void LoadGoEvent()
    {
        var models = GameObject.FindGameObjectsWithTag("Model");

        for (int i = 0; i < models.Length; i++)
        {
            var go = models[i];
            Transform[] father = go.GetComponentsInChildren<Transform>();
            for (int j = 1; j < father.Length; j++)
            {
                var goChild = father[j];
                var goBtn = (GameObject)Instantiate(Resources.Load("Prefabs/GoBtnItem"));
                goBtn.transform.SetParent(Goview.transform, false);
                goBtn.GetComponentInChildren<Text>().text = goChild.name;
            }

        }
    }
    private void OPCUAClient_ConnectComplete(object sender, string[] e)
    {
        if (opcuaClient.Connected == false) return;
        opcuaClientEX = new OPCUAClientEX(opcuaClient);
        var opcList = opcuaClientEX.GetBranch(SourceID.ObjectsFolder, true);
       
        TreeView.Items = opcList;
    }
    private void OPCUAClient_OpcStatusChange(object sender, OpcUaStatusEventArgs e)
    {
        if (e.Text.Contains("Disconnected"))
        {
            isConnect = false;
            opcuaClientEX = null;
        }
        else
        {
            isConnect = true;
        }
    }
    private void OnDestroy()
    {
        TreeEventUnBind();
    }

    private async void OnConnectBtnClick()
    {
        var connectText = ConnectBtn.GetComponentInChildren<Text>();
        if (!isConnect)
        {
            var url = URLInput.text;
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            opcuaClient = new OPCUAClient();
            opcuaClient.ConnectComplete += OPCUAClient_ConnectComplete;
            opcuaClient.OpcStatusChange += OPCUAClient_OpcStatusChange;
            clientLogIn = new ClientLogIn(opcuaClient);
            bool result = clientLogIn.GuestLogin();
            bool isConnected = await opcuaClient.ConnectServer(url);
            if (!result)
            {
                return;
            }
            if (!opcuaClient.Connected || !isConnected)
            {
                return;
            }
            isConnect = true;
            connectText.text = "断开";
        }
        else
        {
            isConnect = false;
            connectText.text = "连接";
        }
    }
    private void OnBackBtnClick()
    {
        GameObject.Find("ConnectWindow").transform.position += transform.up * 10;
        GameObject.Find("Main Camera").GetComponent<Camera_move>().enabled = true;
        BindOPC();

    }

    private void BindOPC()
    {
        var items = OPCView.GetComponentsInChildren<OPCItem>();
        foreach (var item in items)
        {
            var bindName = item.BindBtn.GetComponentInChildren<Text>().text;
            if (string.IsNullOrEmpty(bindName))
            {
                if (dicGo.ContainsKey(item.IDTxt.text))
                    dicGo.Remove(item.IDTxt.text);
                continue;
            }

            var go = GameObject.Find(bindName);
            dicGo[item.IDTxt.text] = go;
        }
    }

    private void UIEventBind()
    {
        ConnectBtn.onClick.AddListener(OnConnectBtnClick);
        BackBtn.onClick.AddListener(OnBackBtnClick);
    }

    #region Tree
    public TreeView TreeView;
    public static bool IsPrefab(Transform This)
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            throw new InvalidOperationException("can not work in edit mode");
        }
        return This.gameObject.scene.buildIndex < 0;
    }
    private void TreeEventBind()
    {
        if (!TreeView)
        {
            return;
        }
        TreeView.ItemDataBinding += OnItemDataBinding;
        TreeView.SelectionChanged += OnselectionChanged;
        TreeView.ItemsRemoved += OnItemsRemoved;
        TreeView.ItemExpanding += OnItemExpanding;

    }
    private void TreeEventUnBind()
    {
        if (!TreeView)
        {
            return;
        }
        TreeView.ItemDataBinding -= OnItemDataBinding;
        TreeView.SelectionChanged -= OnselectionChanged;
        TreeView.ItemsRemoved -= OnItemsRemoved;
        TreeView.ItemExpanding -= OnItemExpanding;

    }
    private void OnItemExpanding(object sender, ItemExpandingArgs e)
    {
        OPCNode dataItem = (OPCNode)e.Item;
        var treeNodes = opcuaClientEX.GetNodeChild(dataItem.Tag);
        Debug.Log("获取OPC数据");
        if (treeNodes.Count > 0)
        {
            e.Children = treeNodes;
        }
    }
    private void OnselectionChanged(object sender, SelectionChangedArgs e)
    {
#if UNITY_EDITOR
        //do something on selection changed (just syncronized with editor's hierarchy for demo purposes)
        UnityEditor.Selection.objects = e.NewItems.OfType<GameObject>().ToArray();
#endif
        if (e.NewItems.Length == 1)
        {
            BindOPC();
            ClearOPCItem();
            var opcNode = e.NewItems[0] as OPCNode;
            var opcData = opcuaClientEX.GetData(opcNode.NodeId.ToString());
            var data = opcuaClientEX.AfterNodeSelect(opcNode.Tag);
      
            if (data == null || data.Count == 0) return;
            opcuaClientEX.UpdateInTime(opcNode.NodeId.ToString(), opcNode.Tag, SubCallBack);
            dicOpcItem.Clear();

            //for (int i = 0; i < data.Count; i++)
            //{
            //    OPCNode node = data[i];
            //    var opcItem = (GameObject)Instantiate(Resources.Load("Prefabs/OPCItem"));
            //    //   var a = opcItem.transform.GetChild(6).GetComponentInChildren<Text>().text;
            //    //   if (a!="")
            //    //opcItem.transform.GetChild(6).GetComponentInChildren<Text>().text = btnbind.GetDic()[opcItem].name;
            //    opcItem.transform.SetParent(OPCView, false);
            //    var opcMono = opcItem.GetComponent<OPCItem>();
            //    opcMono.Init(node);
            //    if (dicOpcItem.ContainsKey(opcNode.Tag))
            //    {
            //        var childItem = dicOpcItem[opcNode.Tag];
            //        childItem[node.NodeId.ToString()] = opcItem;
            //    }
            //    else
            //    {
            //        Dictionary<string, GameObject> childItem = new Dictionary<string, GameObject>();
            //        childItem[node.NodeId.ToString()] = opcItem;
            //        dicOpcItem.Add(opcNode.Tag, childItem);
            //    }
            //}
            for (int i = 0; i < data.Count; i++)
            {
                OPCNode node = data[i];
                var opcItem = (GameObject)Instantiate(Resources.Load("Prefabs/OPCItem"));


                opcItem.transform.SetParent(OPCView, false);
                var opcMono = opcItem.GetComponent<OPCItem>();

                opcMono.Init(node);
                foreach (var key in dicGo.Keys)
                {
                    if (opcMono.IDTxt.text == key)
                    {
                        opcMono.BindBtn.GetComponentInChildren<Text>().text = dicGo[key].name;
                    }

                }
                if (dicOpcItem.ContainsKey(node.NodeId.ToString()))
                {
                    dicOpcItem[node.NodeId.ToString()] = opcItem;
                }
                else
                {

                    dicOpcItem.Add(node.NodeId.ToString(), opcItem);
                }
            }
        }

    }

    private void ClearOPCItem()
    {
        for (int i = OPCView.childCount - 1; i >= 0; i--)
        {
            if (i == 0) break;
            Destroy(OPCView.GetChild(i).gameObject);
        }
    }
    private void OnItemsRemoved(object sender, ItemsRemovedArgs e)

    {
        for (int i = 0; i < e.Items.Length; ++i)
        {
            GameObject go = (GameObject)e.Items[i];
            if (go != null)
            {
                Destroy(go);
            }
        }

    }
    private void OnItemDataBinding(object sender, TreeViewItemDataBindingArgs e)
    {
        OPCNode dataItem = e.Item as OPCNode;
        if (dataItem != null)
        {
            Text text = e.ItemPresenter.GetComponentInChildren<Text>(true);
            text.text = dataItem.Name;

            //specify whether data item has children(to display expander arrow if needed)
            if (dataItem.Name != "TreeView")
            {
                e.HasChildren = true;//不再频繁查找子节点
                //var treeNodes = opcuaClientEX.GetNodeChild(dataItem.Tag);
                //if (treeNodes != null)
                //{
                //    e.HasChildren = true; //treeNodes?.Count > 0;
                //}
            }
        }
    }
    #endregion
}
