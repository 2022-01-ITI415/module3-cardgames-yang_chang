using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.UI;
public class ScoreBoard_ : MonoBehaviour { 
    public static ScoreBoard_ S; 
   
    [Header("Set in Inspector")] 
    public GameObject  prefabFloatingScore_; 
   
    [Header("Set Dynamically")] 
    [SerializeField] private int _score = 0; 
    [SerializeField] private string _scoreString; 
  
    private Transform  canvasTrans; 
    public int score { 
        get { 
            return(_score); 
        } 
        set { 
            _score = value;
            scoreString = Utils.AddCommasToNumber(_score);
        } 
    } 
    public string scoreString { 
        get { 
            return(_scoreString); 
            } 
        set { 
            _scoreString = value; 
            GetComponent<Text>().text = _scoreString; 
        } 
    } 
   
    void Awake() { 
        if (S == null) { 
            S = this; 
        } else { 
            Debug.LogError("ERROR: Scoreboard.Awake(): S is already set!"); 
        } 
        canvasTrans = transform.parent; 
    } 
    public void FSCallback(FloatingScore_ fs) { 
        score = ScoreManager_.SCORE; 
    } 
    public FloatingScore_ CreateFloatingScore(int amt, List<Vector2> pts) { 
        GameObject go = Instantiate <GameObject> (prefabFloatingScore_); 
        go.transform.SetParent( canvasTrans ); 
        FloatingScore_ fs = go.GetComponent<FloatingScore_>(); 
        fs.score = amt; 
        fs.reportFinishTo = this.gameObject; 
        fs.Init(pts); 
        return(fs); 
    }  
 }