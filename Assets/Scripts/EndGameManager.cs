using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum GameType
{
    Moves,
    Time

}

[System.Serializable] // nécessaire pour exposer les variables publiques dans unity
                      // parce qu'on crée un classe
public class EndGameRequirements
{
    public GameType gameType;//type qui permet de choisir entre temps ou nombre de mouvements
                             //comme condition d'arret du jeu

    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{

    public GameObject movesLabel;
    public GameObject timeLabel;
    public Text counter;
    public int currentCounterValue;
    public EndGameRequirements requirements;

    public GameObject winPanel;
    public GameObject losePanel;

    private Board board;
    private float timerSeconds;


    void Start()
    {
        board = FindObjectOfType<Board>();
        SetGameType();
        SetUpGame();

    }

    //ajuster le jeu en fonction du niveau
    void SetGameType()
    {
        if(board.world!=null) 
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    requirements = board.world.levels[board.level].endGameRequirements;

                }
            }

            
        }
    }

    public void SetUpGame()
    {
        currentCounterValue = requirements.counterValue;
        if (requirements.gameType == GameType.Moves) //si on a des mouvements 
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1; 
            movesLabel.SetActive(false); //si on a le temps
            timeLabel.SetActive(true);
        }
        counter.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause) //attendre que le jeu commence (important pour le temps)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }

        }


    }

    public void WinGame()
    {
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        winPanel.SetActive(true);
        board.currentState = GameState.win;

    }

    public void LoseGame()
    {
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        losePanel.SetActive(true); // afficher le paneau de perte
        board.currentState = GameState.lose;
        //Debug.Log("Perdu!!!!");


    }


    void Update()
    {
        if (requirements.gameType == GameType.Time && currentCounterValue>0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
