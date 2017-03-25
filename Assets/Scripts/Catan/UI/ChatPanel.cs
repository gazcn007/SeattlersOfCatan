using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon.Chat;
using UnityEngine.UI;

public class ChatPanel : MonoBehaviour, IChatClientListener {

	public ChatClient chatClient;
	public Text playerName;
	string worldChat;
	public InputField msgInput;
	public Text msgArea;


	public GameObject MessagePanel;
	public GameObject OpenPanelButton;

	void Start(){
		Application.runInBackground = true;
		if (string.IsNullOrEmpty (PhotonNetwork.PhotonServerSettings.ChatAppID)) {
			print ("No Chat App ID provided");
			return;
		}
		worldChat = "World";
		getConnected ();
	}

	void Update(){
		if (chatClient == null) {
			return; 
		}
		this.chatClient.Service ();
	}

	public void getConnected (){
		print ("Trying To Connect");
		this.chatClient = new ChatClient (this);
		this.chatClient.Connect (PhotonNetwork.PhotonServerSettings.ChatAppID, "antyhing", new ExitGames.Client.Photon.Chat.AuthenticationValues (playerName.text));
	}

	public void OnConnected() {
		print ("Connected");
		OpenPanelButton.SetActive (true);
		this.chatClient.Subscribe (new string[] { worldChat });
		this.chatClient.SetOnlineStatus (ChatUserStatus.Online);
	}

	public void sendMsg(){
		if (msgInput.text != "") {
			this.chatClient.PublishMessage (worldChat, msgInput.text);
			msgInput.text = "";
		}
	}

	public void OnDisconnected()
	{}

	public void OnGetMessages (string channelName,string[] senders,object[] messages) { 
		for (int i = 0; i < senders.Length; i++) {
			msgArea.text += senders [i] + " : " + messages [i] + "\n";
		}
	}

	public void OnPrivateMessage(string sender,object message,string channelName)
	{}

	public void OnSubscribed(string[] channels, bool[] results){ 
		this.chatClient.PublishMessage (worldChat, "joined");
	}

	public void OnUnsubscribed(string[] channels)
	{}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
	} public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
	{}

	public void OnChatStateChange(ChatState state)
	{}

	public void openPanel(){
		MessagePanel.SetActive (true);
		OpenPanelButton.SetActive (false);
	}

	public void closePanel(){
		MessagePanel.SetActive (false);
		OpenPanelButton.SetActive (true);
	}
}
