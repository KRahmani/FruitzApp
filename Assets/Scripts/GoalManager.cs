using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // nécessaire pour exposer les variables publiques dans unity
                      // parce qu'on crée un classe
public class BlankGoal // conteneur des données 
{
    public int numberNeeded; //le nombre de pièces qu'on veut atteindre
    
    public Sprite goalSprite;
    public string matchValue; //message qui va afficher si la but a été atteint ou pas

    public int numberCollected; //le nombre de pièces obtenus

}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals; //on peut avoir 1 ou plusieurs buts en fonction de chaque niveau
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject GoalGameParent;

    private EndGameManager endGame;
    private Board board;


    void Start()
    {
        
        endGame = FindObjectOfType<EndGameManager>();
        board = FindObjectOfType<Board>();

        GetGoals(); //récupérér et mettre les buts des niveaux
        SetUpGoals();

    }

    void GetGoals()
    {
        if(board!=null)
        {
            if (board.world != null)
            { 
                if (board.level < board.world.levels.Length)
                {
                    if (board.world.levels[board.level] != null)
                    {
                        levelGoals = board.world.levels[board.level].levelGoals;
                        for(int i = 0; i < levelGoals.Length; i++)
                        {
                            board.world.levels[board.level].levelGoals[i].numberCollected = 0;
                            levelGoals[i].numberCollected = 0;
                        }
                    }
                }
            }
                
        }
    }

    void SetUpGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            //initialiser le panneau de buts du jeu
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform,false);

            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;


            //initialiser les buts qui apparaissent dans le jeu en haut à droite
            GameObject gameGoal= Instantiate(goalPrefab, GoalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(GoalGameParent.transform, false);

            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;

        }
    }

    public void UpdateGoals()
    { // mettre à jour les texts affichant les buts à atteindre
        int goalsCompleted = 0;
        for(int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            if(levelGoals[i].numberCollected>= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }
        if (goalsCompleted >= levelGoals.Length)
        {
            if (endGame != null)
            {
                endGame.WinGame();
                //Debug.Log("Partie gagnée");

                board.level++;
                if (board.level < board.world.levels.Length)
                {
                    board.Awake();
                }



            } 
        }

    }

    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }
}
