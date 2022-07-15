using UnityEngine;

public class Card : MonoBehaviour {

    public bool flipped = false;
    public Sprite defaultSprite;
    public Sprite flippedSprite;

    // Movement animation
    private static float pixelMoveSpeed = 200; // Pixels per second
    private float moveTime = 1;
    private float moveSpeed;
    private Vector3 origin;
    private Vector3 destination;

    public DeckManager.Card card { get; private set; }

    // Update is called once per frame
    protected void Update() {
        if (moveTime < 1) {
            moveTime = Mathf.Min(1, moveTime + Time.deltaTime * moveSpeed);
            transform.position = Vector3.Lerp(origin, destination, moveTime);
        }
    }

    // Sets the graphics (image, value texts) of a card.
    // Also used to set the graphics of the selected card.
    public void SetCard(DeckManager.Card card) {
        if (card == null) {
            gameObject.SetActive(false);
            return;
        }
        this.card = card;
        gameObject.SetActive(true);
        SpriteRenderer image = GetComponentsInChildren<SpriteRenderer>()[1];
        image.sprite = card.image;
        TextMesh[] texts = GetComponentsInChildren<TextMesh>();
        for (int i = 0; i < 4; i++) {
            texts[i].text = card.values[i].ToString();
        }
    }

    public void MoveTo(Vector3 destination) {
        this.destination = destination;
        origin = transform.position;
        moveSpeed = pixelMoveSpeed / (destination - origin).magnitude * 60;
        moveTime = 0;
    }

    public void SetFlipped(bool value) {
        GetComponent<SpriteRenderer>().sprite = value ? flippedSprite : defaultSprite;
        flipped = value;
    }

}
