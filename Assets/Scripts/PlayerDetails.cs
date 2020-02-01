using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerDetails : MonoBehaviour
{

    public AudioSource[] AudioSample;
    public AudioClip[] Speeches;
    public string currentPlayer = null;
    public int level = 0;
    public int[] star;
    public string playerInfo = null;
    public bool userSpelling = false;
    public bool forceNewPlayer = false;

    public RectTransform InfoBackground;
    public RectTransform InfoContinue;
    public RectTransform ChooseLevel;
    public Text InfoText;
    public float movingVelocity = 15.0f;
    public float InstantSpeed = 5.0f;
    public float writeDelay = 0.05f;
    public float delayBeforeWrite = 1.0f;
    public float[] continueScale = { 0.18f, 0.85f };
    public float buuttonScaleOffset = 0.8f;
    public GameObject LoadScreen;

    private int infoLength = 0;
    private int infoIndex = 0;
    private float delayWrite = 0;
    private float cursorDelayTimer = 0;
    private float delayLoad = 0.3f;
    private bool cursorBlink = false;
    private bool[] playedClips;
    public string savedData = null;
    private Collider tappedUi = null;
    private Transform theKey = null;
    RaycastHit hit;

    private bool continueClicked = false;

    private Image[,] Money_Star = new Image[8, 2];

    public string _DataInfo = null;
    public string _AllPlayers = null;

    void PlayOnce(int playIndex)
    {
        if (!playedClips[playIndex] || playIndex < 1)
        {
            AudioSample[1].clip = Speeches[playIndex];
            AudioSample[1].Play();
            playedClips[playIndex] = true;
        }
    }

    void Start()
    {
        playedClips = new bool[Speeches.Length];

        _DataInfo = PlayerPrefs.GetString("SmartKids");
        _AllPlayers = _DataInfo.Substring(0, _DataInfo.IndexOf('['));

        currentPlayer = _DataInfo.Remove(0, _DataInfo.IndexOf('[') + 1).TrimEnd(']');
        savedData = currentPlayer.Substring(0, currentPlayer.Length - 10);
        int A = _AllPlayers.IndexOf(savedData) + savedData.Length + 1,
            B = _AllPlayers.IndexOf('+', A);
        savedData = _AllPlayers.Substring(A, B - A);
        userSpelling = savedData.StartsWith("Spelling");
        if (currentPlayer.EndsWith("-NewPlayer") || forceNewPlayer)
            infoLength = (playerInfo = "Welcome!\nAs a new Player, we would begin with the fundamentals to help you become the best in " + (userSpelling ? "Spelling" : "Mathematics and Science")).Length;
        else playerInfo = null;
        level = savedData[savedData.Length - 17] - 48;
        star = new int[8];
        for (int i = 7, j = 1; i >= 0; i--, j += 2)
            star[i] = savedData[savedData.Length - j] - 48;

        for (int i = 0, j = 1; i < 8; i++, j += 2)
        {
            Money_Star[i, 0] = ChooseLevel.Find("Level-" + (i + 1)).Find("Enable").GetComponent<Image>();
            Money_Star[i, 1] = ChooseLevel.Find("Level-" + (i + 1)).Find("Star").GetComponent<Image>();

            if (star[i] > 0 || level >= i + 1)
            {
                if (i > 0) ChooseLevel.Find("Level-" + (i + 1)).Find("Money").gameObject.SetActive(false);
                Money_Star[i, 0].sprite = Resources.Load<Sprite>("LevelsTextures\\Level-" + (i + 1));
                Money_Star[i, 1].sprite = Resources.Load<Sprite>("LevelsTextures\\" + star[i] + "_Star");
            }
        }
        delayLoad += Time.time;
    }

    void LevelSelect()
    {

        if (InfoBackground.gameObject.activeSelf)
        {
            InfoBackground.gameObject.SetActive(false);
            InfoContinue.gameObject.SetActive(false);
        }

        if (ChooseLevel.transform.localPosition.y > 0.01f)
            ChooseLevel.transform.localPosition = new Vector2(0, Mathf.Lerp(ChooseLevel.transform.localPosition.y, 0.00f, Time.deltaTime * movingVelocity));
        else
        {
            PlayOnce(3);
            if (!AudioSample[1].isPlaying) 
                if (!AudioSample[0].isPlaying) 
                    AudioSample[0].Play();
                else AudioSample[0].volume = Mathf.Lerp(AudioSample[0].volume, 0.2f, Time.deltaTime);

            if (tappedUi == null && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.touchCount > 0 ? new Vector3(Input.touches[0].position.x, Input.touches[0].position.y) : Input.mousePosition), out hit, 100))
                tappedUi = hit.collider;

            if (tappedUi != null)
            {
                (theKey = tappedUi.transform).localScale *= buuttonScaleOffset;
                PlayOnce(0);
                if (tappedUi.GetComponent<Image>().sprite.name.Length < 8)
                {
                    LoadScreen.SetActive(true);
                    string _savedDataStr = savedData;
                    _savedDataStr = _savedDataStr.Replace("Level-" + level, "Level-" + (level = tappedUi.GetComponent<Image>().sprite.name[6] - 48));
                    _AllPlayers = _AllPlayers.Replace(currentPlayer.Substring(0, currentPlayer.Length - 10) + '=' + savedData,
                                                      currentPlayer.Substring(0, currentPlayer.Length - 10) + '=' + _savedDataStr);
                    PlayerPrefs.SetString("SmartKids", _AllPlayers + '[' + currentPlayer + ']');
                    SceneManager.LoadScene("Scenes/03.MainGame_[1280_X_720]");
                }
                else tappedUi = null;
            }
            if (theKey != null && (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)))
            {
                theKey.localScale = new Vector2(1, 1);
                theKey = null;
            }
        }
    }

    void Update()
    {
        if (Time.time <= delayLoad)
            return;

        if (infoLength < 1)
        {
            LevelSelect();
            return;
        }

        if (!continueClicked)
        {
            if (InfoBackground.localPosition.y < 54.0f)
            {
                Vector3 InfoPos = InfoBackground.localPosition;
                InfoPos.y = Mathf.Lerp(InfoPos.y, 54.1f, Time.deltaTime * (movingVelocity + InstantSpeed));
                InfoBackground.localPosition = InfoPos;
            }
            else if (InfoContinue.localPosition.y < -273.1f)
            {
                Vector3 InfoPos = InfoContinue.localPosition;
                InfoPos.y = Mathf.Lerp(InfoPos.y, -273.0f, Time.deltaTime * (movingVelocity + InstantSpeed / 2));
                InfoContinue.localPosition = InfoPos;
                delayWrite = Time.time + delayBeforeWrite;
            }
            else if (infoIndex < 1 && Time.time < delayWrite)
            {
                InfoText.text = (cursorBlink ? " " : "_");
                if (!AudioSample[0].isPlaying) AudioSample[0].Play();
            }
            else if (Time.time > delayWrite)
            {
                delayWrite = Time.time + writeDelay;

                if (playerInfo != null)
                    if (infoIndex > 7) PlayOnce(userSpelling ? 1 : 2);

                InfoText.text = InfoText.text.Remove(InfoText.text.Length - 1);

                if (infoIndex < infoLength)
                    InfoText.text += playerInfo[infoIndex++] + (cursorBlink ? " " : "_");
                else InfoText.text += (cursorBlink ? " " : "_");
            }
        }

        if (infoIndex == infoLength)
        {
            if (!AudioSample[1].isPlaying)
                AudioSample[0].volume = Mathf.Lerp(AudioSample[0].volume, 0.2f, Time.deltaTime);
            InfoContinue.localScale = new Vector3(Mathf.PingPong(Time.time, continueScale[0]) + continueScale[1], Mathf.PingPong(Time.time, continueScale[0]) + continueScale[1], transform.localScale.z);
        }

        if ((cursorDelayTimer += Time.deltaTime) > 0.5f)
        {
            cursorDelayTimer = 0;
            cursorBlink = !cursorBlink;
        }

        if ((Input.GetMouseButtonDown(0) || Input.touchCount == 1) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.touchCount > 0 ? new Vector3(Input.touches[0].position.x, Input.touches[0].position.y) : Input.mousePosition), out hit, 100))
            if (hit.collider.name[0] == 'C')
            {
                continueClicked = true;
                AudioSample[1].Stop();
                PlayOnce(0);
            }

        if (continueClicked)
            if (InfoBackground.localPosition.y < 800.0f)
            {
                Vector3 InfoPos = InfoBackground.localPosition;
                InfoPos.y = Mathf.Lerp(InfoPos.y, 832.1f, Time.deltaTime * movingVelocity);
                InfoBackground.localPosition = InfoPos;
            }
            else if (InfoContinue.localPosition.y < 505.0f)
            {
                Vector3 InfoPos = InfoContinue.localPosition;
                InfoPos.y = Mathf.Lerp(InfoPos.y, 505.1f, Time.deltaTime * movingVelocity);
                InfoContinue.localPosition = InfoPos;
            }
            else
            {
                LoadScreen.SetActive(true);
                SceneManager.LoadScene("Scenes/03.MainGame_[1280_X_720]");
            }
    }
}
