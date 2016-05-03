using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EasyHelpdesk : MonoBehaviour
{
    public Font defaultFont = null;
    public int defaultFontSize = 20;

    public float delayBetweenQueued = 1.0f; //How long to wait between Queued messages

    public Text helpMessage = null;

    private bool _isPlaying = false;
    public bool isPlaying {  get { return _isPlaying; } set { _isPlaying = value; } }

    public Queue<HelpdeskMessage> messageQueue = null;

    private static EasyHelpdesk _self = null;
    private static void InitSingleton() { _self = new GameObject("Helpdesk").AddComponent<EasyHelpdesk>(); } //InitSingleton
    public static EasyHelpdesk self { get { if (_self == null) InitSingleton(); return _self; } } //self
    
    public static void QueueMessage(string message, float timer = 5.0f, Font newFont = null, int size = -1)
    {
        self.QM(message, timer, newFont, size);
    } //Message

    private void QM(string message, float timer, Font newFont, int size)
    {
        messageQueue.Enqueue( new HelpdeskMessage(message, timer, newFont, size) );
    } //QueueMessage

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

        helpMessage.font = message.font != null ? message.font : defaultFont; //temporarily switch the font if asked
        helpMessage.fontSize = message.textSize > 0 ? message.textSize : defaultFontSize;

        yield return new WaitForSeconds(message.timer);

        helpMessage.enabled = false;

        helpMessage.font = defaultFont;
        helpMessage.fontSize = defaultFontSize;

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


        if (defaultFont == null)
        {
            defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            //font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        }//if

        if (helpMessage == null)
        {
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.AddComponent<CanvasScaler>();

            helpMessage = new GameObject("Helpdesk text").AddComponent<Text>();
            helpMessage.transform.SetParent(canvas.transform);

            helpMessage.font = defaultFont;
            helpMessage.fontSize = defaultFontSize;
            
            helpMessage.text = "ASDJLASJDLASJKDLKAS";
            helpMessage.color = Color.white;

            helpMessage.alignment = TextAnchor.LowerCenter;
            helpMessage.rectTransform.sizeDelta = new Vector2(Screen.width - Screen.width * 0.2f, Screen.height / 7);
            helpMessage.rectTransform.anchorMin = new Vector2(0.5f, 0);
            helpMessage.rectTransform.anchorMax = new Vector2(0.5f, 0);
            helpMessage.horizontalOverflow = HorizontalWrapMode.Wrap;
            helpMessage.verticalOverflow = VerticalWrapMode.Overflow;
            
			helpMessage.supportRichText = true;

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
    public Font font = null; 
    public int textSize = 20;
    public HelpdeskMessage(string m, float t, Font f, int s) { text = m; timer = t; font = f; textSize = s; } //constructor
    public override string ToString() { return "Display " + text + " for " + timer + " seconds"; } //ToString()
}
