using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class KeyController : MonoBehaviour
{
    public TitleBackgroundControl _TGB_Control;

    public float scaleOffset = 0.8f;

    public Image maleCheck;
    public Image femaleCheck;

    public Text playerIntroText;
    public int maxCharacters = 16;

    private float delay = 0;
    private float curserDelay = 0.5f;
    private bool blink;
    private string _text = "";
    private string _thePlayer = null;
    private Collider theKey = null;
    private RaycastHit Hit;
    private int QuitUi;
    private char _C;
    private Transform _T;

    public GameObject LoadScreen;
    public RectTransform QuitInfo;
    public float QuitScaleOffset = 12f;

    public string _DataInfo = null;
    public string _AllPlayers = null;

    void Start()
    {
        _DataInfo = PlayerPrefs.GetString("SmartKids");
        _AllPlayers = _DataInfo.Substring(0, _DataInfo.IndexOf('['));
    }

    void Update()
    {
        #region QuitBack
        if (QuitUi == 0 && Input.GetKeyDown(KeyCode.Escape))
            _TGB_Control.PlayOnce((QuitUi = 1) + 1);

        if (QuitUi == 1)
            if (QuitInfo.localScale.x < QuitScaleOffset)
                QuitInfo.localScale = new Vector2(Mathf.Lerp(QuitInfo.localScale.x, QuitScaleOffset + 0.01f, Time.deltaTime * _TGB_Control.movingVelocity), QuitInfo.localScale.x);
            else if ((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.touchCount > 0 ? new Vector3(Input.touches[0].position.x, Input.touches[0].position.y) : Input.mousePosition), out Hit, 100))
                QuitUi = 2;

        if (QuitUi > 1)
            if (Hit.collider.name.Length == 6)
                if (QuitInfo.localScale.x > 0.01f)
                {
                    if (QuitUi == 2) _TGB_Control.PlayOnce(++QuitUi - 2);
                    QuitInfo.localScale = new Vector2(Mathf.Lerp(QuitInfo.localScale.x, 0f, Time.deltaTime * _TGB_Control.movingVelocity), QuitInfo.localScale.x);
                }
                else QuitUi = 0;
            else if (Hit.collider.name.Length == 7)
            {
                if (QuitUi == 2) _TGB_Control.PlayOnce(++QuitUi - 2);
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                     Application.Quit ();
                #endif
            }

        if (QuitUi > 0)
            return; 
        #endregion

        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
           #region AllowKeyboard
        if  (_TGB_Control.nameNext.parent.localPosition.x == 0.1f)
        {
            for (_C = Input.inputString.Length > 0 ? Input.inputString[0] : '0'; _C == '0' && _T != null; _T = null)
                _T.localScale = _T.localScale.z == 0 ? _T.localScale / (scaleOffset + 0.1f) : new Vector3(_T.name.Length > 1 ? 1.75f : 1, 1f, 1f);

            if (_C > 96 && _C < 123 && _text.Length < maxCharacters)
            {
                _TGB_Control.PlayOnce(0);
                (_T = _TGB_Control.theKeyboard.Find("Alphabets").Find((_C = char.ToUpperInvariant(_C)).ToString()).transform).localScale = new Vector3(1.4f, 0.8f, 0.8f);
                _text += char.ToUpperInvariant(_C);
            }
            else if (_C == '\n' || _C == '\r')
            {
                _TGB_Control.PlayOnce(2);
                (_T = _TGB_Control.nameNext.parent.transform).localScale *= (scaleOffset + 0.1f);
                if (playerIntroText.text.Length > 1)
                    if (_AllPlayers.Contains(_thePlayer = playerIntroText.text.ToString().Trim('_') + '_' + (maleCheck.IsActive() ? "Male" : "Female")))
                    {
                        LoadScreen.SetActive(true);
                        PlayerPrefs.SetString("SmartKids", _AllPlayers + '[' + (_thePlayer + "-OldPlayer") + ']');
                        SceneManager.LoadScene("Scenes/02.PlayerDetails_[1280_X_720]");
                    }
                    else _TGB_Control.newPlayer = true;
            }
            else if (_C == '\b' && _text.Length > 0)
            {
                _TGB_Control.PlayOnce(0);
                (_T = _TGB_Control.theKeyboard.Find("Alphabets").Find("Del").transform).localScale = new Vector3(0.8f, 0.8f, 0.8f);
                _text = _text.Remove(_text.Length - 1);
            }
        }
        
        #endregion
        #endif

        if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)) && theKey == null && Physics.Raycast(Camera.main.ScreenPointToRay(Input.touchCount > 0 ? new Vector3(Input.touches[0].position.x, Input.touches[0].position.y) : Input.mousePosition), out Hit, 100))
        {
            if ((theKey = Hit.collider).tag[0] == 'K')
            {
                _TGB_Control.PlayOnce(0);
                theKey.transform.localScale *= scaleOffset;
                if (theKey.name.Length < 2 && _text.Length < maxCharacters)
                    _text += theKey.name;
                else if (theKey.name.Length > 1 && _text.Length > 0)
                    _text = _text.Remove(_text.Length - 1);
            }
            else if (theKey.tag[0] == 'G')
            {
                _TGB_Control.PlayOnce(1);
                theKey.transform.localScale *= (scaleOffset + 0.1f);

                if (theKey.name[0] == 'M')
                    maleCheck.gameObject.SetActive(true);
                else femaleCheck.gameObject.SetActive(true);

                _TGB_Control.beforeKeyboard = false;
            }
            else if (theKey.tag[0] == 'P')
            {
                _TGB_Control.PlayOnce(2);
                theKey.transform.localScale *= (scaleOffset + 0.1f);
                if (playerIntroText.text.Length > 1)
                    if (_AllPlayers.Contains(_thePlayer = playerIntroText.text.ToString().Trim('_') + '_' + (maleCheck.IsActive() ? "Male" : "Female")))
                    {
                        LoadScreen.SetActive(true);
                        PlayerPrefs.SetString("SmartKids", _AllPlayers + '[' + (_thePlayer + "-OldPlayer") + ']');
                        SceneManager.LoadScene("Scenes/02.PlayerDetails_[1280_X_720]");
                    }
                    else _TGB_Control.newPlayer = true;
            }
            else if (theKey.gameObject.tag[0] == 'S' || theKey.gameObject.tag[0] == 'M')
            {
                _TGB_Control.PlayOnce(2);
                theKey.gameObject.transform.localScale *= (scaleOffset + 0.1f);
                LoadScreen.SetActive(true);

                _AllPlayers += (_thePlayer = playerIntroText.text.ToString().Trim('_') + '_' + (maleCheck.IsActive() ? "Male" : "Female")) +
                               (theKey.gameObject.tag[0] == 'S' ? "=Spelling-₵10-Level-1_0,0,0,0,0,0,0,0" : "=MathScience-₵10-Level-1_0,0,0,0,0,0,0,0") + '+';
                PlayerPrefs.SetString("SmartKids", _AllPlayers + '[' + (_thePlayer + "-NewPlayer") + ']');
                SceneManager.LoadScene("Scenes/02.PlayerDetails_[1280_X_720]");
            }
        }

        if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) && theKey != null)
        {
            if (theKey.gameObject.tag[0] == 'K')
                theKey.gameObject.transform.localScale /= scaleOffset;
            else if (theKey.gameObject.tag[0] == 'G' || theKey.gameObject.tag[0] == 'N' || theKey.gameObject.tag[0] == 'P' || theKey.gameObject.tag[0] == 'S' || theKey.gameObject.tag[0] == 'M')
                theKey.gameObject.transform.localScale /= (scaleOffset + 0.1f);
            theKey = null;
        }

        if (!_TGB_Control.beforeKeyboard)
        {
            string text = _text;

            if (_text.Length < maxCharacters)
            {
                text += "_";
                if (blink)
                    text = text.Remove(text.Length - 1);
            }

            playerIntroText.text = text;

            if ((delay += Time.deltaTime) > curserDelay)
            {
                delay = 0;
                blink = !blink;
            }
        }
    }
}