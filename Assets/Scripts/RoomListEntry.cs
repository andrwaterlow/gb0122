using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


    public class RoomListEntry : MonoBehaviour
    {
        public Text RoomNameText;
        public Text RoomPlayersText;
        public Button JoinRoomButton;
        public Text StateRoom;

        private string roomName;

        public void Start()
        {
            JoinRoomButton.onClick.AddListener(() =>
            {
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.LeaveLobby();
                }

                PhotonNetwork.JoinRoom(roomName);
            });
        }

        public void Initialize(string name, byte currentPlayers, byte maxPlayers, string state)
        {
            roomName = name;

            RoomNameText.text = name;
            StateRoom.text = state;
            RoomPlayersText.text = currentPlayers + " / " + maxPlayers;
        }
    }
