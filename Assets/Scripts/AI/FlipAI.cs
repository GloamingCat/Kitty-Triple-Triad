using System;
using System.Collections.Generic;

public class FlipAI : PlayerAI {

    public override DeckManager.Card[] ChooseHand(DeckManager.DeckEntry[] deck) {
        // Chooses cards with higher sum of numbers.
        int i = 0;
        DeckManager.Card[] hand = new DeckManager.Card[5];
        while (i < 5) {
            int bestSum = -1;
            DeckManager.DeckEntry bestCard = null;
            foreach (DeckManager.DeckEntry entry in deck) {
                if (entry.count == 0)
                    continue;
                DeckManager.Card card = DeckManager.Instance.FindCard(entry.cardName);
                int sum = card.values[0] + card.values[1] + card.values[2] + card.values[3];
                if (sum > bestSum) {
                    bestSum = sum;
                    bestCard = entry;
                }
            }
            hand[i] = DeckManager.Instance.FindCard(bestCard.cardName);
            i++;
            bestCard.count--;
        }
        return hand;
    }

    protected override Tuple<Card, BoardSlot> Play(List<Card> hand, List<BoardSlot> slots) {
        Card bestCard = null;
        BoardSlot bestSlot = null;
        int bestFlipCount = -1;
        foreach (Card card in hand) {
            foreach (BoardSlot slot in slots) {
                int flipCount = TurnManager.Instance.FlippedCards(card, slot.pos).Count;
                if (flipCount > bestFlipCount) {
                    bestFlipCount = flipCount;
                    bestCard = card;
                    bestSlot = slot;
                }
            }
        }
        return new Tuple<Card, BoardSlot>(bestCard, bestSlot);
    }

}
