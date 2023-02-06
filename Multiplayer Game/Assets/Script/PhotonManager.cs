using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

//MonoBehaviourPunCallbacks
public class PhotonManager : MonoBehaviourPunCallbacks
{
    // Decalaring Variable/Objects
    public TMP_InputField userNameText;
    public TMP_InputField roomNameText;
    public TMP_InputField maxPlayer;

    public GameObject PlayerNamePanel;
    public GameObject LobbyPanel;
    public GameObject RoomCreatePanel;
    public GameObject ConnectingPanel;
    public GameObject RoomListPanel;
    public GameObject roomListPrefab;
    public GameObject roomListParent;

    private Dictionary <string , RoomInfo> roomListData;
    private Dictionary<string, GameObject> roomListGameobject;
    private Dictionary<int, GameObject> playerListGameobject;
    
    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;
    public GameObject playerListItemPrefab;
    public GameObject playerListItemParent;
    public GameObject PlayButton;

    #region UnityMehtods
    // Start is called before the first frame update
    void Start()
    {
        // Initialize Variable/Objects
        ActivateMyPanel(PlayerNamePanel.name);
        roomListData = new Dictionary<string, RoomInfo>();
        roomListGameobject = new Dictionary<string, GameObject>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log("Network state :" + PhotonNetwork.NetworkClientState);
    }

    #endregion

    #region UiMethods
    // OnLogin Button
    public void OnLoginClick()
    {   
        
        // taking input from user
        string name = userNameText.text;
        //Condition
        if (!string.IsNullOrEmpty(name))
        {
            //
            PhotonNetwork.LocalPlayer.NickName = name;
            // Connecting Online
            PhotonNetwork.ConnectUsingSettings();
            // Activate Connecting Panel
            ActivateMyPanel(ConnectingPanel.name);
        }
        else
        {
            Debug.Log("Empty name ");
        }
    }

    // OnClickRoomCreate Button
    public void OnClickRoomCreate()
    {   // taking input from user
        string roomName = roomNameText.text;
         //Condition
        if (string.IsNullOrEmpty(roomName))
        {   // Initializing RoomName
            roomName = roomName + Random.Range(0, 1000);
        }
        RoomOptions roomOptions = new RoomOptions();
        // Initializing MaxPlayers from taking input from user
        roomOptions.MaxPlayers =(byte) int.Parse(maxPlayer.text);
        // CreatingRoom Online
        PhotonNetwork.CreateRoom(roomName,roomOptions);
    }

    // OnClick CancelButton
     public void OnCancelClick()
    {   
        // Activate LobbyPanel
        ActivateMyPanel(LobbyPanel.name);
    }

    // OnClickRoomListButton
    public void RoomListBtnClicked()
    {   
        // if PhotonNetwork is not null
        if (!PhotonNetwork.InLobby)
        {   // JoinLobby
            PhotonNetwork.JoinLobby();
        }
        // Activate RoomListPanel
        ActivateMyPanel(RoomListPanel.name);
    }

    public void BackFromRoomList()
    {
        // if PhotonNetwork is null
        if (PhotonNetwork.InLobby)
        {   
            // LeaveLobby
            PhotonNetwork.LeaveLobby();
        }
        // Activate LobbyPanel
        ActivateMyPanel(LobbyPanel.name);
    }

    public void BackFromPlayerList()
    {   
        // if PhotonNetwork is null
        if (PhotonNetwork.InRoom)
        {
            // LeaveRoom
            PhotonNetwork.LeaveRoom();
        }
        // Activate LobbyPanel
        ActivateMyPanel(LobbyPanel.name);
    }

    #endregion

    #region PHOTON_CALLBACKS

    public override void OnConnected()
    {   
        Debug.Log("Connected to Internet!");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "is connected to photon...");
        // Activate LobbyPanel
        ActivateMyPanel(LobbyPanel.name);

    }

    // OnCreatedRoom
    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + "Is created !");
    }

    // OnJoinedRoom
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "Room Joined");
        // Activate InsideRoomPanel
        ActivateMyPanel(InsideRoomPanel.name);
            // if playerListGameobject is Null
        if (playerListGameobject == null)
        {   // Initializing playerListGameobject
            playerListGameobject = new Dictionary<int, GameObject>();
        }

            // if IsMasterClient
        if (PhotonNetwork.IsMasterClient)
        {   // PlayButton SetActive(true)
            PlayButton.SetActive(true);
        }
        else
        {   
            // PlayButton SetActive(false)
            PlayButton.SetActive(false);
        }

        foreach(Player p in PhotonNetwork.PlayerList)
        {    // Initializing playerListItem with playerListItemPrefab
            GameObject playerListItem = Instantiate(playerListItemPrefab);
            // SetParent playerListItemParent
            playerListItem.transform.SetParent(playerListItemParent.transform);
            playerListItem.transform.localScale = Vector3.one;

            // Initializing playerListItem with playerList name
            playerListItem.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text=p.NickName;

            // condition
            if (p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListItem.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                playerListItem.transform.GetChild(1).gameObject.SetActive(false);
            }

            playerListGameobject.Add(p.ActorNumber, playerListItem.gameObject);

        }

    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {   // Initializing playerListItem with playerListItemPrefab
        GameObject playerListItem = Instantiate(playerListItemPrefab);
        playerListItem.transform.SetParent(playerListItemParent.transform);
        playerListItem.transform.localScale = Vector3.one;

        playerListItem.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = newPlayer.NickName;
        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListItem.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            playerListItem.transform.GetChild(1).gameObject.SetActive(false);
        }

        playerListGameobject.Add(newPlayer.ActorNumber, playerListItem);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListGameobject[otherPlayer.ActorNumber]);
        playerListGameobject.Remove(otherPlayer.ActorNumber);

        if (PhotonNetwork.IsMasterClient)
        {
            PlayButton.SetActive(true);
        }
        else
        {
            PlayButton.SetActive(false);
        }

    }

    public void OnClickPlayButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
        PhotonNetwork.LoadLevel("Game");

        }
    }

    public override void OnLeftRoom()
    {
        ActivateMyPanel(LobbyPanel.name);
        foreach (GameObject obj in playerListGameobject.Values)
        {
            Destroy(obj);
        }

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Clear List
        ClearRoomList();


        foreach (RoomInfo rooms in roomList)
        {
            Debug.Log("Room Name :" + rooms.Name);
            if(!rooms.IsOpen || !rooms.IsVisible || rooms.RemovedFromList)
            {
                if (roomListData.ContainsKey(rooms.Name))
                {
                    roomListData.Remove(rooms.Name);
                }
            }
            else{
                if (roomListData.ContainsKey(rooms.Name))
                {
                    //Update List
                    roomListData[rooms.Name] = rooms;
                }
                else
                {
                    roomListData.Add(rooms.Name, rooms);
                }
            }
           
        }


        // Generate List Item

        foreach(RoomInfo roomItem in roomListData.Values)
        {
            GameObject roomListItemObject = Instantiate(roomListPrefab);
            roomListItemObject.transform.SetParent(roomListParent.transform);
            roomListItemObject.transform.localScale = Vector3.one;
            // room name  player Number  Button room Join 
            Debug.Log("Room Name :;" + roomItem.Name);
            roomListItemObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = roomItem.Name;
            roomListItemObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = roomItem.PlayerCount + "/" + roomItem.MaxPlayers; ;
            roomListItemObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => RoomJoinFromList(roomItem.Name));
            roomListGameobject.Add(roomItem.Name, roomListItemObject);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomList();
        roomListData.Clear();
    }
    #endregion

    #region Public_Mehtods
    public void RoomJoinFromList(string roomName)
    {   
        if (PhotonNetwork.InLobby)
        {   
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    }


    public void ClearRoomList()
    {
        if (roomListGameobject.Count > 0)
        {
            foreach (var v in roomListGameobject.Values)
            {   
                // Destorying gameobject list
                Destroy(v);
            }
            roomListGameobject.Clear();
        }
    }

    // Activating all Panel
    public void ActivateMyPanel(string panelName)
    {
        LobbyPanel.SetActive(panelName.Equals(LobbyPanel.name));
        PlayerNamePanel.SetActive(panelName.Equals(PlayerNamePanel.name));
        RoomCreatePanel.SetActive(panelName.Equals(RoomCreatePanel.name));
        ConnectingPanel.SetActive(panelName.Equals(ConnectingPanel.name));
        RoomListPanel.SetActive(panelName.Equals(RoomListPanel.name)); //InsideRoomPanel
        InsideRoomPanel.SetActive(panelName.Equals(InsideRoomPanel.name));
    }
    #endregion
}
