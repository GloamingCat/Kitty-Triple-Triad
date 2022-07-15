using UnityEngine;

[RequireComponent(typeof(Card))]
[RequireComponent(typeof(BoxCollider2D))]
public class CardButton : MonoBehaviour {

    public Card cardComponent { get; private set; }
    public bool added { get; private set; }
    public Vector3 originalPosition { get; private set; }

    protected void Awake() {
        cardComponent = GetComponent<Card>();
    }

    protected void Start() {
        originalPosition = transform.position;
    }

    protected void OnMouseOver() {
        if (enabled)
            CardSelector.Instance.SelectCard(this);
    }

    protected void OnMouseExit() {
        if (enabled)
            CardSelector.Instance.DeselectCard(this);
    }

    protected void OnMouseDown() {
        if (!enabled)
            return;
        if (added) {
            added = !CardSelector.Instance.RemoveCard(this);
        } else {
            added = CardSelector.Instance.AddCard(this);
        }
    }

}
