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
    public GameObject InGamePanel;
    public GameObject EndPanel;
    public GameObject Black;
    public Camera MainCamera;


    private bool start = false;
    private bool end = false;
    private double startTime;
    private double endTime;

    [SerializeField] private double playTime; //seconds


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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            return;
        }
        if (start)
        {
            double time = PhotonNetwork.Time;
            if (time >= endTime)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Hashtable room_cp = PhotonNetwork.CurrentRoom.CustomProperties;
                    room_cp["end"] = true;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(room_cp);
                }
                InGamePanel.transform.Find("TimerText").GetComponent<TMP_Text>().text = $"게임 종료!";
            }
            else
            {
                double timeLeft = endTime - time;
                string minute_text = ((int)timeLeft / 60 % 60).ToString();
                string second_text = ((int)timeLeft % 60).ToString();
                InGamePanel.transform.Find("TimerText").GetComponent<TMP_Text>().text = $"{minute_text} : {second_text.PadLeft(2, '0')}";
                if (timeLeft < 30f)
                {
                    InGamePanel.transform.Find("TimerText").GetComponent<TMP_Text>().color = Color.red;
                }
            }
        }

    }

    public void Spawn()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        DisconnectPanel.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
        InGamePanel.SetActive(false);
        EndPanel.SetActive(false);
    }

    public void startGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        List<Vector3> SpawnSpaces = new List<Vector3>();
        SpawnSpaces.Add(new Vector3(-27, 6, 0)); SpawnSpaces.Add(new Vector3(0f, 0f, 0)); SpawnSpaces.Add(new Vector3(11f, -4f, 0));
        SpawnSpaces.Add(new Vector3(25, 0, 0)); SpawnSpaces.Add(new Vector3(40f, 5.5f, 0)); SpawnSpaces.Add(new Vector3(52f, 5.19f, 0));
        SpawnSpaces.Add(new Vector3(77.61f, -4f, 0)); SpawnSpaces.Add(new Vector3(105.54f, 16.39f, 0)); SpawnSpaces.Add(new Vector3(36.21f, 20.97f, 0));
        SpawnSpaces.Add(new Vector3(35f, 37.33f, 0)); SpawnSpaces.Add(new Vector3(19.58f, 17f, 0)); SpawnSpaces.Add(new Vector3(5f, 22.05f, 0));
        SpawnSpaces.Add(new Vector3(3.76f, 26.63f, 0)); SpawnSpaces.Add(new Vector3(9.02f, 17.38f, 0)); SpawnSpaces.Add(new Vector3(-9.26f, 18.11f, 0));
        SpawnSpaces.Add(new Vector3(2.07f, 30.76f, 0)); SpawnSpaces.Add(new Vector3(56.3f, 14.8f, 0)); SpawnSpaces.Add(new Vector3(2.89f, 14.21f, 0));
        SpawnSpaces.Add(new Vector3(74.8f, 6f, 0)); SpawnSpaces.Add(new Vector3(70.12f, -4f, 0));

        for (int i = 0; i < SpawnSpaces.Count; i++)
        {
            Vector3 fruitCurrentIndex = SpawnSpaces[i];
            int randomIndex = Random.Range(i, SpawnSpaces.Count);
            SpawnSpaces[i] = SpawnSpaces[randomIndex];
            SpawnSpaces[randomIndex] = fruitCurrentIndex;
        }

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
            Hashtable player_cp = new Hashtable();
            player_cp.Add("isDead", false);
            player_cp.Add("power", intArr[index]);
            player_cp.Add("space", SpawnSpaces[intArr[index]]);
            player.SetCustomProperties(player_cp);
            index++;
        }
        Hashtable room_cp = new Hashtable();
        room_cp.Add("start", true);
        room_cp.Add("startTime", PhotonNetwork.Time);
        room_cp.Add("end", false);
        PhotonNetwork.CurrentRoom.SetCustomProperties(room_cp);
    }


    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        object propsStart;
        if (propertiesThatChanged.TryGetValue("start", out propsStart))
        {
            start = (bool)propsStart;
        }
        object propsStartTime;
        if (propertiesThatChanged.TryGetValue("startTime", out propsStartTime))
        {
            startTime = (double)propsStartTime;
            endTime = startTime + playTime;
            ReadyPanel.SetActive(false);
            InGamePanel.SetActive(true);
        }
        object propsEnd;
        if (propertiesThatChanged.TryGetValue("end", out propsEnd))
        {
            end = (bool)propsEnd;
            if (end == true)
            {
                InGamePanel.SetActive(false);
                EndPanel.SetActive(true);
            }
        }
    }

    [PunRPC]
    void startGameRPC()
    {
        ReadyPanel.SetActive(false);
        // start timer
    }
}
