using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoryGame : MonoBehaviour
{
    [SerializeField]
    public int sizeX;
    [SerializeField]
    public int sizeY;

    [SerializeField]
    List<Sprite> availableSprites;
    List<Material> cardFaces;

    [SerializeField]
    List<Card> cards;
    
    int[] board;

    [SerializeField]
    private GameObject cardPrefab;

    public static MemoryGame Instance = null;

    private Card selection1, selection2;

    bool canSelect = true;

    [SerializeField]
    TMP_Text accuracy;

    int score;
    int attempts;
    int pairsLeft;

    public int GetPairs()
    {
        return sizeX * sizeY / 2;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }
    void Start()
    {
        if (availableSprites.Count < GetPairs())
        {
            Debug.LogError("Not Enough Available Sprites for Board Size");
        }
        else
        {
            cardFaces = new List<Material>();
            foreach (Sprite sprite in availableSprites)
            {
                // Create a new Material instance
                Material newMaterial = new Material(Shader.Find("Sprites/Default"));

                // Set the main texture of the material to the sprite's texture
                newMaterial.mainTexture = sprite.texture;

                // Set the material's texture wrap mode to match the sprite's settings
                newMaterial.mainTexture.wrapMode = sprite.texture.wrapMode;

                cardFaces.Add(newMaterial);
            }

        }
        NewRound();

    }

    void NewRound()
    {
        pairsLeft = GetPairs();
        foreach (Transform childTransform in transform)
        {
            GameObject child = childTransform.gameObject;
            Destroy(child);
        }
        cards.Clear();

        board = new int[sizeX * sizeY];

        // return the total number of pairs, i.e if there are 10 face-down cards there are 5 pairs
        int pairs = GetPairs();

        // insert pairs into the board  [0, 0, 1, 1, 2, 2, ... n]
        int currentPair = 0;
        for (int i = 0; i < board.Length; i += 2)
        {
            board[i] = currentPair;
            board[i + 1] = currentPair++;
        }

        // Shuffle the board    [1, 0, 1, n, 2, n-1, ... ?]
        for (int i = 0; i < board.Length; i++)
        {
            int rng = Random.Range(0, board.Length - 1);
            int tmp = board[i];
            board[i] = board[rng];
            board[rng] = tmp;
        }

        string boardAsString = string.Join(", ", board);
        // Debug.Log(boardAsString);

        MakeBoard();

        StartCoroutine(FlipAllCards());
    }


    void MakeBoard()
    {
        cards = new List<Card>();
        float buffer = 0.1f;
        float cardHalfWidth = 0.5f;
        Vector2 offset = new Vector2((sizeX * 0.5f) - cardHalfWidth + ((sizeX - 1) * buffer * 0.5f),
            -(sizeY * 0.5f) + cardHalfWidth - ((sizeY - 1) * buffer * 0.5f)) * -1f;



        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                GameObject g = Instantiate(cardPrefab, new Vector2(x + x * buffer, -y - y * buffer) + offset, Quaternion.Euler(0f, 180f, 0f), transform);
                Card card = g.GetComponent<Card>();
                card.pairNumber = board[(y*sizeX) + x];
                card.SetFace(cardFaces[card.pairNumber]);
                cards.Add(card);
                g.SetActive(false);
            }
        }
    }

    public void SelectCard(Card card)
    {
        if (!canSelect) return;

        if (selection1 == null)
        {
            selection1 = card;
            selection1.FlipToFront();
        }
        else if (selection2 == null && card != selection1)
        {
            selection2 = card;
            selection2.FlipToFront();
            canSelect = false;
            if (selection1.pairNumber == selection2.pairNumber)
            {
                score++;
                pairsLeft--;
                StartCoroutine(ResetCards(true));
            }
            else
            {
                StartCoroutine(ResetCards(false));
            }
            attempts++;
            float newAcc = (float)score / (float)attempts * 100f;
            accuracy.text = string.Format("Accuracy: {0}% ({1} of {2} guesses)", newAcc, score, attempts);

        }
    }

    IEnumerator FlipAllCards()
    {
        yield return new WaitForSeconds(1f);
        canSelect = false;
        foreach (Card card in cards)
        {
            card.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            card.FlipToFront();
        }

        yield return new WaitForSeconds(2f);
        foreach (Card card in cards)
        {
            card.FlipToBack();
        }
        yield return new WaitForSeconds(0.5f);
        canSelect = true;
    }

    IEnumerator ResetCards(bool isPaired)
    {
        yield return new WaitForSeconds(0.5f);
        if (isPaired)
        {
            selection1.Hide();
            selection2.Hide();
            if (pairsLeft <= 0)
            {
                NewRound();
            }
        }

        selection1.FlipToBack();
        selection2.FlipToBack();

        yield return new WaitForSeconds(0.5f);

        selection1 = null;
        selection2 = null;

        canSelect = true;


    }




}
