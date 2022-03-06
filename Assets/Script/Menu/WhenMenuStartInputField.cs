using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhenMenuStartInputField : MonoBehaviour
{
    Text Placeholder;
    InputField NickNameText;
    private void Awake()
    {
        Placeholder = gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        NickNameText = GetComponent<InputField>();
    }
    // Start is called before the first frame update
    void Start()
    {
        string NickName = GameObject.Find("NotDestroyObject").GetComponent<NotDestroy>().NickName;
        Debug.Log(NickName);
        if ( NickName != "")
        {
            NickNameText.text = NickName;
            Placeholder.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
