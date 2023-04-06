using OPCClientInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OPCItem : MonoBehaviour
{
    public Text IDTxt;
    public Text NameTxt;
    public Text DescriptionTxt;
    public Text TypeTxt;
    public Text ValueTxt;
    public Text AuthTxt;
    public Button BindBtn;

    internal void Init(OPCNode node)
    {
        if (node== null) return;
        IDTxt.text = node.NodeId?.ToString();//问号代表为空则不执行
        NameTxt.text = node.Name?.ToString();
        ValueTxt.text = node.Value?.ToString();
        TypeTxt.text = node.Type ?. ToString();
        AuthTxt.text = node.Auth?.ToString();
        DescriptionTxt.text = node.Description?.ToString();
     
    }
   
}
