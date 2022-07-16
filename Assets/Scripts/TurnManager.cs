using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

    public static TurnManager Instance { get; private set; }
    public static OpponentScriptableObject opponent;

    // Game state
    public Card[] board { get; private set; }
    public bool playerTurn { get; private set; }
    public int currentRound { get; private set; }

    protected int score1, score2;

    protected PlayerAI opponentAI;

    // Scene
    public Text scoreText;
    public Button continueButton;

    protected void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    protected void Start() {
        // Game starts here
        board = new Card[9];
        currentRound = 0;
        playerTurn = true;
        // Get opponent AI
        if (opponent == null)
            opponentAI = null;
        else {
            switch (opponent.aiType) {
            case OpponentScriptableObject.AI.Random:
                opponentAI = new RandomAI();
                break;
            case OpponentScriptableObject.AI.Flip:
                opponentAI = new FlipAI();
                break;
            default:
                opponentAI = null; // Two-player
                break;
            }
        }
        // Let player select their cards.
        if (opponentAI == null) {
            // If no opponent is selected, use a copy of player's deck.
            DeckManager.DeckEntry[] deck = opponent != null ? 
                DeckManager.Instance.ParseDeck(opponent.deck) :
                DeckManager.Instance.GetPlayerDeck();
            CardSelector.Instance.ChooseHand(deck, true);
            scoreText.text = "Select opponent's (player 2) cards";
        } else {
            // Player vs AI.
            CardSelector.Instance.ChooseHand(DeckManager.Instance.GetPlayerDeck(), false);
            scoreText.text = "Select your cards";
        }
    }

    // Button callback when player finishes choosing their cards.
    public void FinishHandSelection() {
        if (playerTurn) { // First player (still need to choose opponent's cards).
            if (opponentAI == null) {
                // Move current hand to opponent hand.
                Card[] opponentHand = CardSelector.Instance.handParent.GetComponentsInChildren<Card>();
                CardSelector.Instance.MoveOpponentHand(opponentHand);
                // Start card selection again, but for second player.
                CardSelector.Instance.ChooseHand(DeckManager.Instance.GetPlayerDeck(), false);
                scoreText.text = "Select player 1's cards";
                playerTurn = false;
                return;
            } else {
                DeckManager.DeckEntry[] deck = DeckManager.Instance.ParseDeck(opponent.deck);
                DeckManager.Card[] opponentHand = opponentAI.ChooseHand(deck);
                CardSelector.Instance.CreateOpponentHand(opponentHand);
            }
        }
        // All hands chosen
        playerTurn = Random.Range(0, 2) == 0; // Coin flip
        currentRound++;
        CardSelector.Instance.StartGame();
        UpdateScores();
        NextTurn();
    }

    // Places a card on the board and flips neighbor cards.
    public void PlaceCard(Card card, int i) {
        board[i] = card;
        foreach (Card flippedCard in FlippedCards(card, i)) {
            flippedCard.SetFlipped(card.flipped);
        }
        UpdateScores();
    }

    public List<Card> FlippedCards(Card card, int i) {
        List<Card> flippedCards = new List<Card>();
        int col = i % 3;
        int row = i / 3;
        if (col > 0 && board[i - 1] != null && board[i - 1].flipped != card.flipped) {
            // Check card on the left
            if (card.card.values[2] > board[i - 1].card.values[0])
                flippedCards.Add(board[i - 1]);
        }
        if (col < 2 && board[i + 1] != null && board[i + 1].flipped != card.flipped) {
            // Check card on the right
            if (card.card.values[0] > board[i + 1].card.values[2])
                flippedCards.Add(board[i + 1]);
        }
        if (row > 0 && board[i - 3] != null && board[i - 3].flipped != card.flipped) {
            // Check card above
            if (card.card.values[1] > board[i - 3].card.values[3])
                flippedCards.Add(board[i - 3]);
        }
        if (row < 2 && board[i + 3] != null && board[i + 3].flipped != card.flipped) {
            // Check card below
            if (card.card.values[3] > board[i + 3].card.values[1])
                flippedCards.Add(board[i + 3]);
        }
        return flippedCards;
    }

    protected void UpdateScores() {
        score1 = CardSelector.Instance.handParent.childCount;
        score2 = CardSelector.Instance.handParent2.childCount;
        foreach (Card card in CardSelector.Instance.boardParent.GetComponentsInChildren<Card>()) {
            if (card.flipped)
                score2++;
            else
                score1++;
        }
    }

    public void NextTurn() {
        // Verify if game ended.
        if (GameEnded()) {
            if (score1 > score2) {
                scoreText.text = opponentAI == null ? "Player 1 wins!\n" : "You win!\n";
            } else if (score1 < score2) {
                scoreText.text = opponentAI == null ? "Player 2 wins!\n" : "You lose.\n";
            } else {
                scoreText.text = "Draw.\n";
            }
            scoreText.text += score1 + " x " + score2;
            continueButton.gameObject.SetActive(true);
            return;
        }
        // Else, continue to next turn.
        playerTurn = !playerTurn;
        if (playerTurn) {
            currentRound++;
            scoreText.text = "Player 1 turn\n";
        } else {
            scoreText.text = "Player 2 turn\n";
        }
        scoreText.text += score1 + " x " + score2;
        // Start next player's turn.
        if (opponentAI != null) {
            if (!playerTurn)
                opponentAI.OpponentTurn();
        } else {
            // Enable buttons for next player.
            foreach (CardButton button in CardSelector.Instance.handParent.GetComponentsInChildren<CardButton>()) {
                button.enabled = playerTurn;
            }
            foreach (CardButton button in CardSelector.Instance.handParent2.GetComponentsInChildren<CardButton>()) {
                button.enabled = !playerTurn;
            }
        }
    }

    // Returns true if game ended.
    protected bool GameEnded() {
        // Verify if one of them won
        for (int i = 0; i < board.Length; i++) {
            if (board[i] == null) {
                return false;
            }
        }
        return true;
    }

    // Button callback
    public void FinishGame() {
        if (opponentAI == null)
            SceneManager.LoadScene("Title");
        else {
            // TODO pick new card
            SceneManager.LoadScene("Title");
        }
    }

}
