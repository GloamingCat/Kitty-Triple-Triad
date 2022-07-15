using System;
using System.Collections.Generic;

public class RandomAI : PlayerAI {

    public override DeckManager.Card[] ChooseHand(DeckManager.DeckEntry[] deck) {
        int i = 0;
        DeckManager.Card[] hand = new DeckManager.Card[5];
        while (i < 5) {
            DeckManager.DeckEntry entry = deck[UnityEngine.Random.Range(0, deck.Length)];
            if (entry.count == 0)
                continue;
            DeckManager.Card card = DeckManager.Instance.FindCard(entry.cardName);
            hand[i] = card;
            i++;
            entry.count--;
        }
        return hand;
    }

    protected override Tuple<Card, BoardSlot> Play(List<Card> hand, List<BoardSlot> slots) {
        int card = UnityEngine.Random.Range(0, hand.Count);
        int slot = UnityEngine.Random.Range(0, slots.Count);
        return new Tuple<Card, BoardSlot>(hand[card], slots[slot]);
    }

}
