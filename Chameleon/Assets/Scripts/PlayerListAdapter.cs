using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListAdapter : MonoBehaviourPunCallbacks
{

    public GameObject contents;
    public GameObject playerInfo;

    // Start is called before the first frame update
    void Start()
    {
        UpdateItems();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        UpdateItems();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdateItems();
    }

    public void DeleteItems()
    {
        
    }
    public void UpdateItems()
    {

        foreach (Transform child in contents.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject curPlayer = Instantiate<GameObject>(this.playerInfo, contents.transform);
            curPlayer.GetComponent<PlayerInfo>().SetNickName(player);
            if(Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties["start"]))
                curPlayer.GetComponent<PlayerInfo>().SetPower(player);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
     public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        UpdateItems();
    }
}
