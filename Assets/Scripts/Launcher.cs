using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    string gameVersion = "1";

    public GameObject RoomListEntryPrefab;
    public GameObject RoomListContent;
    public GameObject PlayerListEntryPrefab;

    private string _roomName;
    private string _maxPlayers;
    private string _nickName;
    private string _masterID;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    [SerializeField] private InputField _randomName;
    [SerializeField] private GameObject _roomsListPanel;
    [SerializeField] private GameObject _createRoomPanel;
    [SerializeField] private GameObject _inRoomPanel;
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _playFabPanel;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        _randomName.text = "Player " + Random.Range(1000, 10000);
    }

    void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
    }


    #region Photon

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log($"Create room failed: {message}");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        cachedRoomList.Clear();

        _inRoomPanel.SetActive(true);

        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(_inRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

            playerListEntries.Add(p.ActorNumber, entry);
        }

        _masterID = PhotonNetwork.MasterClient.UserId;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(PlayerListEntryPrefab);
        entry.transform.SetParent(_inRoomPanel.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListEntries.Add(newPlayer.ActorNumber, entry);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        ActivePanel(_mainPanel.name);

        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    #endregion


    #region UI

    public void OnLoginButtonClicked()
    {
        if (!_nickName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = _nickName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        _roomName = (_roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : _roomName;

        byte maxPlayers;
        byte.TryParse(_maxPlayers, out maxPlayers);
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };

        PhotonNetwork.CreateRoom(_roomName, options, null);
    }

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        ActivePanel(_mainPanel.name);
    }

    public void OnOpenRoomsListPanelButtonClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        _roomsListPanel.SetActive(true);
    }

    public void OnOpenCreateRoomButtonClick()
    {
        _createRoomPanel.SetActive(true);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void SetRoomName(string roomName)
    {
        _roomName = roomName;
    }

    public void Active()
    {
        gameObject.SetActive(true);
    }

    public void Deactive()
    {
        _playFabPanel.SetActive(false);
    }

    public void SetName(string name)
    {
        _nickName = name;
    }

    public void SetCountOfPlayers(string countOfPlayers)
    {
        _maxPlayers = countOfPlayers;
    }

    #endregion

    public void SetAccess(bool state)
    {
        if (PhotonNetwork.MasterClient.IsMasterClient)
        {
            if (!PhotonNetwork.CurrentRoom.IsOpen)
            {
                PhotonNetwork.CurrentRoom.IsOpen = state;
            }
            else
            {
                PhotonNetwork.CurrentRoom.IsOpen = state;
            }
        }

        Debug.Log(PhotonNetwork.CurrentRoom.IsOpen);
    }

    public void CreateForFriends()
    {
        _roomName = (_roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : _roomName;

        byte maxPlayers;
        byte.TryParse(_maxPlayers, out maxPlayers);
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };
        options.IsVisible = false;

        PhotonNetwork.CreateRoom(_roomName, options);
        GUIUtility.systemCopyBuffer = _roomName;
    }

    #region UNITY

    public void ActivePanel(string namePanel)
    {
        _roomsListPanel.SetActive(Equals(namePanel, _roomsListPanel.name));
        _inRoomPanel.SetActive(Equals(namePanel, _inRoomPanel.name));
        _mainPanel.SetActive(Equals(namePanel, _mainPanel.name));
        _createRoomPanel.SetActive(Equals(namePanel, _createRoomPanel.name));
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            string roomState;

            if (info.IsOpen)
            {
                roomState = "Opened Room";
            }
            else
            {
                roomState = "Closed Room";
            }

            GameObject entry = Instantiate(RoomListEntryPrefab);
            entry.transform.SetParent(RoomListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers, roomState);

            roomListEntries.Add(info.Name, entry);
        }
    }

    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }

    #endregion
}
