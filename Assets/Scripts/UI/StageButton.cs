using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageButton : MonoBehaviour {

    public OpponentScriptableObject player;
    public Text text;

    protected void Start() {
        if (!player)
            return;
        if (player.name.Equals("Alice") || player.name.Equals("Bob"))
            GetComponent<Button>().interactable = true;
        else
            GetComponent<Button>().interactable = PlayerPrefs.GetInt("stageunlocked" + player.name, 0) == 1;
        text.text = player.title;
    }

    public void SelectStage() {
        TurnManager.opponent = player;
        SceneManager.LoadScene("Play");
    }
    
}
