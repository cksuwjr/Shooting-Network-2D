using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
// IPunObservable 은 네트워크 간 변수 동기화를 위함
public class PlayerControl : MonoBehaviour, IPunObservable
{
    public float moveSpeed = 5f;
    public Rigidbody2D RB;
    public PhotonView PV;
    public SpriteRenderer SR; 
    public Animator AN;
    public Text Nickname;
    public Image Health;

    double ShotCooltime = 0.0f;  // 샷 쿨타임 적용0.5f
    double MumChit = 0.0f; // 멈칫 적용  0.3f
    int finalSee = 0;
    Vector3 curPos;

    void Awake()
    {
        Nickname.text = "Player";
        Health.color = PV.IsMine ? Color.green : Color.red;

        if (PV.IsMine)
        {
            // 2D 카메라
            var CM = GameObject.Find("CMcamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
        }
    }
    void Update()
    {
        if (ShotCooltime >= 0)
        {
            ShotCooltime -= Time.deltaTime * 1.0f;

            Debug.Log("쿨타임: " + ShotCooltime);
        }
        if (MumChit >= 0)
        {
            MumChit -= Time.deltaTime * 1.0f;
        }
        if (PV.IsMine) // 만약  PV가 is mine이라면, controll localy가 true라면 (키보드입력이 내 화면 내꺼라면)
        {
            float haxis = Input.GetAxisRaw("Horizontal");    //   ← : -1    → : +1 
            float vaxis = Input.GetAxisRaw("Vertical");      //   ↑ : +1    ↓ : -1
                //transform.Translate(new Vector3(haxis * Time.deltaTime * moveSpeed, vaxis * Time.deltaTime * moveSpeed, 0));
                // 플레이어 이동 코드이나 벽에 닿으면 벽을 뜷으려고 하거나 떨림. 그렇기에 이동은 Rigidbody로 표현하겠음
                RB.velocity = new Vector3(haxis * moveSpeed, vaxis * moveSpeed, 0);
            if (MumChit < 0)  // 멈칫 상태가 아니라면
            {
                moveSpeed = 6f; 

                if (haxis != 0)  // 수평 방향키 키보드 입력 값이 있다면
                {
                    PV.RPC("FlipXRPC", RpcTarget.AllBuffered, haxis);    // PV.RPC("적용함수명", 적용타겟, 적용되는 함수에 넘길 인자);
                    AN.SetFloat("Horizontal", haxis);                    // PV의 RPC함수는 그 방에 해당하는 모든 Player에게 모두 이 함수를 실행시키는 함수
                }
                if (vaxis != 0)  // 수직 방향키 키보드 입력 값이 있다면
                {
                    AN.SetFloat("Vertical", vaxis);
                    if (vaxis < 0 && haxis == 0)
                        ChangeDirection(1);
                    else if (vaxis > 0 && haxis == 0)
                        ChangeDirection(3);
                }

                if (haxis != 0 || vaxis != 0) // 만약 키보드의 어떤 입력이라도 있다면 
                {
                    AN.SetBool("walk", true);
                    AN.SetBool("stand", false);//
                }
                else if (haxis == 0 && vaxis == 0) // 만약 키보드의 입력이 없다면
                {
                    ChangeDirection(0);
                    AN.SetBool("walk", false);//
                    AN.SetBool("stand", true);//
                }

            }
            else
            {
                moveSpeed = 0.0f;
            }
            // 스페이스 총알 발사
            if (Input.GetKeyDown(KeyCode.E) && ShotCooltime < 0)
            {// PhotonNetwork.Instantiate("스프라이트", 위치      flipX가 true면 Player위치에서 x기준 -0.4,false면 0.4 , y기준 -0.11, z: 0, 회전 값:그대로); 
             // 총알 소환
             //ShotCooltime = 0.375f;
             //MumChit = 0.225f;
                ShotCooltime = 0.0f;
                MumChit = 0.0f;

                if (finalSee == 2)
                {
                    PhotonNetwork.Instantiate("Bullet", transform.position + new Vector3(SR.flipX ? -0.4f : 0.4f, -0.11f, 0), Quaternion.identity)
                        .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, SR.flipX ? -1 : 1);
                    AN.SetTrigger("shot");
                }
                else if (finalSee == 1 || finalSee == 3)
                {
                    PhotonNetwork.Instantiate("Bullet", transform.position + new Vector3(0, (finalSee == 1) ? -0.5f : 0.5f, 0), Quaternion.identity)
                        .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, (finalSee == 1) ? -2 : 2);
                    AN.SetTrigger("shot");
                }

            }
        }
        // isMine 아닌 것들 동기화
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);

        AN.SetFloat("Speed", moveSpeed);
    }

    [PunRPC]
    void FlipXRPC(float haxis)
    {
        SR.flipX = haxis == -1;   // 키보드입력값의 가로   ← → 의 입력은 axis, 
        ChangeDirection(2);
                                  // axis 가 -1이면 flipX(플레이어의 방향)은 true  (왼쪽)
    }                             //                                  아니면 false (오른쪽)
    public void ChangeDirection(int n)
    {
        if (n != 0)
        {
            finalSee = n;
            AN.SetInteger("finalsee", n);
        }
        AN.SetInteger("direction", n);
    }
    public void Hit()
    {
        Health.fillAmount -= 0.1f;
        if (Health.fillAmount <= 0)
        {
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered); // AllBuffered로 해야 버그가 안 생긴다.
        }
    }
    [PunRPC]
    void DestroyRPC()
    {
        Destroy(gameObject);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)   // IPunObservable 상속받아 재정의
    {
        if (stream.IsWriting)      // if(IsWriting): 보낼때
        {
            stream.SendNext(transform.position);
            stream.SendNext(Health.fillAmount);
            stream.SendNext(Nickname.text);                  
        }
        else                       // else      : 받을때
        {
            curPos = (Vector3)stream.ReceiveNext();
            Health.fillAmount = (float)stream.ReceiveNext();
            Nickname.text = (string)stream.ReceiveNext();      
        }
        // 나의 A에서 전달하면 상대는 B로 받음. 상대의 A에서 전달하면 나의 B로 받음
        // 즉, A와 B는 수가 같아야한다. 받는 순서도 같아야한다. 1코드 -> 1코드    2코드 -> 2코드 (코드 == 번째 줄 코드)
    }

}
