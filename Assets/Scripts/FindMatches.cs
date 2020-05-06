using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    //la list ede tous les match qu'on a à un instant t
    public List<GameObject> currentMatches = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    //cette méthode qui appelle la coroutine en bas
    //on crée cette méthode car on va devoir l'appeler en dehors de cette class, ce qu'on ne peut pas faire avec la coroutine
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                if(currentDot != null) // on vérifie si la dot existe
                {
                    //chercher des Matchs Horizontalement
                    if(i > 0 && i < board.width - 1) //on vérifie si sa position fait partie du board
                    {
                        //on crée deux GO qui stockent les dot à gauche et à droite du dot courant
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if(leftDot != null && rightDot != null) // on vérifie s'ils existent
                        {
                            if(leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag) //on verifie s'ils ont les mêmes tags avec le dot courant (ie que les trois sont les mêmes)
                            {
                                if (!currentMatches.Contains(leftDot))
                                {
                                    currentMatches.Add(leftDot);
                                }
                                //on ajoute à la liste des Matchs et on indique que ces trois Dots consécutifs Match (sont pareil)
                                if (!currentMatches.Contains(leftDot))
                                {
                                    currentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(rightDot))
                                {
                                    currentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }

                    //chercher des Match Verticalement
                    if (j > 0 && j < board.height - 1) //on vérifie si sa position fait partie du board
                    {
                        //on crée deux GO qui stockent les dot à gauche et à droite du dot courant
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downtDot = board.allDots[i, j - 1];

                        if (upDot != null && downtDot != null) // on vérifie s'ils existent
                        {
                            if (upDot.tag == currentDot.tag && downtDot.tag == currentDot.tag) //on verifie s'ils ont les mêmes tags avec le dot courant (ie que les trois sont les mêmes)
                            {
                                //on ajoute à la liste des matchs et on indique que ces trois Dots consécutifs Match (sont pareil)
                                if (!currentMatches.Contains(upDot))
                                {
                                    currentMatches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(downtDot))
                                {
                                    currentMatches.Add(downtDot);
                                }
                                downtDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
