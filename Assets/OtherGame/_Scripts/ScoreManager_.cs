using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum pScoreEvent 
{ 
    draw,
    mine,
    gameWin,
    gameLoss
}
public class ScoreManager_ : MonoBehaviour
{
    static private ScoreManager_ S;
    static public int SCORE_FROM_PREV_ROUND = 0;
    static public int HIGH_SCORE = 0;
    [Header("Set Dynamically")]
    public int chain = 0;
    public int scoreRun = 0;
    public int score = 0;

    void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("ERROR:ScoreManager_.Awake():S is already set!");
        }
        if (PlayerPrefs.HasKey("PyramidHighScore"))
        {
            HIGH_SCORE = PlayerPrefs.GetInt("PyramidHighScore");
        }
        score += SCORE_FROM_PREV_ROUND;
        SCORE_FROM_PREV_ROUND = 0;
    }

    static public void EVENT(pScoreEvent evt)
    {
        try
        {
            S.Event(evt);
        }
        catch (System .NullReferenceException nre)
        {
            Debug.LogError ("ScoreManager_:EVENT() called while S = null. \n" +nre);
        }
    }
    void Event(pScoreEvent evt)
    {
        switch (evt)
        {
            case pScoreEvent.draw:
            case pScoreEvent.gameWin:
            case pScoreEvent.gameLoss:
                chain = 0;
                score += scoreRun;
                scoreRun = 0;
                break;

            case pScoreEvent.mine:
                chain++;
                scoreRun += chain;
                break;
        }


        switch (evt)
        {
            case pScoreEvent.gameWin:
                SCORE_FROM_PREV_ROUND = score;
                print("You won this round! Roundscore:" + score);
                break;

            case pScoreEvent.gameLoss:
                if (HIGH_SCORE <= score)
                {
                    print("You got the highscore! Highscore:" + score);
                    HIGH_SCORE = score;
                    PlayerPrefs.SetInt("PyramidHighScore", score);
                }
                else
                {
                    print("Your final score for the game was:" + score);
                }
                break;
            default:
                print("score:" + score + "scoreRun:" + scoreRun + "chain:" + chain);
                break;
        }
    }
    static public int CHAIN { get { return S.chain; } }
    static public int SCORE { get { return S.score; } }
    static public int SCORE_RUN { get { return S.scoreRun; } }
}