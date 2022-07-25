using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D RB;
    public Animator AN;
    public SpriteRenderer SR;
    public PhotonView PV;
    public TMP_Text NickNameText;
    [SerializeField] private AttackRange attackRange;
    public TMP_Text PowerText;
    [SerializeField] private float moveSpeed;
    bool isAlive;
    Vector3 curPos;
    
    private float curTime;
    public float coolTime = 1.7f;
    private float curTime_fake;
    public float coolTime_fake = 6f;
    public Transform pos;
    public int power;
    [SerializeField] public TMP_Text AlarmText;
    

    void Awake()
    {
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
        attackRange = transform.Find("AttackRange").gameObject.GetComponent<AttackRange>();

        System.Random rand = new System.Random();
        
        if (PV.IsMine)
        {
            // 공격력 일단 대충 처리
            power = rand.Next(1,7);
            PowerText.text = power.ToString();
            if(power < 3)
            {
                GameObject.Find("CameraCanvas").transform.Find("AlramText").gameObject.SetActive(true);
            }
            // 본인 공격력 안보이게 처리
            // transform.Find("PowerText").gameObject.SetActive(false); 
            Camera.main.GetComponent<CameraController>().target = transform;
        }
        else
        {
            transform.Find("AttackRange").gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (PV.IsMine)
        {
            attackRange.SetOrigin(transform.position);

            // 꼴등 이속 버프
            if(PowerText.text.Equals("1"))
            {
                moveSpeed = 8;
            }
             // 샤킹중이면 아무것도 못함
            if(curTime_fake > 0)
            {
                curTime_fake -= Time.deltaTime;
            }
            else
            {
                if (!AN.GetBool("dead"))
                {
                    float xAxis = Input.GetAxisRaw("Horizontal");
                    float yAxis = Input.GetAxisRaw("Vertical");
                    RB.velocity = new Vector2(xAxis, yAxis).normalized * moveSpeed;

                    if (xAxis != 0 || yAxis != 0)
                    {
                        AN.SetBool("walk", true);
                        PV.RPC("FlipXRPC", RpcTarget.AllBuffered, xAxis);
                    }
                    else AN.SetBool("walk", false);

                    if(curTime <= 0)
                    {   //공격
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(pos.position,2f);
                            

                            foreach(Collider2D collider in collider2Ds)
                            {
                                if(collider.tag == "Player" && !collider.GetComponent<PlayerScript>().PV.IsMine)
                                {
                                    if(Convert.ToInt32(collider.GetComponent<PlayerScript>().PowerText.text) < power)
                                        collider.GetComponent<PlayerScript>().MakeDead();
                                    else if(Convert.ToInt32(collider.GetComponent<PlayerScript>().PowerText.text) > power)
                                        MakeDead();
                                }
                            }
                            PV.RPC("AttackRPC", RpcTarget.All);
                            curTime = coolTime;
                        }
                    }
                    else
                    {
                        curTime -= Time.deltaTime;
                    }
                    
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        PV.RPC("FakeRPC", RpcTarget.All);
                        curTime_fake = coolTime_fake;
                    }
                }   
            }
        }// !PV.IsMine
        else if ((transform.position - curPos).sqrMagnitude >= 100)
            transform.position = curPos;
        else
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }


    public void MakeDead()
    {
        PV.RPC("DeadRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void FlipXRPC(float xAxis)
    {
        if (xAxis == -1) SR.flipX = true;
        else if (xAxis == 1) SR.flipX = false;
    }

    [PunRPC]
    void AttackRPC()
    {
        AN.SetTrigger("attack");
    }

    [PunRPC]
    void DeadRPC()
    {
        RB.velocity = Vector2.zero;
        AN.SetBool("walk", false);
        AN.SetBool("dead", true);
    }

    [PunRPC]
    void FakeRPC()
    {
        AN.SetTrigger("fake");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(PowerText.text);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            PowerText.text = (String)stream.ReceiveNext();
        }

    }
}