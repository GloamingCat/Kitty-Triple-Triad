using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Opponent", order = 1)]
public class OpponentScriptableObject : ScriptableObject {

    public enum AI {
        None, Random, Flip
    }

    public string title;
    public string deck;
    public AI aiType;

}
