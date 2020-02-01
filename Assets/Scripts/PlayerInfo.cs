using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    public AudioSource[] AudioSample;
    public string currentPlayer = null;
    public string gender = null;
    public int cashInCedis = 0;
    public int level = 0;
    [HideInInspector]
    public int showLevel = 0;
    public float showDelay = 1.5f;
    public string playerInfo = null;
    public bool userSpelling = false;

    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI Level;
    public TextMeshProUGUI Cash;
    public Image[] Gender;
    public GameObject SpellBoard;
    public GameObject MathBoard;
    public GameObject Keyboard;
    public Text LevelTitle; 

    public float movingVelocity = 15.0f;
    public float InstantSpeed = 5.0f;

    public string savedData = null;
    private float delayLoad = 0.3f;

    public string _DataInfo = null;
    public string _AllPlayers = null;

    void Awake()
    {
        _DataInfo = PlayerPrefs.GetString("SmartKids");
        _AllPlayers = _DataInfo.Substring(0, _DataInfo.IndexOf('['));

        currentPlayer = _DataInfo.Remove(0, _DataInfo.IndexOf('[') + 1).TrimEnd(']');
        savedData = currentPlayer.Substring(0, currentPlayer.Length - 10);
        int A = _AllPlayers.IndexOf(savedData) + savedData.Length + 1,
                B = _AllPlayers.IndexOf('+', A);
        savedData = _AllPlayers.Substring(A, B - A);
        if (userSpelling = savedData.StartsWith("Spelling"))
            MathBoard.SetActive(false);
        else
        {
            SpellBoard.SetActive(false);
            Keyboard.SetActive(false);
        }

        if (currentPlayer.EndsWith("-NewPlayer"))
            playerInfo = "New";
        else playerInfo = null;

        currentPlayer = currentPlayer.Substring(0, currentPlayer.Length - 10);
        gender = currentPlayer[currentPlayer.IndexOf('_') + 1] == 'M' ? "Male" : "Female";
        if (gender[0] == 'M')
            Gender[0].gameObject.SetActive(true);
        else Gender[1].gameObject.SetActive(true);
        PlayerName.text = currentPlayer.Substring(0, currentPlayer.IndexOf('_'));
        cashInCedis = int.Parse(savedData.Substring(savedData.IndexOf('-') + 2, savedData.IndexOf('L')-1 - savedData.IndexOf('-') - 2));
        Cash.text = cashInCedis.ToString("N0");
        Level.text = LevelTitle.text = "Level " + (level = savedData[savedData.Length - 17] - 48);
        delayLoad += Time.time;
    }

    void Update()
    {
        if (showLevel == 0 && Time.time > delayLoad)
            if (LevelTitle.color.a < 1.00f)
                LevelTitle.color = new Color(1, 1, 1, Mathf.Lerp(LevelTitle.color.a, 1.01f, Time.deltaTime * movingVelocity / 2));
            else showDelay += Time.time + (showLevel = 1) - 1;
        else if (showLevel == 1)
        {
            if (AudioSample[1].clip == null)
            {
                AudioSample[1].clip = Resources.Load<AudioClip>(@"T2S\03\Level " + level);
                delayLoad = AudioSample[1].clip.length + Time.time + 0.5f;
                AudioSample[1].Play();
            }

            if (!AudioSample[1].isPlaying && !AudioSample[0].isPlaying && Time.time > delayLoad)
                AudioSample[0].Play();
            if (Time.time > showDelay)
                if (LevelTitle.color.a > 0.001f)
                    LevelTitle.color = new Color(1, 1, 1, Mathf.Lerp(LevelTitle.color.a, 0.000f, Time.deltaTime * movingVelocity / 2));
                else showLevel = 2;
        }
    }
}