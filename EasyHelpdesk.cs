using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EasyHelpdesk : MonoBehaviour
{
    public Font font = null;
    public int fontSize = 20;

    public float delayBetweenQueued = 1.0f; //How long to wait between Queued messages

    public Text helpMessage = null;

    private bool _isPlaying = false;
    public bool isPlaying {  get { return _isPlaying; } set { _isPlaying = value; } }

    public Queue<HelpdeskMessage> messageQueue = null;

    private static EasyHelpdesk _self = null;
    private static void InitSingleton() { _self = new GameObject("Helpdesk").AddComponent<EasyHelpdesk>(); } //InitSingleton
    public static EasyHelpdesk self { get { if (_self == null) InitSingleton(); return _self; } } //self
    
    public static void QueueMessage(string message, float timer = 5.0f) { self.QM(message, timer); } //Message
    private void QM(string message, float timer) { messageQueue.Enqueue( new HelpdeskMessage(message, timer) ); } //QueueMessage

    public void PlayNext()
    {
        if (!isPlaying && messageQueue.Count > 0)
            StartCoroutine(Play(messageQueue.Dequeue()));
    }//playNext

    private IEnumerator Play(HelpdeskMessage message)
    {
        isPlaying = true;

        helpMessage.text = message.text;
        helpMessage.enabled = true;
        yield return new WaitForSeconds(message.timer);
        helpMessage.enabled = false;

        yield return new WaitForSeconds(delayBetweenQueued);
        isPlaying = false;
    }//Play

    void Awake ()
    {

        //There should only ever be one of these, so keep track of the singleton instance.
        if (_self == null)
            _self = this;
        else
        {
            Debug.LogError("Object \"" + name + "\" tried to create a second Helpdesk instance. Disabling. -" + GetType());
            enabled = false;
            return;
        }//else

        messageQueue = new Queue<HelpdeskMessage>();


        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            //font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        }//if

        if (helpMessage == null)
        {
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.AddComponent<CanvasScaler>();

            helpMessage = new GameObject("Helpdesk text").AddComponent<Text>();
            helpMessage.transform.SetParent(canvas.transform);

            helpMessage.font = font;
            helpMessage.fontSize = fontSize;
            
            helpMessage.text = "ASDJLASJDLASJKDLKAS";
            helpMessage.color = Color.white;

            helpMessage.alignment = TextAnchor.LowerCenter;
            helpMessage.rectTransform.sizeDelta = new Vector2(Screen.width - Screen.width * 0.2f, Screen.height / 7);
            helpMessage.rectTransform.anchorMin = new Vector2(0.5f, 0);
            helpMessage.rectTransform.anchorMax = new Vector2(0.5f, 0);
            helpMessage.horizontalOverflow = HorizontalWrapMode.Wrap;
            helpMessage.verticalOverflow = VerticalWrapMode.Overflow;
            
            
            Shadow textShadow = helpMessage.gameObject.AddComponent<Shadow>();
            textShadow.effectColor = Color.black;


            helpMessage.transform.position = new Vector3(Screen.width * 0.5f, Screen.height / 10, 0);

            helpMessage.enabled = false; //Turn it off by default.
        }//if

	}//Start
	
	void Update ()
    {
        if (!isPlaying && messageQueue.Count > 0)
            PlayNext();
	}//Update
}//EasyHelpdesk

public class HelpdeskMessage
{
    public string text = "ERROR"; //The text of the message
    public float timer = 1.0f; //How long in seconds to leave the message up
    public HelpdeskMessage(string m, float t) { text = m; timer = t; } //constructor
    public override string ToString() { return "Display " + text + " for " + timer + " seconds"; } //ToString()
}
