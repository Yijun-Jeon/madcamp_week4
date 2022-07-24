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

    bool isAlive;
    Vector3 curPos;

    void Awake()
    {
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
        if (PV.IsMine)
        {
            Camera.main.GetComponent<CameraController>().target = transform;
        }
    }

    void Update()
    {
        if (PV.IsMine)
        {
            if (!AN.GetBool("dead"))
            {
                float xAxis = Input.GetAxisRaw("Horizontal");
                float yAxis = Input.GetAxisRaw("Vertical");
                RB.velocity = new Vector2(4 * xAxis, 4 * yAxis);

                if (xAxis != 0 || yAxis != 0)
                {
                    AN.SetBool("walk", true);
                    PV.RPC("FlipXRPC", RpcTarget.AllBuffered, xAxis);
                }
                else AN.SetBool("walk", false);

                if (Input.GetKeyDown(KeyCode.Space))
                    PV.RPC("AttackRPC", RpcTarget.All);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                // PV.RPC("DeadRPC", RpcTarget.AllBuffered);

                RB.velocity = Vector2.zero;
                AN.SetBool("walk", false);
                AN.SetBool("dead", !AN.GetBool("dead"));
            }
        }

        // !PV.IsMine
        else if ((transform.position - curPos).sqrMagnitude >= 100)
            transform.position = curPos;
        else
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
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
        AN.SetBool("dead", !AN.GetBool("dead"));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
        }

    }
}
