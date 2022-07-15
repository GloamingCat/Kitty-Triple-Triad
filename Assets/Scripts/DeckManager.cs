using UnityEngine;

public class DeckManager : MonoBehaviour {

    public static DeckManager Instance { get; private set; }

    [System.Serializable]
    public class Card {
        public string title;
        public Sprite image;
        public int[] values = new int[4];
    }

    [System.Serializable]
    public class DeckEntry {
        public string cardName;
        public int count;
    }
    
    // Deck entries in format of NAME COUNT separated by whitespaces.
    public string defaultPlayerDeck;

    public Card[] cards = new Card[8];

    protected void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public DeckEntry[] ParseDeck(string str) {
        string[] cardIds = str.Split(null);
        DeckEntry[] deck = new DeckEntry[cardIds.Length / 2];
        for (int i = 0; i < deck.Length; i++) {
            DeckEntry entry = new DeckEntry();
            entry.cardName = cardIds[i * 2];
            entry.count = int.Parse(cardIds[i * 2 + 1]);
            deck[i] = entry;
        }
        return deck;
    }

    // Load player's deck from PlayerPrefs.
    public DeckEntry[] GetPlayerDeck() {
        string playerDeck = PlayerPrefs.GetString("Deck", defaultPlayerDeck);
        return ParseDeck(playerDeck);
    }

    // Stores deck in PlayerPrefs.
    public void StorePlayerDeck(DeckEntry[] deck) {
        string deckString = "";
        foreach (DeckEntry entry in deck)
            deckString += entry.cardName + " " + entry.count + " ";
        PlayerPrefs.SetString("Deck", deckString.Trim());
    }

    // Gets the card identified by given name.
    public Card FindCard(string name) {
        foreach (Card card in cards) {
            if (card.image.name.Equals(name))
                return card;
        }
        return null;
    }

    // Add card to deck.
    // If player already has this card, increases card count of the entry.
    // Otherwise, creates new entry.
    public DeckEntry[] AddCard(Card card, DeckEntry[] deck) {
        foreach (DeckEntry entry in deck) {
            if (card.image.name.Equals(entry.cardName)) {
                entry.count++;
                return deck;
            }
        }
        // Create new entry in the deck for the new card
        DeckEntry newEntry = new DeckEntry();
        newEntry.cardName = card.image.name;
        newEntry.count = 1;
        DeckEntry[] newDeck = new DeckEntry[deck.Length + 1];
        deck.CopyTo(newDeck, 0);
        newDeck[deck.Length] = newEntry;
        return newDeck;
    }

}
