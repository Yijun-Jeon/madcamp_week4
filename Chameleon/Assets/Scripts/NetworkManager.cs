using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    public TMP_InputField NickNameInput;
    public GameObject DisconnectPanel;
    public GameObject RespawnPanel;
    public GameObject ReadyPanel;
    public GameObject Black;
    public Camera MainCamera;

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnJoinedRoom()
    {
        DisconnectPanel.SetActive(false);
        Black.SetActive(true);
        ReadyPanel.SetActive(true);
        MainCamera.orthographicSize = 6;
        Spawn();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public Dictionary<int, Player> GetPlayerInfo()
    {
        return PhotonNetwork.CurrentRoom.Players;
    }

    void Update() { if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) PhotonNetwork.Disconnect(); }

    public void Spawn()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        DisconnectPanel.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
    }

    public void startGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        // else
        PhotonNetwork.CurrentRoom.IsOpen = false;

        int numOfPlayers = PhotonNetwork.PlayerList.Length;

        int[] intArr = new int[numOfPlayers];

        for (int i = 0; i < numOfPlayers; i++)
            intArr[i] = i + 1;

        int random1, random2;
        int temp;

        for (int i = 0; i < intArr.Length; i++)
        {
            random1 = Random.Range(0, intArr.Length);
            random2 = Random.Range(0, intArr.Length);

            temp = intArr[random1];
            intArr[random1] = intArr[random2];
            intArr[random2] = temp;
        }

        int index = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            print(player.NickName);
            print(intArr[index]);
            player.SetCustomProperties(new Hashtable { { "power", intArr[index++] } });
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "start", true }, { "startTime", PhotonNetwork.Time } });
    }


    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        ReadyPanel.SetActive(false);
    }

    [PunRPC]
    void startGameRPC()
    {
        ReadyPanel.SetActive(false);
        // start timer
    }
}
