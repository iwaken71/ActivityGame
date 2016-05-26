using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class InRoomChat : Photon.MonoBehaviour 
{
    public Rect GuiRect = new Rect(0,0, 250,300);
    public bool IsVisible = true;
    public bool AlignBottom = false;
    public List<string> messages = new List<string>();
    private string inputLine = "";
    private Vector2 scrollPos = Vector2.zero;
	private GUIStyle m_guiStyle;
	private GUIStyleState m_styleState;
	private ScoreScript script;



    public static readonly string ChatRPC = "Chat";

    public void Start()
    {
		m_guiStyle = new GUIStyle();
		m_styleState = new GUIStyleState();
        if (this.AlignBottom)
        {
            this.GuiRect.y = Screen.height - this.GuiRect.height;
        }
		script = GameObject.Find ("ScoreManager").GetComponent<ScoreScript>();

    }

    public void OnGUI()
    {

        if (!this.IsVisible || PhotonNetwork.connectionStateDetailed != PeerState.Joined)
        {
            return;
        }
        
        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(this.inputLine))
            {
                this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
                this.inputLine = "";
                GUI.FocusControl("");
                return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
            }
            else
            {
                GUI.FocusControl("ChatInput");
            }
        }

        GUI.SetNextControlName("");
        GUILayout.BeginArea(this.GuiRect);

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.FlexibleSpace();
        for (int i = messages.Count - 1; i >= 0; i--)
        {
			m_styleState.textColor = script.mycolor;  
			m_guiStyle.normal = m_styleState;
			//GUI.Label (messages[i],m_styleState);
			GUILayout.Label(messages[i],m_guiStyle);
        }
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("ChatInput");
        inputLine = GUILayout.TextField(inputLine);
        if (GUILayout.Button("Send", GUILayout.ExpandWidth(false)))
        {
            this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
            this.inputLine = "";
            GUI.FocusControl("");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    [PunRPC]
    public void Chat(string newLine, PhotonMessageInfo mi)
    {
        string senderName = "anonymous";

        if (mi != null && mi.sender != null)
        {
            if (!string.IsNullOrEmpty(mi.sender.name))
            {
                senderName = mi.sender.name;
            }
            else
            {
               // senderName = "player " + mi.sender.ID;
				if (GameObject.Find ("NameManager")) {
					senderName = GameObject.Find ("NameManager").GetComponent<NameManager> ().GetName ();
				} else {
					senderName = "guest";
				}
            }
        }

        this.messages.Add(senderName +": " + newLine);
		if (messages.Count > 3) {
			messages.RemoveAt (0);
		}
    }

    public void AddLine(string newLine)
    {
        this.messages.Add(newLine);
    }
}
