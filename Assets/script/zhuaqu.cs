using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class zhuaqu : MonoBehaviour
{
    public GameObject zhuaQu;
    private GameObject beiZhua;
    private GameObject[] wuPing;

    private bool isCatch = false;

    // Start is called before the first frame update
    void Start()
    {
        wuPing = GameObject.FindGameObjectsWithTag("wuping");

    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (zhuaQu.GetComponentInChildren<ArticulationBody>().xDrive.target == -10f)
        //{
        for (int i = 0; i < wuPing.Length; i++)
        {

            if (collision.gameObject == wuPing[i])
            {
                beiZhua = wuPing[i];
                isCatch = true;
                break;
                //  zhuaQu.GetComponent<BoxCollider>().enabled = false;
                //   beiZhua.GetComponent<MeshCollider>().enabled = false;

            }
        }
        //}

    }
    private void OnCollisionExit(Collision collision)
    {
        isCatch = false;
    }



    // Update is called once per frame
    void Update()
    {
        if (isCatch && zhuaQu.transform.GetChild(1).GetComponent <ArticulationBody>().xDrive.target == -10f&&zhuaQu.transform.childCount==2)
        {
            if (!beiZhua.GetComponent<Rigidbody>().isKinematic)
            {
                beiZhua.GetComponent<Rigidbody>().isKinematic = true;
                beiZhua.GetComponent<Rigidbody>().useGravity = false;
                beiZhua.transform.SetParent(zhuaQu.transform, true);
                isCatch = false;
            }         
        }

        if (zhuaQu.transform.GetChild(1).GetComponent<ArticulationBody>().xDrive.target != -10f && beiZhua != null)
        {
            if (beiZhua.GetComponent<Rigidbody>().isKinematic)
            {
                beiZhua.GetComponent<Rigidbody>().useGravity = true;
                beiZhua.GetComponent<Rigidbody>().isKinematic = false;
                beiZhua.transform.SetParent(GameObject.Find("liushuixiangzz").transform, true);
                beiZhua = null;
                isCatch = false;
            }
            
        }
    }
}
