using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class BulletScript : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    int dir;  // 총알 방향
    void Start() => Destroy(gameObject, 3.5f);

    void Update()
    {
        
        if (dir == 1 || dir == -1)       // 1 또는 -1 인 경우 수평 발사
            transform.Translate(Vector3.right * 14 * Time.deltaTime * dir);
        else if (dir == 2 || dir == -2)  // 2 또는 -2 인 경우 수직 발사
            transform.Translate(Vector3.down * -14 * Time.deltaTime * (dir / 2));

    }

    void OnTriggerEnter2D(Collider2D col)  
    {
        if (col.tag == "Wall") PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        if(!PV.IsMine && col.tag == "Player" && col.GetComponent<PhotonView>().IsMine) // 느린쪽에 맞춰서 Hit판정
        {
            col.GetComponent<PlayerControl>().Hit();
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DirRPC(int dir)
    {
        this.dir = dir;
    }
    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
}
