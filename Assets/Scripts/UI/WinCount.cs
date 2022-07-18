using UnityEngine;
using UnityEngine.UI;

public class WinCount : MonoBehaviour {

    protected void Start() {
        int winCount = PlayerPrefs.GetInt("WinCount", 0);
        GetComponent<Text>().text += winCount;
    }

}
