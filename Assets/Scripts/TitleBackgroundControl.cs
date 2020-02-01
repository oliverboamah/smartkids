using UnityEngine;
using UnityEngine.UI;

public class TitleBackgroundControl : MonoBehaviour
{

    public float rotateSpeed = 80.0f;
    public float InstantSpeed = 5.0f;
    public float movingVelocity = 15.0f;

    public Transform mainBackground;
    public RectTransform genderMale;
    public RectTransform genderFemale;
    public RectTransform nameNext;
    public Transform theKeyboard;
    public RectTransform spellPlay;
    public RectTransform mathPlay;
    public RectTransform InputText;

    public AudioSource AudioSample;
    public AudioClip[] Speeches;
    public Animator titleAnimator;

    public Transform[] RotateList;
    public float RotateList_Speed = -1000f;

    bool[] Played;

    [HideInInspector]
    public float countScale = 0.0f;
    [HideInInspector]
    public bool beforeKeyboard = true;
    [HideInInspector]
    public bool newPlayer = false;

    public Text inputField;
    public float[] playScale;

    public void PlayOnce(int playIndex)
    {
        if (playIndex < 3 || !Played[playIndex])
        {
            AudioSample.clip = Speeches[playIndex];
            AudioSample.Play();
            Played[playIndex] = true;
        }
    }

    void Start()
    {
        Played = new bool[Speeches.Length];
    }

    void Update()
    {
        mainBackground.Rotate(Vector3.back * rotateSpeed * Time.deltaTime);

        if (InputText.localPosition.y <= -87.51f && inputField.text.Length > 0 && inputField.text[0] != '_')
            nameNext.localScale = new Vector3(Mathf.PingPong(Time.time, playScale[0]) + playScale[1], Mathf.PingPong(Time.time, playScale[0]) + playScale[1], 0);
        else nameNext.localScale = new Vector3(Mathf.Lerp(nameNext.localScale.x, 1f, Time.deltaTime * movingVelocity), Mathf.Lerp(nameNext.localScale.y, 1f, Time.deltaTime * movingVelocity), 0);

        if (Time.time > 0.5f)
            PlayOnce(3);

        if (!titleAnimator.isActiveAndEnabled)
        {
            #region RegisterNewPlayer
            if (newPlayer)
            {
                if (theKeyboard.position.y > -1.11f)
                    theKeyboard.position = new Vector3(theKeyboard.position.x, Mathf.Lerp(theKeyboard.position.y, -1.12f, Time.deltaTime * (movingVelocity + InstantSpeed)), theKeyboard.position.z);
                else if (InputText.localPosition.y < 378.00f)
                    InputText.localPosition = new Vector2(InputText.localPosition.x, Mathf.Lerp(InputText.localPosition.y, 400.01f, Time.deltaTime * (movingVelocity + InstantSpeed)));
                else if (nameNext.parent.position.x < 4.00f)
                    nameNext.parent.position = new Vector2(Mathf.Lerp(nameNext.parent.position.x, 4.01f, Time.deltaTime * (movingVelocity + InstantSpeed/2)), nameNext.parent.position.y);            
                else if (spellPlay.localPosition.x < 0f)
                {
                    if (!AudioSample.isPlaying)
                    {
                        PlayOnce(6);
                        if (!AudioSample.isPlaying)
                        {
                            RotateList[1].Rotate(Vector3.up, RotateList_Speed * Time.deltaTime);
                            spellPlay.localPosition = new Vector2(Mathf.Lerp(spellPlay.localPosition.x, 0.1f, Time.deltaTime * (movingVelocity + InstantSpeed)), spellPlay.localPosition.y);
                        }
                    }
                }
                else if (Mathf.Abs(RotateList[1].eulerAngles.y) > 0.01f)
                    RotateList[1].eulerAngles = Vector3.up * Mathf.Lerp(RotateList[1].eulerAngles.y, 0.00f, Time.deltaTime * movingVelocity);
                else if (mathPlay.localPosition.x > 0.1f)
                {
                    PlayOnce(7);
                    if (Played[7] && !AudioSample.isPlaying)
                    {
                        RotateList[2].Rotate(Vector3.up, RotateList_Speed * Time.deltaTime);
                        mathPlay.localPosition = new Vector2(Mathf.Lerp(mathPlay.localPosition.x, 0f, Time.deltaTime * (movingVelocity + InstantSpeed)), mathPlay.localPosition.y);
                    }
                }
                else if (Mathf.Abs(RotateList[2].eulerAngles.y) > 0.01f)
                    RotateList[2].eulerAngles = Vector3.up * Mathf.Lerp(RotateList[2].eulerAngles.y, 0.00f, Time.deltaTime * movingVelocity);
                else PlayOnce(8);
                return;
            }
            #endregion

            #region BeforeNameInput
            if (beforeKeyboard)
            {
                if (transform.position.y < 1.875f)
                    transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, 1.876f, Time.deltaTime * InstantSpeed), transform.position.z);
                else if ( genderMale.localPosition.x < 255.0f)
                {
                    PlayOnce(4);
                    RotateList[4].Rotate(Vector3.up, RotateList_Speed * Time.deltaTime);
                    genderMale.localPosition = new Vector2(Mathf.Lerp(genderMale.localPosition.x, 255.1f, Time.deltaTime * (movingVelocity + InstantSpeed) * 2), genderMale.localPosition.y);
                }
                else if (Mathf.Abs(RotateList[4].eulerAngles.y) > 0.01f)
                    RotateList[4].eulerAngles = Vector3.up * Mathf.Lerp(RotateList[4].eulerAngles.y, 0.00f, Time.deltaTime * movingVelocity);
               else if (genderMale.localPosition.x >= 255.0f && genderFemale.localPosition.x > 255.1)
                {
                    RotateList[3].Rotate(Vector3.up, RotateList_Speed * Time.deltaTime);
                    genderFemale.localPosition = new Vector2(Mathf.Lerp(genderFemale.localPosition.x, 255.0f, Time.deltaTime * (movingVelocity + InstantSpeed) * 2), genderFemale.localPosition.y);
                }
                else if (Mathf.Abs(RotateList[3].eulerAngles.y) > 0.01f)
                    RotateList[3].eulerAngles = Vector3.up * Mathf.Lerp(RotateList[3].eulerAngles.y, 0.00f, Time.deltaTime * movingVelocity);
             }
            #endregion

            #region NameInputSetup
            else
            {
                if (genderMale.localPosition.x > -990.0f)
                     genderMale.localPosition = genderFemale.localPosition = new Vector2(Mathf.Lerp(genderMale.localPosition.x, -990.1f, Time.deltaTime * (movingVelocity + InstantSpeed)), genderMale.localPosition.y);
                 else if (theKeyboard.position.y < 0.64f)
                {
                    PlayOnce(5);
                    RotateList[0].Rotate(Vector3.up, RotateList_Speed * Time.deltaTime);
                    theKeyboard.position = new Vector3(theKeyboard.position.x, Mathf.Lerp(theKeyboard.position.y, 0.65f, Time.deltaTime * (movingVelocity + InstantSpeed)), theKeyboard.position.z);
                }
                else if (Mathf.Abs(RotateList[0].eulerAngles.y) > 0.01f)
                    RotateList[0].eulerAngles = Vector3.up * Mathf.Lerp(RotateList[0].eulerAngles.y, 0.00f, Time.deltaTime * movingVelocity);
                else if (InputText.localPosition.y > -87.51f)
                    InputText.localPosition = new Vector2(InputText.localPosition.x, Mathf.Lerp(InputText.localPosition.y, -87.52f, Time.deltaTime * (movingVelocity + InstantSpeed)));
                else if (nameNext.parent.localPosition.x > 0.0f)
                    nameNext.parent.localPosition = new Vector2(Mathf.Lerp(nameNext.parent.localPosition.x, 0.1f, Time.deltaTime * (movingVelocity + InstantSpeed)), nameNext.parent.localPosition.y);
            }
            #endregion
        }
    }
}
