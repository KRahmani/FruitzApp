using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    //la liste de tous les match qu'on a à un instant t
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
                                //on vérifié si on trouve une bombe horizontale (de ligne)
                                if(currentDot.GetComponent<Dot>().isRowBomb ||
                                    leftDot.GetComponent<Dot>().isRowBomb ||
                                    rightDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));// on récupère la ligne si c'est le cas
                                }

                                //on vérifié si on trouve une bombe verticale (de colonne)
                                if (currentDot.GetComponent<Dot>().isColumnBomb)
                                    currentMatches.Union(GetColumnPieces(i));// on récupère la colonne si c'est le cas
                                if (leftDot.GetComponent<Dot>().isColumnBomb)
                                    currentMatches.Union(GetColumnPieces(i-1));
                                if (rightDot.GetComponent<Dot>().isColumnBomb)
                                    currentMatches.Union(GetColumnPieces(i+1));



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
                        GameObject downDot = board.allDots[i, j - 1];

                        if (upDot != null && downDot != null) // on vérifie s'ils existent
                        {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag) //on verifie s'ils ont les mêmes tags avec le dot courant (ie que les trois sont les mêmes)
                            {
                                //on vérifié si on trouve une bombe verticale (de colonne)
                                if (currentDot.GetComponent<Dot>().isColumnBomb ||
                                    upDot.GetComponent<Dot>().isColumnBomb ||
                                    downDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));// on récupère la colonne si c'est le cas
                                }
                                //on vérifié si on trouve une bombe horizontale (de ligne)
                                if (currentDot.GetComponent<Dot>().isRowBomb)
                                    currentMatches.Union(GetRowPieces(j));// on récupère la ligne si c'est le cas
                                if (upDot.GetComponent<Dot>().isRowBomb)
                                    currentMatches.Union(GetRowPieces(j + 1));
                                if (downDot.GetComponent<Dot>().isRowBomb)
                                    currentMatches.Union(GetRowPieces(j - 1));


                                //on ajoute à la liste des matchs et on indique que ces trois Dots consécutifs Match (sont pareil)
                                if (!currentMatches.Contains(upDot))
                                {
                                    currentMatches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(downDot))
                                {
                                    currentMatches.Add(downDot);
                                }
                                downDot.GetComponent<Dot>().isMatched = true;
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

    //Récuperer les pièces d'une colonne donnée  
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    //Récuperer les pièces d'une ligne donnée
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                dots.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }
}
