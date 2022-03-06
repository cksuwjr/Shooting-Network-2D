using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Click : MonoBehaviour
{
    public void OnClickStart ()
    {
        GameObject.Find("NotDestroyObject").GetComponent<NotDestroy>().RegisterObject();
        SceneManager.LoadScene("PlayScene");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameObject.Find("NotDestroyObject").GetComponent<NotDestroy>().RegisterObject();
            SceneManager.LoadScene("PlayScene");
        }

    }
}
