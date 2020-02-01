using UnityEngine;

public class LoadingScreen : MonoBehaviour
{

    void Start()
    {
       // PlayerPrefs.DeleteAll();

        if (!PlayerPrefs.HasKey("SmartKids"))
            PlayerPrefs.SetString("SmartKids", "[]");

        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/01.BeginScene_[1280_X_720]");
    }

}
