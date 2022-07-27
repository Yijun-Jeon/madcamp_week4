using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerInfo : MonoBehaviour
{
    public TMP_Text nickName;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        nickName = GetComponent<TMP_Text>();
    }

    public void SetNickName(Player player)
    {
        this.nickName.text = player.NickName;
        this.player = player;
    }

    public void changeCamera()
    {
        Rigidbody2D targetTF = (PhotonNetwork.GetPhotonView((int)player.CustomProperties["PVID"])).GetComponent<Rigidbody2D>();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().target = targetTF;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
