using UnityEngine;
using UnityEngine.UI;

public class CardSelector : MonoBehaviour {
    
    public static CardSelector Instance { get; private set; }
    public float cardHeight = 300;

    // Prefab
    public CardButton cardButtonPrefab;

    // State
    public CardButton selectedCard { get; private set; }
    public CardButton clickedCard { get; private set; }
    protected bool handSelectionStage;

    // Scene
    public Button startButton;
    public Card selectedCardCopy; // To visualize the current selected card.
    public Transform handParent; // List of selected (player) cards
    public Transform handParent2; // List of selected (opponent) cards
    public Transform deckParent; // List of cards to select
    public Transform boardParent;

    protected void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    // Creates button for each card in given deck.
    public void ChooseHand(DeckManager.DeckEntry[] deck, bool flipped) {
        deckParent.gameObject.SetActive(true);
        handParent2.gameObject.SetActive(false);
        handSelectionStage = true;
        for (int i = deckParent.childCount - 1; i >= 0; i--)
            Destroy(deckParent.GetChild(i).gameObject);
        Vector3 position = deckParent.transform.position;
        foreach (DeckManager.DeckEntry entry in deck) {
            DeckManager.Card card = DeckManager.Instance.FindCard(entry.cardName);
            for (int i = 0; i < entry.count; i++) {
                CardButton cardButton = Instantiate(cardButtonPrefab, position, Quaternion.identity);
                cardButton.transform.SetParent(deckParent);
                cardButton.cardComponent.SetCard(card);
                cardButton.cardComponent.SetFlipped(flipped);
                position.z -= 3;
            }
            position.y -= cardHeight * 5.0f / DeckManager.Instance.cards.Length;
        }
        startButton.interactable = false;
        selectedCardCopy.gameObject.SetActive(false);
        selectedCardCopy.transform.position = Vector3.zero;
    }

    // Creates a card instance for each card in given hand (used for AI-selected hand).
    public void CreateOpponentHand(DeckManager.Card[] hand) {
        Vector3 position = deckParent.transform.position;
        for (int i = 0; i < hand.Length; i++) {
            Card copy = Instantiate(selectedCardCopy, position, Quaternion.identity);
            copy.transform.SetParent(handParent2);
            copy.SetCard(hand[i]);
            copy.SetFlipped(true);
            position.y -= cardHeight;
            position.z -= 3;
        }
    }

    // Moves card instances to opponent hand parent (used for user-selected hand).
    public void MoveOpponentHand(Card[] hand) {
        for (int i = 0; i < hand.Length; i++) {
            Vector3 localPos = hand[i].transform.localPosition;
            hand[i].transform.SetParent(handParent2);
            hand[i].transform.localPosition = localPos;
        }
    }

    public void StartGame() {
        handSelectionStage = false;
        startButton.gameObject.SetActive(false);
        float y = -cardHeight * 2;
        selectedCardCopy.transform.Translate(0, y, 0);
        handParent.gameObject.SetActive(true);
        handParent2.gameObject.SetActive(true);
        boardParent.gameObject.SetActive(true);
        deckParent.gameObject.SetActive(false);
    }

    public void SelectCard(CardButton button) {
        selectedCard = button;
        if (clickedCard != null) {
            Card cardComponent = (button == null ? clickedCard : button).cardComponent;
            selectedCardCopy.SetCard(cardComponent.card);
            selectedCardCopy.SetFlipped(cardComponent.flipped);
        } else {
            if (button == null) {
                selectedCardCopy.SetCard(null);
            } else {
                selectedCardCopy.SetCard(button.cardComponent.card);
                selectedCardCopy.SetFlipped(button.cardComponent.flipped);
            }
        }
    }

    public void DeselectCard(CardButton button) {
        if (selectedCard == button)
            SelectCard(null);
    }

    public bool AddCard(CardButton button) {
        if (handSelectionStage) {
            // Hand selection stage: add to hand
            if (handParent.childCount == 5)
                return false; // Reached max
            Vector3 pos = handParent.position;
            pos.y -= cardHeight * handParent.childCount;
            pos.z = -handParent.childCount * 3;
            button.transform.SetParent(handParent);
            button.cardComponent.MoveTo(pos);
            startButton.interactable = handParent.childCount == 5;
            return true;
        } else {
            // Play stage: choose card to put on board
            clickedCard = button;
        }
        return false;
    }

    public bool RemoveCard(CardButton button) {
        if (handSelectionStage) {
            // Hand selection stage: remove from hand
            Vector3 previousPosition = button.transform.position;
            for (int i = button.transform.GetSiblingIndex() + 1; i < handParent.childCount; i++) {
                CardButton button2 = handParent.GetChild(i).GetComponent<CardButton>();
                button2.cardComponent.MoveTo(previousPosition);
                previousPosition = button2.transform.position;
            }
            button.transform.SetParent(deckParent);
            button.cardComponent.MoveTo(button.originalPosition);
            startButton.interactable = false;
        } else {
            // Play stage: choose card to put on board
            clickedCard = button;
        }
        return true;
    }

    public void PlaceCard(Vector3 position, Card card = null) {
        if (clickedCard) {
            card = clickedCard.cardComponent;
            Destroy(clickedCard); // Destroys button component only.
        }
        clickedCard = null;
        position.z = 0;
        card.MoveTo(position);
        card.transform.SetParent(boardParent);
        SelectCard(null);
    }

}
