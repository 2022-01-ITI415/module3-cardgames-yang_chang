using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Pyramid : MonoBehaviour
{

    static public Pyramid S;

    [Header("Set in Inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public float reloadDelay = 2f;
    public float xOffset = 3;
    public float yOffset = -2.5f;
    public Vector3 layoutCenter;
    public Vector2 fsPosMid = new Vector2(0.5f, .90f);
    public Vector2 fsPosRun = new Vector2(0.5f, .75f);
    public Vector2 fsPosMid2 = new Vector2(0.4f, 1.0f);
    public Vector2 fsPosEnd = new Vector2(0.5f, .95f);
    [Header("Set Dynamically")]
    public Deck deck;
    public Layout layout;
    public List<CardPyramid> drawPile;
    public Transform layoutAnchor;
    public CardPyramid current;
    public CardPyramid selectedCard_1;
    public CardPyramid selectedCard_2;
    public List<CardPyramid> tableau;
    public List<CardPyramid> temporary;
    public List<CardPyramid> discardPile;
    public FloatingScore fsRun;
    public GameObject button;
    public int shuffleNum = 2;
    private Text highScoreText;
    private Text gameOverText;
    private Text roundResultText;
    void Awake()
    {
        S = this;
        SetUpUITexts();
    }
    void SetUpUITexts()
    {
        GameObject go = GameObject.Find("HighScore");
        if (go != null)
        {
            highScoreText = go.GetComponent<Text>();
        }
        int highScore = ScoreManager.HIGH_SCORE;
        string hScore = "High Score: " + Utils.AddCommasToNumber(highScore);
        go.GetComponent<Text>().text = hScore;
        go = GameObject.Find("GameOver");
        if (go != null)
        {
            gameOverText = go.GetComponent<Text>();
        }

        go = GameObject.Find("RoundResult");
        if (go != null)
        {
            roundResultText = go.GetComponent<Text>();
        }
        ShowResultsUI(false);
    }

    void ShowResultsUI(bool show)
    {
        gameOverText.gameObject.SetActive(show);
        roundResultText.gameObject.SetActive(show);
    }
    void Start()
    {
        ScoreBoard.S.score = ScoreManager.SCORE;
        deck = GetComponent<Deck>();
        deck.InitDeck(deckXML.text);
        Deck.Shuffle(ref deck.cards);
        layout = GetComponent<Layout_>();
        layout.ReadLayout(layoutXML.text);
        drawPile = ConvertListCardsToListCardPyramids(deck.cards);
        LayoutGame();
    }
    List<CardPyramid> ConvertListCardsToListCardPyramids(List<Card> lCD)
    {
        List<CardPyramid> lCP = new List<CardPyramid>();
        CardPyramid tCP;
        foreach (Card tCD in lCD)
        {
            tCP = tCD as CardPyramid;
            lCP.Add(tCP);
        }
        return (lCP);
    }
    CardPyramid Draw()
    {
        CardPyramid cd = drawPile[0];
        drawPile.RemoveAt(0);
        return (cd);
    }
    void LayoutGame()
    {

        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }
        CardPyramid cp;
        foreach (SlotDef tSD in layout.slotDefs)
        {
            cp = Draw();
            cp.faceUp = tSD.faceUp;
            cp.transform.parent = layoutAnchor;
            cp.transform.localPosition = new Vector3
                (layout.multiplier.x * tSD.x,
                layout.multiplier.x * tSD.y,
                -tSD.layerID);
            cp.layoutID = tSD.id;
            cp.slotDef = tSD;
            cp.state = pCardState.tableau;
            cp.SetSortingLayerName(tSD.layerName);
            tableau.Add(cp);
        }
        foreach (CardPyramid tCP in tableau)
        {
            foreach (int hid in tCP.slotDef.hiddenBy)
            {
                cp = FindCardByLayoutID(hid);
                tCP.hiddenBy.Add(cp);
            }
        }
        MoveToTemporaryTableau(Draw());
        //MoveToTarget(Draw());
        UpdateDrawPile();
        SetCardAvailable();
    }
    CardPyramid FindCardByLayoutID(int layoutID)
    {
        foreach (CardPyramid tCP in tableau)
        {
            if (tCP.layoutID == layoutID)
            {
                return (tCP);
            }
        }
        return (null);
    }
    void MoveToDiscard(CardPyramid cd)
    {
        if (cd == current)
        {
            current = null;
            if (temporary.Count > 0)
            {
                MoveToTemporaryTableau(temporary[temporary.Count - 1]);
                temporary.Remove(temporary[temporary.Count - 1]);
            }
        }
        cd.state = pCardState.discard;
        discardPile.Add(cd);
        cd.transform.parent = layoutAnchor;
        cd.transform.localPosition = new Vector3(
        layout.multiplier.x * layout.discardPile.x,
        layout.multiplier.y * layout.discardPile.y,
        -layout.discardPile.layerID + 0.5f);
        cd.faceUp = true;
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(-100 + discardPile.Count);
    }

    void MoveToTemporaryPile(CardPyramid cd)
    {
        cd.state = pCardState.temporary;
        temporary.Add(cd);
        cd.transform.parent = layoutAnchor;
        cd.transform.localPosition = new Vector3(
        layout.multiplier.x* layout.temporaryPile.x,
        layout.multiplier.y* layout.temporaryPile.y,
        -layout.temporaryPile.layerID + 0.5f);
        cd.faceUp = true;
        cd.SetSortingLayerName(layout.temporaryPile.layerName);
        cd.SetSortOrder(-100 + temporary.Count);
    }
    void MoveToTemporaryTableau(CardPyramid cd)
    {
        current = cd;
        cd.state = pCardState.tableau;
        cd.transform.parent = layoutAnchor;
        cd.transform.localPosition = new Vector3(
        layout.multiplier.x * layout.temporaryPile.x,
        layout.multiplier.y * layout.temporaryPile.y,
        -layout.temporaryPile.layerID);

        cd.faceUp = true;
        cd.SetSortingLayerName(layout.temporaryPile.layerName);
        cd.SetSortOrder(0);
    }

    void UpdateDrawPile()
    {
        CardPyramid cd;
        for (int i = 0; i < drawPile.Count; i++)
        {
            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;

            Vector2 dpStagger = layout.drawPile.stagger;
            cd.transform.localPosition = new Vector3(
            layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
            layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
            -layout.drawPile.layerID + 0.1f * i);
            cd.faceUp = false;
            cd.state = pCardState.drawpile;
            cd.SetSortingLayerName(layout.drawPile.layerName);
            cd.SetSortOrder(-10 * i);
            Debug.Log(i);
        }
    }
    public void CardClicked(CardPyramid cd)
    {
        switch (cd.state)
        {
            case pCardState.target:
                break;
            case pCardState.drawpile:
                if (selectedCard_1 != null)
                {
                    SelectedZoomOut(selectedCard_1);
                    selectedCard_1 = null;
                }
                if (current != null)
                {
                    MoveToTemporaryPile(current);
                }              
                MoveToTemporaryTableau(Draw());            
                UpdateDrawPile();
                if (drawPile.Count < 1 && shuffleNum > 0)
                {
                    button.SetActive(true);
                }
                break;
            case pCardState.tableau:
                bool validMatch = false;
                if (cd.rank == 13)
                {
                    validMatch = true;
                    tableau.Remove(cd);
                    MoveToDiscard(cd);
                }
                else if (selectedCard_1 == null)
                {
                    selectedCard_1 = cd;
                    SelectedZoomIn(selectedCard_1);
                }
                else if (cd == selectedCard_1)
                {
                    SelectedZoomOut(selectedCard_1);
                    selectedCard_1 = null;
                }
                else if (AdjacentRank(cd, selectedCard_1))
                {
                    validMatch = true;
                    tableau.Remove(cd);
                    MoveToDiscard(cd);
                    SelectedZoomOut(selectedCard_1);
                    tableau.Remove(selectedCard_1);
                    MoveToDiscard(selectedCard_1);
                    selectedCard_1 = null;
                }
                if (!cd.faceUp)
                {
                    validMatch = false;
                }

                if (!validMatch) return;
                tableau.Remove(cd);
                //MoveToTarget(cd);
                SetCardAvailable();
                //else
                //{
                    //ScoreManager.EVENT(eScoreEvent.mine);
                    //FloatingScoreHandler(eScoreEvent.mine);
                //}
                break;
        }
        //CheckForGameOver();
    }
    void SelectedZoomIn(CardPyramid cd)
    {
        cd.transform.localScale = cd.transform.localScale * 1.25f;
    }
    void SelectedZoomOut(CardPyramid cd)
    {
        cd.transform.localScale = cd.transform.localScale * 0.8f;
    }
    public void Reshuffle()
    {
        MoveToTemporaryPile(current);
        current = null;

        drawPile = temporary;
        temporary = new List<CardPyramid>();

        UpdateDrawPile();
        MoveToTemporaryTableau(Draw());
        Shuffle();

        shuffleNum--;

        button.SetActive(false);
    }

    void Shuffle()
    {
        List<CardPyramid> tCards = new List<CardPyramid>();
        int ndx; 
        while (drawPile.Count > 0)
        {
            ndx = Random.Range(0, drawPile.Count);
            tCards.Add(drawPile[ndx]);
            drawPile.RemoveAt(ndx);
        }
        drawPile = tCards;
    }

    void SetCardAvailable()
    {
        foreach (CardPyramid cd in tableau)
        {
            bool available = true;
            foreach (CardPyramid cover in cd.hiddenBy)
            {
                if (cover.state == pCardState.tableau)
                {
                    available = false;
                }
                if (cover.state == pCardState.unavailable)
                {
                    available = false;
                }
            }
            if (available == true)
            {
                cd.state = pCardState.tableau;
                cd.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                cd.state = pCardState.unavailable;
                cd.GetComponent<SpriteRenderer>().color = Color.gray;
            }
        }
    }
    void SetTableauFaces()
    {
        foreach (CardPyramid cd in tableau)
        {
            bool faceUp = true;
            foreach (CardPyramid cover in cd.hiddenBy)
            {
                if (cover.state == pCardState.tableau)
                {
                    faceUp = false;
                }
            }
            cd.faceUp = faceUp;
        }
    }
    void CheckForGameOver()
    {
        if (tableau.Count == 0)
        {
            GameOver(true);
            return;
        }
        if (drawPile.Count > 0)
        {
            return;
        }
        foreach (CardPyramid cd in tableau)
        {
            if (AdjacentRank(cd, current))
            {
                return;
            }
        }
        GameOver(false);
    }
    void GameOver(bool won)
    {
        int score = ScoreManager.SCORE;
        if (fsRun != null) score += fsRun.score;
        if (won)
        {
            gameOverText.text = "Round Over";
            roundResultText.text = "You won this round!\nRound Score: " + score;
            ShowResultsUI(true);
            ScoreManager.EVENT(eScoreEvent.gameWin);
            FloatingScoreHandler(eScoreEvent.gameWin);
        }
        else
        {
            gameOverText.text = "Game Over";
            if (ScoreManager.HIGH_SCORE <= score)
            {
                string str = "You got the high score!\nHigh score: " + score;
                roundResultText.text = str;
            }
            else
            {
                roundResultText.text = "Your final score was: " + score;
            }
            ShowResultsUI(true);
            ScoreManager.EVENT(eScoreEvent.gameLoss);
            FloatingScoreHandler(eScoreEvent.gameLoss);
        }
        //SceneManager.LoadScene("_pyramidScene_0");
        Invoke("ReloadLevel", reloadDelay);
    }
    void ReloadLevel()
    {
        SceneManager.LoadScene("__pyramidScene_0");
    }
    public bool AdjacentRank(CardPyramid c0, CardPyramid c1)
    {
        if (!c0.faceUp || !c1.faceUp) return (false);
        if (c0.rank + c1.rank == 13)
        {
            return (true);
        }
        return (false);
    }
    void FloatingScoreHandler(eScoreEvent evt)
    {
        List<Vector2> fsPts;
        switch (evt)
        {
            case eScoreEvent.draw:
            case eScoreEvent.gameWin:
            case eScoreEvent.gameLoss:
                if (fsRun != null)
                {
                    fsPts = new List<Vector2>();
                    fsPts.Add(fsPosRun);
                    fsPts.Add(fsPosMid2);
                    fsPts.Add(fsPosMid2);
                    fsPts.Add(fsPosEnd);
                    fsRun.reportFinishTo = ScoreBoard.S.gameObject;
                    fsRun.Init(fsPts, 0, 1);
                    fsRun.fontSizes = new List<float>(new float[] { 28, 36, 4 });
                    fsRun = null;
                }
                break;

            case eScoreEvent.mine:
                FloatingScore fs;
                Vector2 p0 = Input.mousePosition;
                p0.x /= Screen.width;
                p0.y /= Screen.height;
                fsPts = new List<Vector2>();
                fsPts.Add(p0);
                fsPts.Add(fsPosMid);
                fsPts.Add(fsPosRun);
                fs = ScoreBoard.S.CreateFloatingScore(ScoreManager.CHAIN, fsPts); //ScoreBoard
                fs.fontSizes = new List<float>(new float[] { 4, 50, 28 });
                if (fsRun == null)
                {
                    fsRun = fs;
                    fsRun.reportFinishTo = null;
                }
                else
                {
                    fs.reportFinishTo = fsRun.gameObject;
                }
                break;
        }
    }
}