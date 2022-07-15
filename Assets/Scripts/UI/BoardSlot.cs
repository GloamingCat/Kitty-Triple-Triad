using UnityEngine;

public class BoardSlot : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    public int pos = 0;

    protected void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void OnMouseOver() {
        if (CardSelector.Instance.clickedCard != null)
            spriteRenderer.color = new Color(0.8f, 0.5f, 1, 1);
    }

    protected void OnMouseExit() {
        if (CardSelector.Instance.clickedCard != null)
            spriteRenderer.color = Color.clear;
    }

    protected void OnMouseDown() {
        if (CardSelector.Instance.clickedCard == null)
            return;
        TurnManager.Instance.PlaceCard(CardSelector.Instance.clickedCard.cardComponent, pos);
        CardSelector.Instance.PlaceCard(transform.position);
        TurnManager.Instance.NextTurn();
        Destroy(this);
    }

    protected void OnDestroy() {
        spriteRenderer.color = Color.clear;
    }

}
