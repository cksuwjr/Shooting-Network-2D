using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat.UtilityScripts;
using UnityEngine.SceneManagement;

public class Networkmanager : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        Screen.SetResolution(1920, 1080, false);

        PhotonNetwork.ConnectUsingSettings();
        // 서버 연결
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        // 동기화 빠르게?
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
        // 서버가 연결되면 콜백으로 바로 호출                       바로 방을 만들고 입장     
    }
    public override void OnJoinedRoom() 
    {
        Spawn();
        StartCoroutine("DestroyBullet");
    } 
    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (GameObject GO in GameObject.FindGameObjectsWithTag("Bullet")) GO.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.All);
    }
    public void Spawn()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        // 플레이어 복사
        // 복사는 반드시 PhotonNetwork.Instantiate("Resaurces안의 복사할 Prefab 이름", 벡터값, 회전값); 
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            LoadingSceneManager.LoadScene("Menu");
        }
        if (Input.GetKeyDown(KeyCode.Space) && PhotonNetwork.IsConnected) 
        {
            Spawn();
            StartCoroutine("DestroyBullet");
            
        }
    }

}
