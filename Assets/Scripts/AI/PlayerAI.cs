using System;
using System.Collections.Generic;

public abstract class PlayerAI {

    // Interface for opponent AI.

    // Selects 5 cards from deck.
    public abstract DeckManager.Card[] ChooseHand(DeckManager.DeckEntry[] deck);

    // Choose card (position of the card in their hand) and position on board.

    protected abstract Tuple<Card, BoardSlot> Play(List<Card> hand, List<BoardSlot> slots);

    public void OpponentTurn() {
        Card[] cards = CardSelector.Instance.handParent2.GetComponentsInChildren<Card>();
        List<Card> hand = new List<Card>();
        foreach (Card card in cards) {
            if (card.flipped)
                hand.Add(card);
        }
        BoardSlot[] slots = UnityEngine.Object.FindObjectsOfType<BoardSlot>();
        List<BoardSlot> emptySlots = new List<BoardSlot>();
        foreach (BoardSlot slot in slots) {
            if (TurnManager.Instance.board[slot.pos] == null)
                emptySlots.Add(slot);
        }
        if (emptySlots.Count == 0)
            return;
        Tuple<Card, BoardSlot> selection = Play(hand, emptySlots);
        CardSelector.Instance.PlaceCard(selection.Item2.transform.position, selection.Item1);
        TurnManager.Instance.PlaceCard(selection.Item1, selection.Item2.pos);
        TurnManager.Instance.NextTurn();
        UnityEngine.Object.Destroy(selection.Item2);
    }

}
