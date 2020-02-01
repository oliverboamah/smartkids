using UnityEngine;

public class FixMultiDisplay : MonoBehaviour
{
    void Awake()
    {
        this.transform.localScale = new Vector3(Screen.width / 1280f, Screen.height / 800f, 1);
    }
}