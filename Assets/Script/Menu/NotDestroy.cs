using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NotDestroy : MonoBehaviour
{
    public string NickName = "";
    public void RegisterObject()
    {
        NickName = GameObject.Find("Canvas").transform.GetChild(3).GetChild(2).GetComponent<Text>().text;
        DontDestroyOnLoad(gameObject);
    }
}
