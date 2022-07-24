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
    [SerializeField] private float moveSpeed;
    bool isAlive;
    Vector3 curPos;

    void Awake()
    {
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
        if(PV.IsMine)
        {
            Camera.main.GetComponent<CameraController>().target = transform;
        }
    }

    void Update()
    {
        if (PV.IsMine)
        {
            float xAxis = Input.GetAxisRaw("Horizontal");
            float yAxis = Input.GetAxisRaw("Vertical");
            RB.velocity = new Vector2(xAxis, yAxis) * moveSpeed ;

            if (xAxis != 0 || yAxis != 0)
            {
                AN.SetBool("walk", true);
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered, xAxis);
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered, xAxis);
            }
            else AN.SetBool("walk", false);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                AN.SetTrigger("attack");
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                // AN.SetBool("dead", !AN.GetBool("dead"));
                AN.SetBool("dead", true);
            }

        }
    }

    [PunRPC]
    void FlipXRPC(float xAxis)
    {
        if (xAxis == -1) SR.flipX = true;
        else if (xAxis == 1) SR.flipX = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
