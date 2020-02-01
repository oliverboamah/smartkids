using System;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public PlayerInfo ThePlayerInfo;
    public Image theBoard;
    public Transform toonCool;
    public ToonControl toonScript;
    public float movingVelocity = 15.0f;
    public float InstantSpeed = 1.0f;
    public float buuttonScaleOffset = 0.8f;
    public float HelpQuitScaleOffset = 12f;

    public AudioSource AudioSample;
    public AudioSource LevelSample;
    public AudioClip[] Speeches;
    public AudioClip[] Sfx;
    public Text Math_Question;
    public Text ShowCharText;
    public GameObject[] Math_Objectives;
    private Text[] Math_Option;

    public Text[] _3_Letters;
    public Text[] _4_Letters;
    public Text[] _5_Letters;
    public Text[] _6_Letters;
    public Text[] _7_Letters;
    private GameObject[] _All_Spell_Letter;
    public Text[][] _All_Letters;
    public int[] _Type_Store;
    public int _Current_Length;
    public int _Type_Pointer;
    public int _Type_Position;
    public int _Del_Position;
    private AudioClip _SpellMath_Audios;

    public int _Current_Task;
    public RectTransform NameGender;
    public RectTransform Level;
    public RectTransform Cash;
    public RectTransform Quit;
    public RectTransform QuitInfo;
    public RectTransform Help;
    public RectTransform HelpInfo;
    public RectTransform NoMoneyInfo;
    public RectTransform Sound;
    public RectTransform TheBoard;
    public Transform KeyBoard;

    public bool speedUp;
    public float[] TransPos;
    public float[] CashScale = new float[] { 0.23f, 2.25f };
    public float[] HelpScale = new float[] { 1.15f, 0.05f, 2.15f };

    private bool[] playedSpeeches;
    private bool[] playedSfx;
    private bool[] ReplayQuestion;
    private float playStop;
    private float clipLength;
    private float ToonMainRot;
    private bool EndIntroUI;
    private bool MainUpdate;
    private bool OldPlayer;
    public Image SpellImage;
    public Text SpellMeaning;
    private Collider tappedUi = null;
    private GameObject theKey = null;
    private RectTransform Quit_X;
    private Transform KeyBoard_X;

    private char _C;
    private Transform _T;


    private float delay = 0;
    private float curserDelay = 0.5f;
    private bool blink = false;

    private bool _SpellMath_Replay = false;
    private int QuitUi;
    private int correct;
    private int deductCash = 10;
    private int _keepDeductCash;
    private RaycastHit Hit;
    private RectTransform SpellBoard;
    private Image Shine;
    private int _indexMeaning = -1;
    private int _indexImage = -1;
    private int _indexA;
    private int _indexB;
    public int _totalFails;
    public int _starsMade;
    private bool _indexControl = false;
    public string[] playerMathQuestions;
    private string[] playerSpellQuestions;
    private string[] playerSpellGif;
    private string[] playerSpellMeaning;
    private bool ShowImageMeaning;
    System.Random rand = new System.Random();

    int LevelMoney(int currentLevel)
    {
        if (currentLevel == 1)
            return 500;
        else if (currentLevel == 2)
            return 1000;
        else if (currentLevel == 3)
            return 1500;
        else if (currentLevel == 4)
            return 3000;
        else if (currentLevel == 5)
            return 6000;
        else if (currentLevel == 6)
            return 12000;
        else return 30000;
    }

    void GifAnim(string frameName, int totalFrames, float framesPerSecond)
    {
        int index = (int)(Time.time * framesPerSecond) % totalFrames + 1;
        if (SpellImage.sprite == null || SpellImage.sprite.name != frameName + "_" + (totalFrames - index).ToString("0000") + "_Layer " + index)
            SpellImage.sprite = Resources.Load<Sprite>(@"T2S\Spell-0" + ThePlayerInfo.level + "\\" + frameName + "\\" + frameName + "_" + (totalFrames - index).ToString("0000") + "_Layer " + index);
    }

    void Randomize<T>(T[] items)
    {
        // For each spot in the array, pick
        // a random item to swap into that spot.
        for (int i = 0, j = items.Length; i < j - 1; i++)
        {
            int k = rand.Next(i, j);
            T temp = items[i];
            items[i] = items[k];
            items[k] = temp;
        }
    }

    void PlayOnce(int speechIndex, int sfxIndex = -1, int levels = -1)
    {
        if (levels > -1)
        {
            if (!ReplayQuestion[levels])
            {
                LevelSample.clip = _SpellMath_Audios;
                clipLength = LevelSample.clip.length + Time.time + 0.5f;
                playStop = LevelSample.clip.length - 0.4f < 0 ? LevelSample.clip.length : LevelSample.clip.length - 0.4f;
                LevelSample.Play();
                ReplayQuestion[levels] = true;
            }
        }
        else if (sfxIndex < 0)
        {
            if (!playedSpeeches[speechIndex])
            {
                AudioSample.clip = Speeches[speechIndex];
                clipLength = AudioSample.clip.length + Time.time + 0.5f;
                if (speedUp) clipLength *= 0;
                playStop = AudioSample.clip.length - 0.4f < 0 ? AudioSample.clip.length : AudioSample.clip.length - 0.4f;
                AudioSample.Play();
                playedSpeeches[speechIndex] = true;
            }
        }
        else if (!playedSfx[sfxIndex])
        {
            AudioSample.clip = Sfx[sfxIndex];
            clipLength = AudioSample.clip.length + Time.time + 0.5f;
            playStop = AudioSample.clip.length - 0.4f < 0 ? AudioSample.clip.length : AudioSample.clip.length - 0.4f;
            AudioSample.Play();
            playedSfx[sfxIndex] = true;
        }
    }

    void NextQuestion()
    {
        for (int i = 0; i < _Current_Length; i++)
        {
            _All_Letters[_Current_Length - 3][i].text = "_";
            _All_Letters[_Current_Length - 3][i].color = Color.black;
            _All_Letters[_Current_Length - 3][i].tag = "Untagged";
        }

        for (int i = rand.Next(0, _Current_Length), j = rand.Next(1, _Current_Length); j > 0; i = rand.Next(0, _Current_Length), j--)
        {
            _All_Letters[_Current_Length - 3][i].text = char.ToUpperInvariant(playerSpellQuestions[_Current_Task][i]).ToString();
            _All_Letters[_Current_Length - 3][i].color = Color.white;
            _All_Letters[_Current_Length - 3][i].tag = "Spell";
        }

        ShowImageMeaning = true;
    }

    void MeaningImageShow()
    {
        if (ShowImageMeaning)
        {
            if (ThePlayerInfo.level > 1)
            {
                if (_indexMeaning == -1)
                    if (char.IsUpper(playerSpellQuestions[_Current_Task][playerSpellQuestions[_Current_Task].Length - 1]))
                    {
                        if (!SpellMeaning.gameObject.activeSelf)
                            SpellMeaning.gameObject.SetActive(true);
                        SpellMeaning.text = playerSpellMeaning[Array.IndexOf<string>(playerSpellMeaning, playerSpellQuestions[_Current_Task]) + playerSpellMeaning.Length / 2];
                        _indexMeaning = 0;
                    }
                if(_indexMeaning == 0)
                    if (SpellBoard.eulerAngles.y < 180f || _All_Spell_Letter[_Current_Length - 3].transform.localScale.x > 0f || SpellMeaning.transform.localScale.x < 0.07 || SpellMeaning.transform.localScale.y < 0.35)
                    {
                        SpellBoard.eulerAngles = Vector3.up * Mathf.Lerp(SpellBoard.eulerAngles.y, 180f + 0.01f, Time.deltaTime * movingVelocity);
                        _All_Spell_Letter[_Current_Length - 3].transform.localScale = new Vector2(Mathf.Lerp(_All_Spell_Letter[_Current_Length - 3].transform.localScale.x, 0f - 0.01f, Time.deltaTime * movingVelocity), Mathf.Lerp(_All_Spell_Letter[_Current_Length - 3].transform.localScale.y, 0f - 0.01f, Time.deltaTime * movingVelocity));
                        SpellMeaning.transform.localScale = new Vector2(Mathf.Lerp(SpellMeaning.transform.localScale.x, 0.07f + 0.01f, Time.deltaTime * movingVelocity), Mathf.Lerp(SpellMeaning.transform.localScale.y, 0.35f + 0.01f, Time.deltaTime * movingVelocity));
                    }
                    else _indexMeaning = 1;
                
                if(_indexMeaning == 1)
                    if (Time.time > clipLength + 3.0f)
                        if (Math.Abs(SpellBoard.eulerAngles.y) > 0.01f || _All_Spell_Letter[_Current_Length - 3].transform.localScale.x < 0f || SpellMeaning.transform.localScale.x > 0)
                        {
                            SpellBoard.eulerAngles = Vector3.up * Mathf.Lerp(SpellBoard.eulerAngles.y, 0.00f, Time.deltaTime * movingVelocity);
                            _All_Spell_Letter[_Current_Length - 3].transform.localScale = new Vector2(Mathf.Lerp(_All_Spell_Letter[_Current_Length - 3].transform.localScale.x, 1f + 0.01f, Time.deltaTime * movingVelocity), Mathf.Lerp(_All_Spell_Letter[_Current_Length - 3].transform.localScale.y, 1f + 0.01f, Time.deltaTime * movingVelocity));
                            SpellMeaning.transform.localScale = new Vector2(Mathf.Lerp(SpellMeaning.transform.localScale.x, 0f - 0.01f, Time.deltaTime * movingVelocity), Mathf.Lerp(SpellMeaning.transform.localScale.y, 0f - 0.01f, Time.deltaTime * movingVelocity));
                        }
                        else
                        {
                            SpellMeaning.gameObject.SetActive(ShowImageMeaning = (_indexMeaning = -1) > 0);
                            return;
                        }
            }
            if (_indexMeaning == -1)
            {
                if (!SpellImage.gameObject.activeSelf)
                    SpellImage.gameObject.SetActive(true);
                if (_indexImage == -1)
                    if (char.IsLower(playerSpellQuestions[_Current_Task][0]))
                    {
                        _indexImage = Array.IndexOf<string>(playerSpellGif, playerSpellQuestions[_Current_Task]) + playerSpellGif.Length / 2;
                        _indexA = int.Parse(playerSpellGif[_indexImage].Substring(0, playerSpellGif[_indexImage].IndexOf('-')));
                        _indexB = int.Parse(playerSpellGif[_indexImage].Substring(playerSpellGif[_indexImage].IndexOf('-'), playerSpellGif[_indexImage].Length - playerSpellGif[_indexImage].IndexOf('-')).Trim('-'));
                    }
                    else _indexImage = -2;
                if (_indexImage == -2)
                    SpellImage.sprite = Resources.Load<Sprite>(@"T2S\Spell-0" + ThePlayerInfo.level + "\\" + playerSpellQuestions[_Current_Task]);
                else GifAnim(playerSpellQuestions[_Current_Task], _indexA, _indexB);
                if (!_indexControl)
                    if (SpellBoard.eulerAngles.y < 180f || _All_Spell_Letter[_Current_Length - 3].transform.localScale.x > 0f || SpellImage.transform.localScale.x < 1)
                    {
                        SpellBoard.eulerAngles = Vector3.up * Mathf.Lerp(SpellBoard.eulerAngles.y, 180f + 0.01f, Time.deltaTime * movingVelocity);
                        _All_Spell_Letter[_Current_Length - 3].transform.localScale = new Vector2(Mathf.Lerp(_All_Spell_Letter[_Current_Length - 3].transform.localScale.x, 0f - 0.01f, Time.deltaTime * movingVelocity), Mathf.Lerp(_All_Spell_Letter[_Current_Length - 3].transform.localScale.y, 0f - 0.01f, Time.deltaTime * movingVelocity));
                        SpellImage.transform.localScale = new Vector2(Mathf.Lerp(SpellImage.transform.localScale.x, 1f + 0.01f, Time.deltaTime * movingVelocity), Mathf.Lerp(SpellImage.transform.localScale.y, 1f + 0.01f, Time.deltaTime * movingVelocity));
                    }
                    else _indexControl = true;
                else if (Time.time > clipLength + (_indexA > 15 ? 1.5f : 0.5f))
                    if (Math.Abs(SpellBoard.eulerAngles.y) > 0.01f || _All_Spell_Letter[_Current_Length - 3].transform.localScale.x < 0f || SpellImage.transform.localScale.x > 0)
                    {
                        SpellBoard.eulerAngles = Vector3.up * Mathf.Lerp(SpellBoard.eulerAngles.y, 0.00f, Time.deltaTime * movingVelocity);
                        _All_Spell_Letter[_Current_Length - 3].transform.localScale = new Vector2(Mathf.Lerp(_All_Spell_Letter[_Current_Length - 3].transform.localScale.x, 1f + 0.01f, Time.deltaTime * movingVelocity), Mathf.Lerp(_All_Spell_Letter[_Current_Length - 3].transform.localScale.y, 1f + 0.01f, Time.deltaTime * movingVelocity));
                        SpellImage.transform.localScale = new Vector2(Mathf.Lerp(SpellImage.transform.localScale.x, 0f - 0.01f, Time.deltaTime * movingVelocity), Mathf.Lerp(SpellImage.transform.localScale.y, 0f - 0.01f, Time.deltaTime * movingVelocity));
                    }
                    else SpellImage.gameObject.SetActive(ShowImageMeaning = _indexControl = (_indexA = _indexImage = -1) > 0);
            }
        }
    }

    void Start()
    {
        Quit_X = Quit.Find("Quit") as RectTransform;
        KeyBoard_X = KeyBoard.Find("Alphabets");
        Shine = (Help.Find("Shine")).GetComponent<Image>();
        playedSpeeches = new bool[Speeches.Length];
        playedSfx = new bool[Sfx.Length];
        ToonMainRot = toonCool.eulerAngles.y;

        if (!ThePlayerInfo.userSpelling)
            Speeches[8] = Resources.Load<AudioClip>(@"T2S\03\This is the Math board");

        if (ThePlayerInfo.userSpelling)
        {
            SpellBoard = TheBoard.Find("SpellBoard").transform as RectTransform;
            ShowCharText.text = "Show next character for ₵" + (_keepDeductCash = (deductCash *= ThePlayerInfo.level));
            playerSpellQuestions = Resources.Load<TextAsset>(@"T2S\Spell-0" + ThePlayerInfo.level + "\\_" + ThePlayerInfo.level).text.Split('\n');
            if (ThePlayerInfo.level < 3)
                playerSpellGif = Resources.Load<TextAsset>(@"T2S\Spell-0" + ThePlayerInfo.level + "\\__" + ThePlayerInfo.level).text.Split('\n');
            if (ThePlayerInfo.level > 1)
                playerSpellMeaning = Resources.Load<TextAsset>(@"T2S\Spell-0" + ThePlayerInfo.level + "\\___" + ThePlayerInfo.level).text.Split('\n');
            Randomize<string>(playerSpellQuestions);

            _Type_Store = new int[7];
            _Current_Length = playerSpellQuestions[_Current_Task].Length;
            ReplayQuestion = new bool[1];

            _All_Letters = new Text[5][];
            _All_Letters[0] = _3_Letters;
            _All_Letters[1] = _4_Letters;
            _All_Letters[2] = _5_Letters;
            _All_Letters[3] = _6_Letters;
            _All_Letters[4] = _7_Letters;

            _All_Spell_Letter = new GameObject[5];
            for (int i = 0; i < 5; i++)
                _All_Spell_Letter[i] = SpellBoard.Find(i + 3 + "-Letters").gameObject;
            _All_Spell_Letter[_Current_Length - 3].SetActive(true);

            NextQuestion();
        }
        else
        {
            ShowCharText.text = "Hide 2 Options for ₵" + (_keepDeductCash = (deductCash *= ThePlayerInfo.level));       
            playerMathQuestions = Resources.Load<TextAsset>(@"T2S\Math-0" + ThePlayerInfo.level + "\\_" + ThePlayerInfo.level).text.Split('\n');
            Randomize<string>(playerMathQuestions);
            Math_Question.text = Resources.Load<TextAsset>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2)).text + "?";
            if (char.IsLower(playerMathQuestions[_Current_Task][0]))
            {
                Math_Question.transform.parent.localScale = new Vector2(0.08f, 0.35f);
                Math_Question.fontSize = 60;
            }
            else
            {
                Math_Question.transform.parent.localScale = new Vector2(0.08f, 0.87f);
                Math_Question.fontSize = 30;
            }
            Math_Option = new Text[4];
            (Math_Option[0] = Math_Objectives[0].GetComponentInChildren<Text>(true)).text = Resources.Load<TextAsset>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_A").text.Replace(',', '.');
            (Math_Option[1] = Math_Objectives[1].GetComponentInChildren<Text>(true)).text = Resources.Load<TextAsset>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_B").text.Replace(',', '.');
            (Math_Option[2] = Math_Objectives[2].GetComponentInChildren<Text>(true)).text = Resources.Load<TextAsset>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_C").text.Replace(',', '.');
            (Math_Option[3] = Math_Objectives[3].GetComponentInChildren<Text>(true)).text = Resources.Load<TextAsset>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_D").text.Replace(',', '.');
            ReplayQuestion = new bool[5];
        }
    }

    void AllowKeyboard()
    {
        if (QuitInfo.localScale.x > 1 || HelpInfo.localScale.x > 1 || ShowImageMeaning)
            return;

        _C = Input.inputString.Length > 0 ? Input.inputString[0] : '0';
        if (_C == '0' && _T != null)
        {
            playedSfx[2] = false;
            _T.localScale = new Vector3(_T.name.Length > 1 ? 1.75f : 1, 1f, 1f);
            _T = null;
        }
        else PlayOnce(-1, 2);

        if (_C > 96 && _C < 123)
        {
            (_T = ThePlayerInfo.Keyboard.transform.Find("Alphabets").Find((_C = char.ToUpperInvariant(_C)).ToString()).transform).localScale = new Vector3(1.4f, 0.8f, 0.8f);
            if (_Type_Position < _Current_Length)
            {
                _Type_Store[_Type_Pointer++] = _Type_Position;
                _All_Letters[_Current_Length - 3][_Type_Position++].text = _T.name;
            }
        }
        else if (_C == '\b')
        {
            (_T = ThePlayerInfo.Keyboard.transform.Find("Alphabets").Find("Del").transform).localScale = new Vector3(0.8f, 0.8f, 0.8f);
            if (_Type_Pointer > 0)
            {
                _Del_Position = _Type_Position;
                _Type_Position = _Type_Store[--_Type_Pointer];
            }
        }
    }

    void PlayerUpdate()
    {
        string _savedDataStr = ThePlayerInfo.savedData;
        ThePlayerInfo.cashInCedis -= LevelMoney(ThePlayerInfo.level);
        if (ThePlayerInfo.userSpelling)
            ThePlayerInfo.savedData = ThePlayerInfo.savedData.Replace("Level-" + ThePlayerInfo.level, "Level-" + ((ThePlayerInfo.level + 1) == 4 ? 1 : (ThePlayerInfo.level + 1)));
        ThePlayerInfo.savedData = ThePlayerInfo.savedData.Replace(ThePlayerInfo.savedData.Substring(ThePlayerInfo.savedData.IndexOf('-') + 2, ThePlayerInfo.savedData.IndexOf('L') - 1 - ThePlayerInfo.savedData.IndexOf('-') - 2), ThePlayerInfo.cashInCedis.ToString());

        if (deductCash == _keepDeductCash && _totalFails == 0)
            _starsMade = 51;
        else if (deductCash <= _keepDeductCash * 4 && _totalFails <= 3)
            _starsMade = 50;
        else _starsMade = 49;

        char[] _savedDataChar = ThePlayerInfo.savedData.ToCharArray();
        if (_savedDataChar[_savedDataChar.Length - (15 - (ThePlayerInfo.level - 1) * 2)] < (char)_starsMade)
            _savedDataChar[_savedDataChar.Length - (15 - (ThePlayerInfo.level - 1) * 2)] = (char)_starsMade;

        ThePlayerInfo._AllPlayers = ThePlayerInfo._AllPlayers.Replace(ThePlayerInfo.currentPlayer + '=' + _savedDataStr,
                                                                      ThePlayerInfo.currentPlayer + '=' + new string(_savedDataChar));
        PlayerPrefs.SetString("SmartKids", ThePlayerInfo._AllPlayers + '[' + ThePlayerInfo.currentPlayer + "-OldPlayer]");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/03.MainGame_[1280_X_720]");
        Debug.ClearDeveloperConsole();
    }
    
    void _MainUpdate()
    {
        MeaningImageShow();

        #region CorrectSpelling
        if (ThePlayerInfo.userSpelling)
        {
            #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                AllowKeyboard();
            #endif

            if (correct < 1 && _Type_Position == _Current_Length && theKey == null)
                correct++;
            if (correct > 0)
            {
                if (correct < 2)
                {
                    for (int i = (correct = 2) - 2; correct < 3 && i < _Current_Length; i++)
                        correct = _All_Letters[_Current_Length - 3][i].text[0] == Char.ToUpperInvariant(playerSpellQuestions[_Current_Task][i]) ? 2 : 3;
                    _SpellMath_Audios = Resources.Load<AudioClip>(correct < 3 ? @"T2S\_Co-rect" : @"T2S\_No, That is Wrong!");
                    if (correct < 3)
                        ThePlayerInfo.Cash.text = (ThePlayerInfo.cashInCedis += (100 * ThePlayerInfo.level)).ToString("N0");
                    else
                    {
                        _totalFails++;
                        ThePlayerInfo.Cash.text = (ThePlayerInfo.cashInCedis = Math.Max(ThePlayerInfo.cashInCedis - (100 * ThePlayerInfo.level), 0)).ToString("N0");
                    }
                    PlayOnce(-1, -1, 0);
                    ReplayQuestion[0] = false;
                    toonScript.PlayMotion[0] = true;
                }
                if (Time.time > clipLength)
                {
                    ThePlayerInfo.Cash.transform.localScale = new Vector3(Mathf.Lerp(ThePlayerInfo.Cash.transform.localScale.x, 2.212f, Time.deltaTime * (movingVelocity + InstantSpeed)), Mathf.Lerp(ThePlayerInfo.Cash.transform.localScale.y, 1.675f, Time.deltaTime * (movingVelocity + InstantSpeed)), 1);
                    ThePlayerInfo.Cash.color = Color.white;
                    if (ThePlayerInfo.cashInCedis >= LevelMoney(ThePlayerInfo.level))
                    {
                        PlayerUpdate();
                        return;
                    }
                    if (correct < 3)
                    {
                        _All_Spell_Letter[_Current_Length - 3].SetActive(false);
                        for (int i = _Type_Pointer - 1; i >= 0; i--)
                            _All_Letters[_Current_Length - 3][_Type_Store[i]].text = "_";
                        if (_Current_Task > 18)
                            Randomize<string>(playerSpellQuestions);
                        _Current_Length = playerSpellQuestions[_Current_Task = ++_Current_Task % 20].Length;
                        _All_Spell_Letter[_Current_Length - 3].SetActive(true);
                        NextQuestion();
                        _Del_Position = _Type_Position = _Type_Pointer = 0;
                    }
                    else
                    {
                        for (int i = _Type_Pointer - 1; i >= 0; i--)
                            _All_Letters[_Current_Length - 3][_Type_Store[i]].text = "_";
                        _Del_Position = _Type_Position = _Type_Pointer = _Type_Store[0];
                    }
                    _SpellMath_Replay = ShowImageMeaning = true;
                    correct = 0;
                }
                else
                {
                    if (correct < 3)
                        ThePlayerInfo.Cash.color = new Color(0, Mathf.PingPong(Time.time, 0.4f) + 0.6f, 0, 1);
                    else ThePlayerInfo.Cash.color = new Color(Mathf.PingPong(Time.time, 0.7f) + 0.3f, 0, 0, 1);
                    ThePlayerInfo.Cash.transform.localScale = new Vector3(Mathf.PingPong(Time.time, CashScale[0] / 2) + CashScale[1], Mathf.PingPong(Time.time, CashScale[0] / 2) + CashScale[1], 1);
                }
                return;
            }
        }
        #endregion

        #region UI-HitHandler
        if (!ShowImageMeaning && tappedUi == null && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.touchCount > 0 ? new Vector3(Input.touches[0].position.x, Input.touches[0].position.y) : Input.mousePosition), out Hit, 100))
            {
                _SpellMath_Replay = (tappedUi = Hit.collider) != null && (toonScript.PlayMotion[2] = false);
                LevelSample.Stop();
            }
        if (tappedUi != null && tappedUi.tag[0] == '!')
            tappedUi = null;
        #endregion

        #region SoundUI
        if (tappedUi != null && tappedUi.name[0] == 'S' && tappedUi.name.Length == 5)
        {
            (theKey = tappedUi.gameObject).transform.localScale *= buuttonScaleOffset;
            if (ThePlayerInfo.userSpelling)
                ReplayQuestion[0] = false;
            else Array.Clear(ReplayQuestion, 0, 5);
            PlayOnce(-1, 1);
            playedSfx[1] = false;
            _SpellMath_Replay = (tappedUi = null) == null;
        }
        #endregion

        #region Replay
        if (_SpellMath_Replay && !LevelSample.isPlaying && Time.time > clipLength - 0.8f)
            if (!ReplayQuestion[0])
            {
                _SpellMath_Audios = Resources.Load<AudioClip>(ThePlayerInfo.userSpelling ? @"T2S\Spell-0" + ThePlayerInfo.level + "\\" + playerSpellQuestions[_Current_Task] : @"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2));
                PlayOnce(-1, -1, 0);
                toonScript.PlayMotion[0] = toonScript.PlayMotion[2] = true;
            }
            else if (ThePlayerInfo.userSpelling)
            {
                if (ReplayQuestion[0] && Time.time > clipLength)
                    ReplayQuestion[0] = _SpellMath_Replay = toonScript.PlayMotion[2] = false;
            }
            else if (ReplayQuestion[0] && Time.time > clipLength && !ReplayQuestion[1])
            {
                toonScript.PlayMotion[2] = false;
                Math_Objectives[0].SetActive(true);
                _SpellMath_Audios = Resources.Load<AudioClip>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_A");
                PlayOnce(-1, -1, 1);
                toonScript.PlayMotion[0] = true;
            }
            else if (ReplayQuestion[1] && Time.time > clipLength && !ReplayQuestion[2])
            {
                Math_Objectives[1].SetActive(true);
                _SpellMath_Audios = Resources.Load<AudioClip>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_B");
                PlayOnce(-1, -1, 2);
                toonScript.PlayMotion[0] = true;
            }
            else if (ReplayQuestion[2] && Time.time > clipLength && !ReplayQuestion[3])
            {
                Math_Objectives[2].SetActive(true);
                _SpellMath_Audios = Resources.Load<AudioClip>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_C");
                PlayOnce(-1, -1, 3);
                toonScript.PlayMotion[0] = true;
            }
            else if (ReplayQuestion[3] && Time.time > clipLength && !ReplayQuestion[4])
            {
                Math_Objectives[3].SetActive(true);
                _SpellMath_Audios = Resources.Load<AudioClip>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_D");
                PlayOnce(-1, -1, 4);
                toonScript.PlayMotion[0] = true;
            }
            else if (ReplayQuestion[4] && Time.time > clipLength)
            {
                Array.Clear(ReplayQuestion, 0, 5);
                toonScript.PlayMotion[2] = _SpellMath_Replay = false;
            }
        #endregion

        #region MathOptionHit
        if (tappedUi != null && tappedUi.tag[0] == 'M')
        {
            if (correct < 1)
                correct++;

            if (correct == 1)
            {
                PlayOnce(-1, 2);
                (theKey = tappedUi.gameObject).transform.localScale *= buuttonScaleOffset;
                if (tappedUi.name[1] == char.ToUpperInvariant(playerMathQuestions[_Current_Task][0]))
                {
                    _SpellMath_Audios = Resources.Load<AudioClip>(@"T2S\_Co-rect");
                    ThePlayerInfo.Cash.text = (ThePlayerInfo.cashInCedis += (100 * ThePlayerInfo.level)).ToString("N0");
                }
                else
                {
                    _SpellMath_Audios = Resources.Load<AudioClip>(@"T2S\_No, That is Wrong!");
                    ThePlayerInfo.Cash.text = (ThePlayerInfo.cashInCedis = Math.Max(ThePlayerInfo.cashInCedis - (100 * ThePlayerInfo.level), 0)).ToString("N0");
                    _totalFails++;
                }
                ReplayQuestion[0] = false;
                PlayOnce(-1, -1, 0);
                ReplayQuestion[0] = false;
                toonScript.PlayMotion[0] = true;
                correct++;
            }
            if (Time.time > clipLength)
            {
                ThePlayerInfo.Cash.transform.localScale = new Vector3(Mathf.Lerp(ThePlayerInfo.Cash.transform.localScale.x, 2.212f, Time.deltaTime * (movingVelocity + InstantSpeed)), Mathf.Lerp(ThePlayerInfo.Cash.transform.localScale.y, 1.675f, Time.deltaTime * (movingVelocity + InstantSpeed)), 1);
                ThePlayerInfo.Cash.color = Color.white;
                if (ThePlayerInfo.cashInCedis >= 1000)
                {
                    PlayerUpdate();
                    return;
                }

                if (tappedUi.name[1] == char.ToUpperInvariant(playerMathQuestions[_Current_Task][0]))
                {
                    if (_Current_Task > 8)
                        Randomize<string>(playerMathQuestions);
                    Math_Question.text = Resources.Load<TextAsset>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task = ++_Current_Task % 10].Substring(2, playerMathQuestions[_Current_Task].Length - 2)).text + "?";

                    if (char.IsLower(playerMathQuestions[_Current_Task][0]))
                    {
                        Math_Question.transform.parent.localScale = new Vector2(0.08f, 0.35f);
                        Math_Question.fontSize = 60;
                    }
                    else
                    {
                        Math_Question.transform.parent.localScale = new Vector2(0.08f, 0.87f);
                        Math_Question.fontSize = 30;
                    }

                    (Math_Option[0] = Math_Objectives[0].GetComponentInChildren<Text>(true)).text = Resources.Load<TextAsset>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_A").text.Replace(',', '.');
                    (Math_Option[1] = Math_Objectives[1].GetComponentInChildren<Text>(true)).text = Resources.Load<TextAsset>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_B").text.Replace(',', '.');
                    (Math_Option[2] = Math_Objectives[2].GetComponentInChildren<Text>(true)).text = Resources.Load<TextAsset>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_C").text.Replace(',', '.');
                    (Math_Option[3] = Math_Objectives[3].GetComponentInChildren<Text>(true)).text = Resources.Load<TextAsset>(@"T2S\Math-0" + ThePlayerInfo.level + "\\" + playerMathQuestions[_Current_Task].Substring(2, playerMathQuestions[_Current_Task].Length - 2) + "_D").text.Replace(',', '.');
                    Array.Clear(ReplayQuestion, 0, 5);
                    for (int i = 0; i < 4; i++)
                    {
                        Math_Objectives[i].GetComponent<Image>().color = Color.white;
                        Math_Objectives[i].SetActive(false);
                    }

                }
                _SpellMath_Replay = true;
                tappedUi = null;
                correct = 0;
            }
            else
            {
                if (tappedUi.name[1] == char.ToUpperInvariant(playerMathQuestions[_Current_Task][0]))
                    ThePlayerInfo.Cash.color = new Color(0, Mathf.PingPong(Time.time, 0.4f) + 0.6f, 0, 1);
                else ThePlayerInfo.Cash.color = new Color(Mathf.PingPong(Time.time, 0.7f) + 0.3f, 0, 0, 1);
                ThePlayerInfo.Cash.transform.localScale = new Vector3(Mathf.PingPong(Time.time, CashScale[0] / 2) + CashScale[1], Mathf.PingPong(Time.time, CashScale[0] / 2) + CashScale[1], 1);
            }

        }
        #endregion

        #region HelpUI

        if (tappedUi != null && tappedUi.name[0] == 'H' && tappedUi.name.Length > 1)
        {
            if (!ThePlayerInfo.userSpelling)
                if (!Math_Objectives[3].activeSelf || Math_Objectives[0].GetComponent<Image>().color.r != 1 ||
                    Math_Objectives[1].GetComponent<Image>().color.r != 1 ||
                    Math_Objectives[2].GetComponent<Image>().color.r != 1 ||
                    Math_Objectives[3].GetComponent<Image>().color.r != 1)
                    tappedUi = null;

            if (tappedUi != null)
                if (HelpInfo.localScale.x < HelpQuitScaleOffset)
                {
                    if (!playedSfx[0]) (theKey = tappedUi.gameObject).transform.localScale *= buuttonScaleOffset;
                    PlayOnce(-1, 0);
                    Vector2 HelpPos = HelpInfo.localScale;
                    HelpPos.x = HelpPos.y = Mathf.Lerp(HelpPos.y, HelpQuitScaleOffset + 0.01f, Time.deltaTime * movingVelocity);
                    HelpInfo.localScale = HelpPos;
                }
                else { playedSfx[0] = false; tappedUi = null; }
        }

        if (tappedUi != null && tappedUi.name[0] == 'X' && tappedUi.name.Length > 1)
            if (HelpInfo.localScale.x > 0.01f)
            {
                PlayOnce(-1, 1);
                Vector2 HelpPos = HelpInfo.localScale;
                HelpPos.x = HelpPos.y = Mathf.Lerp(HelpPos.y, 0f, Time.deltaTime * movingVelocity);
                HelpInfo.localScale = HelpPos;
            }
            else { playedSfx[1] = false; tappedUi = null; }

        if (tappedUi != null && tappedUi.name[0] == 'S' && tappedUi.name.Length > 1)
        {
            PlayOnce(-1, 0);
            if (ThePlayerInfo.cashInCedis < deductCash)
            {
                if (NoMoneyInfo.localScale.x < HelpQuitScaleOffset || HelpInfo.localScale.x > 0.01f)
                {
                    Vector2 HelpPos = HelpInfo.localScale;
                    HelpPos.x = HelpPos.y = Mathf.Lerp(HelpPos.y, 0f, Time.deltaTime * movingVelocity);
                    HelpInfo.localScale = HelpPos;

                    Vector2 ShowPos = NoMoneyInfo.localScale;
                    ShowPos.x = ShowPos.y = Mathf.Lerp(ShowPos.y, HelpQuitScaleOffset + 0.01f, Time.deltaTime * movingVelocity);
                    NoMoneyInfo.localScale = ShowPos;
                }
                else { playedSfx[0] = false; tappedUi = null; }
            }
            else
                if (HelpInfo.localScale.x > 0.01f)
                {
                    PlayOnce(-1, 0);
                    Vector2 HelpPos = HelpInfo.localScale;
                    HelpPos.x = HelpPos.y = Mathf.Lerp(HelpPos.y, 0f, Time.deltaTime * movingVelocity);
                    HelpInfo.localScale = HelpPos;
                }
                else if (ThePlayerInfo.userSpelling)
                {
                    for (int i = 0; i < _Current_Length; i++)
                    {
                        if (_All_Letters[_Current_Length - 3][i].text[0] != Char.ToUpperInvariant(playerSpellQuestions[_Current_Task][i]))
                        {
                            _All_Letters[_Current_Length - 3][i].text = char.ToUpperInvariant(playerSpellQuestions[_Current_Task][i]).ToString();
                            _All_Letters[_Current_Length - 3][i].color = Color.white;
                            _All_Letters[_Current_Length - 3][i].tag = "Spell";
                            ThePlayerInfo.Cash.text = (ThePlayerInfo.cashInCedis = Math.Max(ThePlayerInfo.cashInCedis - deductCash, 0)).ToString("N0"); ;
                            ShowCharText.text = "Show next character for ₵" + (deductCash *= 2);
                            if (_Type_Position == _Current_Length)
                                correct = 1;
                            i = _Current_Length;
                        }
                        playedSfx[0] = false; tappedUi = null;
                    }
                }
                else
                {
                    switch (char.ToUpperInvariant(playerMathQuestions[_Current_Task][0]))
                    {
                        case 'A':
                            Math_Objectives[1].GetComponent<Image>().color = Math_Objectives[1].GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
                            break;
                        case 'B':
                            Math_Objectives[0].GetComponent<Image>().color = Math_Objectives[2].GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
                            break;
                        case 'C':
                            Math_Objectives[1].GetComponent<Image>().color = Math_Objectives[3].GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
                            break;
                        default:
                            Math_Objectives[0].GetComponent<Image>().color = Math_Objectives[2].GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
                            break;
                    }

                    ThePlayerInfo.Cash.text = (ThePlayerInfo.cashInCedis = Math.Max(ThePlayerInfo.cashInCedis - deductCash, 0)).ToString("N0"); ;
                    ShowCharText.text = "Hide 2 Options for ₵" + (deductCash *= 2);
                    playedSfx[0] = false; tappedUi = null;
                }
        }
        if (tappedUi != null && tappedUi.name[0] == 'O' && tappedUi.name.Length > 1)
            if (NoMoneyInfo.localScale.x > 0.01f)
            {
                PlayOnce(-1, 1);
                Vector2 NoMoneyPos = NoMoneyInfo.localScale;
                NoMoneyPos.x = NoMoneyPos.y = Mathf.Lerp(NoMoneyPos.y, 0f, Time.deltaTime * movingVelocity);
                NoMoneyInfo.localScale = NoMoneyPos;
            }
            else { playedSfx[1] = false; tappedUi = null; }
        #endregion

        #region QuitUI
        if (tappedUi != null && tappedUi.name[0] == 'Q' && tappedUi.name.Length == 9)
        {
            if (QuitInfo.localScale.x < HelpQuitScaleOffset)
            {
                PlayOnce(-1, 0);
                Vector2 QuitPos = QuitInfo.localScale;
                QuitPos.x = QuitPos.y = Mathf.Lerp(QuitPos.y, HelpQuitScaleOffset + 0.01f, Time.deltaTime * movingVelocity);
                QuitInfo.localScale = QuitPos;
            }
            else
            {
                playedSfx[0] = false;
                PlayOnce(-1, 3);
                tappedUi = null;
            }
        }

        if (tappedUi != null && tappedUi.name[0] == 'Q')
            if (tappedUi.name.Length == 6)
            {
                if (AudioSample.isPlaying && AudioSample.clip.name == Sfx[3].name)
                    AudioSample.Stop();
                if (QuitInfo.localScale.x > 0.01f)
                {
                    PlayOnce(-1, 1);
                    Vector2 QuitPos = QuitInfo.localScale;
                    QuitPos.x = QuitPos.y = Mathf.Lerp(QuitPos.y, 0f, Time.deltaTime * movingVelocity);
                    QuitInfo.localScale = QuitPos;
                }
                else { playedSfx[1] = playedSfx[3] = false; tappedUi = null; }
            }
            else if (tappedUi.name.Length == 7)
                if (QuitInfo.localScale.x > 0.01f)
                {
                    PlayOnce(-1, 1);
                    Vector2 QuitPos = QuitInfo.localScale;
                    QuitPos.x = QuitPos.y = Mathf.Lerp(QuitPos.y, 0f, Time.deltaTime * movingVelocity);
                    QuitInfo.localScale = QuitPos;
                }
                else if (!playedSfx[4])
                {
                    toonScript.PlayMotion[0] = true;
                    PlayOnce(-1, 4);
                }
                else if (playedSfx[4] && Time.time > clipLength && !playedSfx[5])
                {
                    NameGender.gameObject.SetActive(false);
                    Level.gameObject.SetActive(false);
                    Cash.gameObject.SetActive(false);
                    Quit.gameObject.SetActive(false);
                    Help.gameObject.SetActive(false);
                    Sound.gameObject.SetActive(false);
                    TheBoard.gameObject.SetActive(false);
                    KeyBoard.gameObject.SetActive(false);
                    toonScript.PlayMotion[0] = true;
                    toonScript.PlayMotion[1] = true;
                    PlayOnce(-1, 5);
                }
                else if (playedSfx[5] && Time.time > clipLength && !toonScript.PlayMotion[1])
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit ();
                 #endif

        #endregion

        #region KeyPress
        if (tappedUi != null && tappedUi.tag[0] == 'K' && theKey == null)
        {
            PlayOnce(-1, 2);
            (theKey = tappedUi.gameObject).transform.localScale *= buuttonScaleOffset;

            if (theKey.name.Length > 1)
            {
                if (_Type_Pointer > 0)
                {
                    _Del_Position = _Type_Position;
                    _Type_Position = _Type_Store[--_Type_Pointer];
                }
            }
            else if (_Type_Position < _Current_Length)
            {
                _Type_Store[_Type_Pointer++] = _Type_Position;
                _All_Letters[_Current_Length - 3][_Type_Position++].text = theKey.name;
            }
            tappedUi = null;
        }

        if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) && theKey != null)
        {
            playedSfx[2] = false;
            theKey.transform.localScale /= buuttonScaleOffset;
            theKey = null;
        }
        #endregion
    }

    void Update()
    {
        #region QuitBack
        if (QuitUi == 0 && Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
            PlayOnce(-1, (QuitUi = 1) - 1);

        if (QuitUi == 1)
            if (QuitInfo.localScale.x < HelpQuitScaleOffset)
                QuitInfo.localScale = new Vector2(Mathf.Lerp(QuitInfo.localScale.x, HelpQuitScaleOffset + 0.01f, Time.deltaTime * movingVelocity), QuitInfo.localScale.x);
            else if ((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.touchCount > 0 ? new Vector3(Input.touches[0].position.x, Input.touches[0].position.y) : Input.mousePosition), out Hit, 100))
                QuitUi = 2;

        if (QuitUi > 1)
            if (Hit.collider.name.Length == 6)
                if (QuitInfo.localScale.x > 0.01f)
                {
                    if (QuitUi == 2) PlayOnce(++QuitUi - 2);
                    QuitInfo.localScale = new Vector2(Mathf.Lerp(QuitInfo.localScale.x, 0f, Time.deltaTime * movingVelocity), QuitInfo.localScale.x);
                }
                else QuitUi = 0;
            else if (Hit.collider.name.Length == 7)
            {
                if (QuitUi == 2) PlayOnce(++QuitUi - 2);
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                     Application.Quit ();
                #endif
            }

        if (QuitUi > 0)
            return;
        #endregion

        if (ThePlayerInfo.showLevel < 2)
            return;

        if (toonScript.PlayMotion[0] && (AudioSample.time >= playStop || LevelSample.time >= playStop))
            toonScript.PlayMotion[0] = false;

        if (Help.localPosition.y <= 187.21f)
            Shine.color = new Color(1, 1, 1, Mathf.PingPong(Time.time / 5, 0.10f) + 0.10f);

        #region SpellCursorBlink
        if (ThePlayerInfo.userSpelling && TheBoard.localPosition.y >= 614.00f)
        {
            if ((delay += Time.deltaTime) > curserDelay && (delay = 0) < 1)
                blink = !blink;
            if (_Type_Position < _Current_Length)
            {
                if (_All_Letters[_Current_Length - 3][_Type_Position].tag[0] == 'S')
                    ++_Type_Position;
                else _All_Letters[_Current_Length - 3][_Type_Position].text = blink ? " " : "_";
                if (_Del_Position > _Type_Position && _Del_Position < _Current_Length)
                    _All_Letters[_Current_Length - 3][_Del_Position].text = "_";
            }
        }
        #endregion

        if (MainUpdate)
        {
            if (OldPlayer)
            {
                ThePlayerInfo.PlayerName.color = new Color(ThePlayerInfo.PlayerName.color.r, ThePlayerInfo.PlayerName.color.g, ThePlayerInfo.PlayerName.color.b, Mathf.Lerp(ThePlayerInfo.PlayerName.color.a, 0.89f, Time.deltaTime * 3f));
                ThePlayerInfo.Level.transform.localPosition = new Vector2(Mathf.Lerp(ThePlayerInfo.Level.transform.localPosition.x, 116.83f, Time.deltaTime * movingVelocity), ThePlayerInfo.Level.transform.localPosition.y);
                ThePlayerInfo.Level.transform.localScale = new Vector3(Mathf.Lerp(ThePlayerInfo.Level.transform.localScale.x, 1.00f, Time.deltaTime * movingVelocity), ThePlayerInfo.Level.transform.localScale.y, 1);
                ThePlayerInfo.Level.color = new Color(ThePlayerInfo.Level.color.r, ThePlayerInfo.Level.color.g, ThePlayerInfo.Level.color.b, Mathf.Lerp(ThePlayerInfo.Level.color.a, 0.89f, Time.deltaTime * movingVelocity));
                Sound.localScale = new Vector2(Mathf.Lerp(Sound.localScale.x, 2.901f, Time.deltaTime * movingVelocity), Mathf.Lerp(Sound.localScale.y, 2.901f, Time.deltaTime * movingVelocity));
                Help.transform.localScale = new Vector2(Mathf.Lerp(Help.transform.localScale.x, 3.3f, Time.deltaTime * (movingVelocity + InstantSpeed)), Mathf.Lerp(Help.transform.localScale.y, 1.31f, Time.deltaTime * (movingVelocity + InstantSpeed)));

                if (ThePlayerInfo.PlayerName.color.a >= 0.88f && ThePlayerInfo.Level.transform.localPosition.x > 116.82f && ThePlayerInfo.Level.transform.localScale.x > 0.99f && ThePlayerInfo.Level.color.a >= 0.88f
                    && Sound.localScale.x >= 2.900f && Sound.localScale.y >= 2.900f && Help.transform.localScale.x >= 2.29f && Help.transform.localScale.y >= 1.30f)
                {
                    Sound.GetComponent<BoxCollider>().enabled = OldPlayer = false;
                    Sound.GetComponent<BoxCollider>().enabled = _SpellMath_Replay = true;
                }
            }
            else _MainUpdate();

            return;
        }

        #region Help
        if (playedSpeeches[6] && Help.localPosition.y > 187.21f)
        {
            if (Time.time > clipLength)
            {
                EndIntroUI = true;
                Help.transform.localScale = new Vector3(Mathf.PingPong(Time.time, HelpScale[0]) + HelpScale[2], Mathf.PingPong(Time.time, HelpScale[1]) + HelpScale[2], 1);
                Vector3 HelpPos = Help.localPosition;
                HelpPos.y = Mathf.Lerp(HelpPos.y, 187.20f, Time.deltaTime * (movingVelocity + InstantSpeed));
                Help.localPosition = HelpPos;
            }
        }
        else if (Help.localPosition.y <= 187.21f && toonCool.eulerAngles.y > 240.51f)
        {
            Help.transform.localScale = new Vector3(Mathf.PingPong(Time.time, HelpScale[0]) + HelpScale[2], Mathf.PingPong(Time.time, HelpScale[1]) + HelpScale[2], 1);
            Vector3 toonRot = toonCool.eulerAngles;
            toonRot.y = Mathf.Lerp(toonRot.y, 240.50f, Time.deltaTime * movingVelocity / 2);
            toonCool.eulerAngles = toonRot;
        }
        else if (Help.localPosition.y <= 187.21f && toonCool.eulerAngles.y <= 240.51f && !playedSpeeches[7])
        {
            Help.transform.localScale = new Vector3(Mathf.PingPong(Time.time, HelpScale[0]) + HelpScale[2], Mathf.PingPong(Time.time, HelpScale[1]) + HelpScale[2], 1);
            toonScript.PlayMotion[0] = true;
            PlayOnce(7);
        }
        #endregion

        else if (playedSpeeches[7] && toonCool.eulerAngles.y > ToonMainRot)
            if (Time.time > clipLength)
            {
                if (toonScript.PlayMotion[2])
                    toonScript.PlayMotion[2] = false;
                Help.transform.localScale = new Vector2(Mathf.Lerp(Help.transform.localScale.x, 3.3f, Time.deltaTime * (movingVelocity + InstantSpeed)), Mathf.Lerp(Help.transform.localScale.y, 1.31f, Time.deltaTime * (movingVelocity + InstantSpeed)));
                Vector3 toonRot = toonCool.eulerAngles;
                toonRot.y = Mathf.Lerp(toonRot.y, ToonMainRot - 0.01f, Time.deltaTime * movingVelocity);
                toonCool.eulerAngles = toonRot;
            }
            else Help.transform.localScale = new Vector3(Mathf.PingPong(Time.time, HelpScale[0]) + HelpScale[2], Mathf.PingPong(Time.time, HelpScale[1]) + HelpScale[2], 1);

        #region TheBoard
        else if (playedSpeeches[7] && toonCool.eulerAngles.y <= ToonMainRot && TheBoard.localPosition.y < 614.00f)
        {
            TheBoard.Rotate(Vector3.up, -350f * Time.deltaTime);
            Vector3 TheBoardPos = TheBoard.localPosition;
            TheBoardPos.y = Mathf.Lerp(TheBoardPos.y, 614.01f, Time.deltaTime * (movingVelocity + InstantSpeed));
            TheBoard.localPosition = TheBoardPos;
        }
        else if (TheBoard.localPosition.y >= 614.00f && (!playedSpeeches[8] || Mathf.Abs(TheBoard.eulerAngles.y) > 0.01f))
        {
            TheBoard.eulerAngles = Vector3.up * Mathf.Lerp(TheBoard.eulerAngles.y, 0.00f, Time.deltaTime * movingVelocity);
            if (!toonScript.PlayMotion[0])
                toonScript.PlayMotion[0] = true;
            PlayOnce(8);
        }
        #endregion

        else if (!ThePlayerInfo.userSpelling)
        {
            if (!playedSpeeches[11] && playedSpeeches[8] && Time.time > clipLength + 1.25f && (toonScript.PlayMotion[0] = true))
                PlayOnce(11);
            else if (playedSpeeches[11] && Time.time > clipLength && (MainUpdate = _SpellMath_Replay = true))
            {
                PlayerPrefs.SetString("SmartKids", ThePlayerInfo._AllPlayers + '[' + ThePlayerInfo.currentPlayer + "-OldPlayer]"); return;
            }
        }

        #region KeyBoard
        else if (playedSpeeches[8] && Time.time > clipLength && KeyBoard.localPosition.y < -0.56f)
        {
            KeyBoard_X.localEulerAngles = new Vector3(KeyBoard_X.transform.localEulerAngles.x, -350f * Time.deltaTime, KeyBoard_X.transform.localEulerAngles.z);
            Vector3 KeyBoardPos = KeyBoard.localPosition;
            KeyBoardPos.y = Mathf.Lerp(KeyBoardPos.y, -0.55f, Time.deltaTime * (movingVelocity + InstantSpeed));
            KeyBoard.localPosition = KeyBoardPos;
        }
        else if (Mathf.Abs(KeyBoard_X.eulerAngles.y) > 0.01f || KeyBoard.localPosition.y >= -0.56f && !playedSpeeches[9] && (toonScript.PlayMotion[0] = true))
        {
            KeyBoard_X.localEulerAngles = new Vector3(KeyBoard_X.localEulerAngles.x, Mathf.Lerp(KeyBoard_X.localEulerAngles.y, 0.00f, Time.deltaTime * movingVelocity), KeyBoard_X.localEulerAngles.z);
            PlayOnce(9);
        }
        #endregion

        else if (!playedSpeeches[11] && playedSpeeches[9] && Time.time > clipLength + 1.25f && (toonScript.PlayMotion[0] = true))
            PlayOnce(11);
        else if (playedSpeeches[11] && Time.time > clipLength && (MainUpdate = _SpellMath_Replay = true))
        {
            PlayerPrefs.SetString("SmartKids", ThePlayerInfo._AllPlayers + '[' + ThePlayerInfo.currentPlayer + "-OldPlayer]"); return;
        }

        if (EndIntroUI) return;

        if (theBoard.color.a < 1.00f)
        {
            Color boardCol = theBoard.color;
            boardCol.a = Mathf.Lerp(boardCol.a, 1.01f, Time.deltaTime * movingVelocity / 5);
            theBoard.transform.localScale = new Vector2(Mathf.Lerp(theBoard.transform.localScale.x, 1.01f, Time.deltaTime * movingVelocity / 2), Mathf.Lerp(theBoard.transform.localScale.y, 1.01f, Time.deltaTime * movingVelocity / 2));
            theBoard.color = boardCol;
        }
        else if (toonCool.position.x > TransPos[0])
        {
            Vector3 toonPos = toonCool.position;
            toonPos.x = Mathf.Lerp(toonPos.x, TransPos[0] - 0.01f, Time.deltaTime * movingVelocity);
            toonCool.position = toonPos;
        }
        else if (ThePlayerInfo.playerInfo == null)
        {
            if (!playedSpeeches[10])
            {
                PlayOnce(10);
                toonScript.PlayMotion[0] = true;
            }
            else if (playedSpeeches[10] && Time.time > clipLength)
            {
                NameGender.localPosition = new Vector2(NameGender.localPosition.x, -632.00f);
                Level.localPosition = new Vector2(Level.localPosition.x, -563.10f);
                Cash.localPosition = new Vector2(Cash.localPosition.x, -563.10f);
                Quit.localPosition = new Vector2(Quit.localPosition.x, 286.01f);
                Sound.localPosition = new Vector2(434f, Sound.localPosition.y);
                Help.localPosition = new Vector2(Help.localPosition.x, 187.2f);
                TheBoard.localPosition = new Vector2(TheBoard.localPosition.x, 614f);
                KeyBoard.localPosition = new Vector3(KeyBoard.localPosition.x, -0.56f, KeyBoard.localPosition.z);

                MainUpdate = OldPlayer = true;
            }
        }
        else if (!playedSpeeches[0])
        {
            toonScript.PlayMotion[0] = toonScript.PlayMotion[1] = true;
            PlayOnce(0);
        }
        else if (!playedSpeeches[1] && Time.time > clipLength)
        {
            toonScript.PlayMotion[0] = true;
            PlayOnce(1);
        }

        #region PlayerName
        else if (playedSpeeches[1] && Time.time > clipLength && NameGender.localPosition.y > -632.00f)
        {
            Vector3 NameGenderPos = NameGender.localPosition;
            NameGenderPos.y = Mathf.Lerp(NameGenderPos.y, -632.01f, Time.deltaTime * (movingVelocity + InstantSpeed));
            NameGender.localPosition = NameGenderPos;
        }
        else if (NameGender.localPosition.y <= -632.00 && ThePlayerInfo.PlayerName.color.a < 0.88f)
        {
            ThePlayerInfo.PlayerName.color = new Color(ThePlayerInfo.PlayerName.color.r, ThePlayerInfo.PlayerName.color.g, ThePlayerInfo.PlayerName.color.b, Mathf.Lerp(ThePlayerInfo.PlayerName.color.a, 0.89f, Time.deltaTime * 1.5f));
            if (!playedSpeeches[2])
            {
                toonScript.PlayMotion[0] = true;
                toonScript.PlayMotion[2] = true;
                PlayOnce(2);
            }
        }
        #endregion

        #region Level
        else if (playedSpeeches[2] && Time.time > clipLength && Level.localPosition.y > -563.10f)
        {
            Vector3 LevelPos = Level.localPosition;
            LevelPos.y = Mathf.Lerp(LevelPos.y, -563.11f, Time.deltaTime * (movingVelocity + InstantSpeed));
            Level.localPosition = LevelPos;
        }
        else if (Level.localPosition.y <= -563.10f && toonCool.eulerAngles.y < 240.50f)
        {
            ThePlayerInfo.Level.transform.localPosition = new Vector2(Mathf.Lerp(ThePlayerInfo.Level.transform.localPosition.x, 116.83f, Time.deltaTime * movingVelocity / 2), ThePlayerInfo.Level.transform.localPosition.y);
            ThePlayerInfo.Level.transform.localScale = new Vector3(Mathf.Lerp(ThePlayerInfo.Level.transform.localScale.x, 1.00f, Time.deltaTime * movingVelocity / 2), ThePlayerInfo.Level.transform.localScale.y, 1);
            ThePlayerInfo.Level.color = new Color(ThePlayerInfo.Level.color.r, ThePlayerInfo.Level.color.g, ThePlayerInfo.Level.color.b, Mathf.Lerp(ThePlayerInfo.Level.color.a, 0.89f, Time.deltaTime * movingVelocity / 2));
            Vector3 toonRot = toonCool.eulerAngles;
            toonRot.y = Mathf.Lerp(toonRot.y, 240.51f, Time.deltaTime * movingVelocity / 2);
            toonCool.eulerAngles = toonRot;
        }
        else if (toonCool.eulerAngles.y >= 240.50f && !playedSpeeches[3])
        {
            toonScript.PlayMotion[0] = true;
            PlayOnce(3);
        }
        #endregion

        #region Cash
        else if (playedSpeeches[3] && Time.time > clipLength && Cash.localPosition.y > -563.10f)
        {
            ThePlayerInfo.Cash.transform.localScale = new Vector3(Mathf.PingPong(Time.time, CashScale[0]) + CashScale[1], Mathf.PingPong(Time.time, CashScale[0]) + CashScale[1], 1);
            Vector3 CashPos = Cash.localPosition;
            CashPos.y = Mathf.Lerp(CashPos.y, -563.11f, Time.deltaTime * (movingVelocity + InstantSpeed));
            Cash.localPosition = CashPos;
        }
        else if (Cash.localPosition.y <= -563.10f && toonCool.eulerAngles.y < 248.50f)
        {
            ThePlayerInfo.Cash.transform.localScale = new Vector3(Mathf.PingPong(Time.time, CashScale[0]) + CashScale[1], Mathf.PingPong(Time.time, CashScale[0]) + CashScale[1], 1);
            Vector3 toonRot = toonCool.eulerAngles;
            toonRot.y = Mathf.Lerp(toonRot.y, 248.51f, Time.deltaTime * movingVelocity / 2);
            toonCool.eulerAngles = toonRot;
        }
        else if (toonCool.eulerAngles.y >= 248.50f && !playedSpeeches[4])
        {
            ThePlayerInfo.Cash.transform.localScale = new Vector3(Mathf.PingPong(Time.time, CashScale[0]) + CashScale[1], Mathf.PingPong(Time.time, CashScale[0]) + CashScale[1], 1);
            toonScript.PlayMotion[0] = true;
            PlayOnce(4);
        }
        #endregion

        #region Quit
        else if (playedSpeeches[4] && Quit.localPosition.y > 286.01f)
        {
            if (Time.time > clipLength)
            {
                Quit_X.Rotate(Vector3.right, -500f * Time.deltaTime);
                ThePlayerInfo.Cash.transform.localScale = new Vector3(Mathf.Lerp(ThePlayerInfo.Cash.transform.localScale.x, 2.212f, Time.deltaTime * (movingVelocity + InstantSpeed)), Mathf.Lerp(ThePlayerInfo.Cash.transform.localScale.y, 1.675f, Time.deltaTime * (movingVelocity + InstantSpeed)), 1);
                Vector3 QuitPos = Quit.localPosition;
                QuitPos.y = Mathf.Lerp(QuitPos.y, 286.00f, Time.deltaTime * (movingVelocity + InstantSpeed));
                Quit.localPosition = QuitPos;
            }
            else ThePlayerInfo.Cash.transform.localScale = new Vector3(Mathf.PingPong(Time.time, CashScale[0]) + CashScale[1], Mathf.PingPong(Time.time, CashScale[0]) + CashScale[1], 1);
        }
        else if (Quit.localPosition.y <= 286.01f && toonCool.eulerAngles.y < 252.50f)
        {
            Quit_X.eulerAngles = Vector3.right * Mathf.Lerp(Quit_X.eulerAngles.x, 0.00f, Time.deltaTime * movingVelocity);
            Vector3 toonRot = toonCool.eulerAngles;
            toonRot.y = Mathf.Lerp(toonRot.y, 252.51f, Time.deltaTime * movingVelocity / 2);
            toonCool.eulerAngles = toonRot;
        }
        else if (toonCool.eulerAngles.y >= 252.50f && !playedSpeeches[5])
        {
            toonScript.PlayMotion[0] = true;
            PlayOnce(5);
        }
        #endregion

        #region Sound
        else if (playedSpeeches[5] && Time.time > clipLength && (Sound.localPosition.x > 434.01f || Sound.localScale.x > 2.901f || Sound.localScale.y > 2.901f))
        {
            Sound.localScale = new Vector2(Mathf.Lerp(Sound.localScale.x, 2.900f, Time.deltaTime * movingVelocity / 2), Mathf.Lerp(Sound.localScale.y, 2.900f, Time.deltaTime * movingVelocity / 2));
            Vector3 SoundPos = Sound.localPosition;
            SoundPos.x = Mathf.Lerp(SoundPos.x, 434.00f, Time.deltaTime * (movingVelocity + InstantSpeed));
            Sound.localPosition = SoundPos;
        }
        else if (Sound.localPosition.x <= 434.01f && !playedSpeeches[6])
        {
            toonScript.PlayMotion[0] = true;
            PlayOnce(6);
        }
        #endregion
    }
}