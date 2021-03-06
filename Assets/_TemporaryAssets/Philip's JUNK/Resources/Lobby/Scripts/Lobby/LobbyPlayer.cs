using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.NetworkLobby
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        static Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };
        //used on server to avoid assigning the same color to two player
        static List<int> _colorInUse = new List<int>();

        public Button colorButton;
        public InputField nameInput;
        public Button readyButton;
        public Button waitingPlayerButton;
        public Button removePlayerButton;
        public Dropdown spell1DropDown;
        public Dropdown spell2DropDown;

        public InputField seedField;
        public Dropdown mapSizeDropDown;
        public InputField waterSizeField;
        public InputField townSpreadField;
        public InputField maxTownCountField;
        public Dropdown typingDifficultyDropDown;

        public List<GameObject> playerInfoList;
        public GameObject hostLobbyInfo;
        public GameObject levelOptionsPanel;
        public LevelControlsUI levelControlsUI;

        public Dragontale.Thassanor thassanor;

        //public GameObject characterDropDown;
        public Dropdown dropDown;

        public Text characterSelectText;

        public GameObject localIcone;
        public GameObject remoteIcone;

        public bool isHost = true;

        //OnMyName function will be invoked on clients when server change the value of playerName
        [SyncVar(hook = "OnMyName")]
        public string playerName = "";
        [SyncVar(hook = "OnMyColor")]
        public Color playerColor = Color.white;
        [SyncVar(hook = "OnCharacterSelect")]
        public int playerCharacterIndex = 0;
        //[SyncVar(hook = "OnSpell1Change")]
        //public int spell1Index = 0;
        //[SyncVar(hook = "OnSpell2Change")]
        //public int spell2Index = 1;
        //[SyncVar(hook = "OnDifficultyChange")]
        //public int typingDifficulty = 0;

        //[SyncVar]
        //public int _itSeed;
        //[SyncVar]
        //public int _columns;
        //[SyncVar]
        //public int _rows;
        //[SyncVar]
        //public int _waterSize;
        //[SyncVar]
        //public int _townSpread;
        //[SyncVar]
        //public int _maxTownCount;

        private string playerCharacterName;
        public string levelDifficultyString;
        private string spell1Name;
        private string spell2Name;

        public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        static Color JoinColor = new Color(255.0f / 255.0f, 0.0f, 101.0f / 255.0f, 1.0f);
        static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
        static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        static Color TransparentColor = new Color(0, 0, 0, 0);

        //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        private void Awake()
        {
            playerCharacterName = "Shousei";
            levelDifficultyString = "Easy";
            levelOptionsPanel = GameObject.FindGameObjectWithTag("LevelOptions");
            levelControlsUI = FindObjectOfType<LevelControlsUI>();
            thassanor = FindObjectOfType<Dragontale.Thassanor>();
        }


        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

            playerInfoList.AddRange(GameObject.FindGameObjectsWithTag("PlayerInfo"));
            seedField = levelControlsUI.seedField;
            mapSizeDropDown = levelControlsUI.mapSizeDropDown;
            waterSizeField = levelControlsUI.waterSizeField;
            townSpreadField = levelControlsUI.townSpreadField;
            maxTownCountField = levelControlsUI.maxTownCountField;
            typingDifficultyDropDown = levelControlsUI.typingDifficultyDropDown;
            FindObjectOfType<CharacterSelect>().spell1DropDown = spell1DropDown;
            FindObjectOfType<CharacterSelect>().spell2DropDown = spell2DropDown;

            foreach (var player in playerInfoList)
            {
                if (player.GetComponent<LobbyPlayer>().isHost)
                {
                    hostLobbyInfo = player;
                }
            }

            if (isLocalPlayer)
            {
                Debug.Log("Player is local");
                SetupLocalPlayer();
                SetUpLevelOptionsPanel();
            }
            else
            {
                Debug.Log("Player is NOT local");
                SetupOtherPlayer();
                SetUpLevelOptionsPanel();
            }

            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            OnMyName(playerName);
            OnMyColor(playerColor);
            OnCharacterChanged(playerCharacterIndex);
            //OnDifficultyChange(typingDifficulty);
            //OnSpell1Change(spell1Index);
            //OnSpell2Change(spell2Index);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            //if we return from a game, color of text can still be the one for "Ready"
            readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

            SetupLocalPlayer();
        }

        void ChangeReadyButtonColor(Color c)
        {
            ColorBlock b = readyButton.colors;
            b.normalColor = c;
            b.pressedColor = c;
            b.highlightedColor = c;
            b.disabledColor = c;
            readyButton.colors = b;
        }

        void SetupOtherPlayer()
        {
            nameInput.interactable = false;
            removePlayerButton.interactable = NetworkServer.active;
            dropDown.interactable = false;
            spell1DropDown.interactable = false;
            spell2DropDown.interactable = false;

            ChangeReadyButtonColor(NotReadyColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
            readyButton.interactable = false;

            OnClientReady(false);
        }

        void SetupLocalPlayer()
        {
            nameInput.interactable = true;
            remoteIcone.gameObject.SetActive(false);
            localIcone.gameObject.SetActive(true);
            levelOptionsPanel.SetActive(true);

            if (playerInfoList[0].GetComponent<LobbyPlayer>().nameInput.interactable == false)
            {
                Debug.Log("Player is Host");
                isHost = false;
                SetUpLevelOptionsPanel();
            }

            CheckRemoveButton();

            if (playerColor == Color.white)
            {
                CmdColorChange();
            }

            ChangeReadyButtonColor(JoinColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
            readyButton.interactable = true;

            //have to use child count of player prefab already setup as "this.slot" is not set yet
            if (playerName == "")
                CmdNameChanged("Player" + (LobbyPlayerList._instance.playerListContentTransform.childCount - 1));

            //we switch from simple name display to name input
            colorButton.interactable = true;
            nameInput.interactable = true;
            dropDown.interactable = true;
            spell1DropDown.interactable = false;
            spell2DropDown.interactable = false;

            nameInput.onEndEdit.RemoveAllListeners();
            nameInput.onEndEdit.AddListener(OnNameChanged);

            colorButton.onClick.RemoveAllListeners();
            colorButton.onClick.AddListener(OnColorClicked);

            dropDown.onValueChanged.RemoveAllListeners();
            dropDown.onValueChanged.AddListener(OnCharacterChanged);

            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(OnReadyClicked);

            //typingDifficultyDropDown.onValueChanged.RemoveAllListeners();
            //typingDifficultyDropDown.onValueChanged.AddListener(OnDifficultyChanged);

            //spell1DropDown.onValueChanged.RemoveAllListeners();
            //spell1DropDown.onValueChanged.AddListener(OnSpell1Changed);

            //spell2DropDown.onValueChanged.RemoveAllListeners();
            //spell2DropDown.onValueChanged.AddListener(OnSpell2Changed);

            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
        }

        private void SetUpLevelOptionsPanel()
        {
            GameObject.FindGameObjectWithTag("LevelOptions").GetComponent<LevelControlsUI>().seedField.interactable = false;
            GameObject.FindGameObjectWithTag("LevelOptions").GetComponent<LevelControlsUI>().mapSizeDropDown.interactable = false;
            GameObject.FindGameObjectWithTag("LevelOptions").GetComponent<LevelControlsUI>().waterSizeField.interactable = false;
            GameObject.FindGameObjectWithTag("LevelOptions").GetComponent<LevelControlsUI>().townSpreadField.interactable = false;
            GameObject.FindGameObjectWithTag("LevelOptions").GetComponent<LevelControlsUI>().maxTownCountField.interactable = false;
            GameObject.FindGameObjectWithTag("LevelOptions").GetComponent<LevelControlsUI>().typingDifficultyDropDown.interactable = false;
        }

        //This enable/disable the remove button depending on if that is the only local player or not
        public void CheckRemoveButton()
        {
            if (!isLocalPlayer)
                return;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            removePlayerButton.interactable = localPlayerCount > 1;
        }

        public override void OnClientReady(bool readyState)
        {
            if (readyState)
            {
                ChangeReadyButtonColor(TransparentColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = "READY";
                textComponent.color = ReadyColor;
                readyButton.interactable = false;
                colorButton.interactable = false;
                nameInput.interactable = false;
                dropDown.interactable = false;
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = isLocalPlayer ? "JOIN" : "...";
                textComponent.color = Color.white;
                readyButton.interactable = isLocalPlayer;
                colorButton.interactable = isLocalPlayer;
                nameInput.interactable = isLocalPlayer;
                dropDown.interactable = isLocalPlayer;
            }
        }

        public void OnPlayerListChanged(int idx)
        {
            GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
        }

        ///===== callback from sync var

        public void OnMyName(string newName)
        {
            playerName = newName;
            nameInput.text = playerName;
        }

        public void OnMyColor(Color newColor)
        {
            playerColor = newColor;
            colorButton.GetComponent<Image>().color = newColor;
        }

        public void OnCharacterSelect(int newIndex)
        {
            playerCharacterIndex = newIndex;
            playerCharacterName = dropDown.options[newIndex].text;
            dropDown.captionText.text = playerCharacterName;
        }

        //public void OnSpell1Change(int newIndex)
        //{
        //    spell1Index = newIndex;
        //    spell1Name = spell1DropDown.options[newIndex].text;
        //    spell1DropDown.captionText.text = spell1Name;
        //}
        //public void OnSpell2Change(int newIndex)
        //{
        //    spell2Index = newIndex;
        //    spell2Name = spell2DropDown.options[newIndex].text;
        //    spell2DropDown.captionText.text = spell2Name;
        //}

        //public void OnDifficultyChange(int num)
        //{
        //    Debug.Log("Difficulty changed");
        //    typingDifficulty = num;
        //    levelDifficultyString = levelOptionsPanel.GetComponent<Dropdown>().options[num].text;
        //    levelOptionsPanel.GetComponent<Dropdown>().captionText.text = levelDifficultyString;
        //    storedDifficulty = hostLobbyInfo.GetComponent<LobbyPlayer>().typingDifficulty;
        //    Debug.Log(storedDifficulty);
        //}

        //===== UI Handler

        //Note that those handler use Command function, as we need to change the value on the server not locally
        //so that all client get the new value throught syncvar
        public void OnColorClicked()
        {
            CmdColorChange();
        }

        public void OnReadyClicked()
        {
            SendReadyToBeginMessage();
        }

        public void OnNameChanged(string str)
        {
            CmdNameChanged(str);
        }

        public void OnCharacterChanged(int index)
        {
            CmdCharacterChanged(index);
        }

        //public void OnSpell1Changed(int index)
        //{
        //    CmdSpell1Changed(index);
        //}

        //public void OnSpell2Changed(int index)
        //{
        //    CmdSpell2Changed(index);
        //}

        //public void OnDifficultyChanged(int num)
        //{
        //    CmdTypingDifficultyChanged(num);
        //}

        public void OnRemovePlayerClick()
        {
            if (isLocalPlayer)
            {
                RemovePlayer();
            }
            else if (isServer)
                LobbyManager.s_Singleton.KickPlayer(connectionToClient);

        }

        public void ToggleJoinButton(bool enabled)
        {
            readyButton.gameObject.SetActive(enabled);
            waitingPlayerButton.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {
            LobbyManager.s_Singleton.countdownPanel.UIText.text = "Match Starting in " + countdown;
            LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
        }

        [ClientRpc]
        public void RpcUpdateRemoveButton()
        {
            CheckRemoveButton();
        }

        //====== Server Command

        [Command]
        public void CmdColorChange()
        {
            int idx = System.Array.IndexOf(Colors, playerColor);

            int inUseIdx = _colorInUse.IndexOf(idx);

            if (idx < 0) idx = 0;

            idx = (idx + 1) % Colors.Length;

            bool alreadyInUse = false;

            do
            {
                alreadyInUse = false;
                for (int i = 0; i < _colorInUse.Count; ++i)
                {
                    if (_colorInUse[i] == idx)
                    {//that color is already in use
                        alreadyInUse = true;
                        idx = (idx + 1) % Colors.Length;
                    }
                }
            }
            while (alreadyInUse);

            if (inUseIdx >= 0)
            {//if we already add an entry in the colorTabs, we change it
                _colorInUse[inUseIdx] = idx;
            }
            else
            {//else we add it
                _colorInUse.Add(idx);
            }

            playerColor = Colors[idx];

            //RpcSendColorToClient(playerColor);

            OnMyColor(playerColor);
        }

        //[ClientRpc]
        //public void RpcSendColorToClient(Color newColor)
        //{
        //    playerColor = newColor;
        //}

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        [Command]
        public void CmdCharacterChanged(int index)
        {
            playerCharacterIndex = index;
            playerCharacterName = dropDown.options[index].text;
            OnCharacterSelect(index);
        }

        //[Command]
        //public void CmdSpell1Changed(int index)
        //{
        //    spell1Index = index;
        //    spell1Name = spell1DropDown.options[index].text;
        //    OnSpell1Change(index);
        //}

        //[Command]
        //public void CmdSpell2Changed(int index)
        //{
        //    spell2Index = index;
        //    spell2Name = spell2DropDown.options[index].text;
        //    OnSpell2Change(index);
        //}

        //[Command]
        //public void CmdTypingDifficultyChanged(int num)
        //{
        //    Debug.Log("sending difficulty to server");
        //    //typingDifficulty = num;
        //    Debug.Log(typingDifficulty);
        //    levelDifficultyString = typingDifficultyDropDown.options[num].text;
        //    OnDifficultyChange(num);
        //}

        //[Command]
        //public void CmdSeedChanged(int num)
        //{
        //    _itSeed = num;
        //}

        //[Command]
        //public void CmdColumnsChanged(int num)
        //{
        //    _columns = num;
        //    _rows = num;
        //}

        //[Command]
        //public void CmdWaterSizeChanged(int num)
        //{
        //    _waterSize = num;
        //}

        //[Command]
        //public void CmdTownSpreadChanged(int num)
        //{
        //    _townSpread = num;
        //}

        //[Command]
        //public void CmdMaxTownCountChanged(int num)
        //{
        //    _maxTownCount = num;
        //}

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            LobbyPlayerList._instance.RemovePlayer(this);
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

            int idx = System.Array.IndexOf(Colors, playerColor);

            if (idx < 0)
                return;

            for (int i = 0; i < _colorInUse.Count; ++i)
            {
                if (_colorInUse[i] == idx)
                {//that color is already in use
                    _colorInUse.RemoveAt(i);
                    break;
                }
            }
        }
    }
}

