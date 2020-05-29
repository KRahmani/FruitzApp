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

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.column, dot1.row));
        }

        if (dot2.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        }

        if (dot3.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.row));
        }

        if (dot2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
        }

        if (dot3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.column));
        }

        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
        }

        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
        }
        return currentDots;
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];

                if (currentDot != null) // on vérifie si la dot existe
                {
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    //chercher des Matchs Horizontalement
                    if (i > 0 && i < board.width - 1) //on vérifie si sa position fait partie du board
                    {
                        //on crée deux GO qui stockent les dot à gauche et à droite du dot courant
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null)
                        {
                            Dot rightDotDot = rightDot.GetComponent<Dot>();
                            Dot leftDotDot = leftDot.GetComponent<Dot>();

                            if (leftDot != null && rightDot != null) // on vérifie s'ils existent
                            {
                                if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag) //on verifie s'ils ont les mêmes tags avec le dot courant (ie que les trois sont les mêmes)
                                {
                                    //on vérifié si on trouve une bombe horizontale (de ligne)
                                    currentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot));

                                    //on vérifié si on trouve une bombe verticale (de colonne)
                                    currentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot));

                                    currentMatches.Union(IsAdjacentBomb(leftDotDot, currentDotDot, rightDotDot));

                                    GetNearbyPieces(leftDot, currentDot, rightDot);

                                }
                            }
                        }
                    }

                    //chercher des Match Verticalement
                    if (j > 0 && j < board.height - 1) //on vérifie si sa position fait partie du board
                    {
                        //on crée deux GO qui stockent les dot à gauche et à droite du dot courant
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];
                        if (upDot != null && downDot != null)
                        {
                            Dot downDotDot = downDot.GetComponent<Dot>();
                            Dot upDotDot = upDot.GetComponent<Dot>();

                            if (upDot != null && downDot != null) // on vérifie s'ils existent
                            {
                                if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag) //on verifie s'ils ont les mêmes tags avec le dot courant (ie que les trois sont les mêmes)
                                {
                                    //on vérifié si on trouve une bombe verticale (de colonne)
                                    currentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot));
                                    //on vérifié si on trouve une bombe horizontale (de ligne)
                                    currentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot));

                                    currentMatches.Union(IsAdjacentBomb(upDotDot, currentDotDot, downDotDot));
                                    //on ajoute à la liste des matchs et on indique que ces trois Dots consécutifs Match (sont pareil)
                                    GetNearbyPieces(upDot, currentDot, downDot);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                //si la pièce existe das notre board
                if (board.allDots[i, j] != null)
                {
                    //si le tag de la pièce est le même que celui passé en paramètre
                    if (board.allDots[i, j].tag == color)
                    {
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }
    //Retourner toutes les pièces qui entourent (adjacentes) la pièce courante
    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                //vérifier que les pièces sont dans le board
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if(board.allDots[i,j] != null)
                    {
                        dots.Add(board.allDots[i, j]);
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                   
                }
            }
        }
        return dots;
    }


    //Récuperer les pièces d'une colonne donnée  
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                Dot dot = board.allDots[column, i].GetComponent<Dot>();
                if (dot.isRowBomb)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }
                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
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
                Dot dot = board.allDots[i, row].GetComponent<Dot>();
                if (dot.isColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }
                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        }
        return dots;
    }
    //Vérifie si on doit générer une bomb
    public void CheckBombs()
    {
        //si le joueur est entrain de déplacer une pièce
        if (board.currentDot != null)
        {
            //est ce que la pièce qu'il déplace constitue un match
            if (board.currentDot.isMatched)
            {
                board.currentDot.isMatched = false;
                //quelle type de bomb générer
                /*int typeOfBomb = Random.Range(0, 100);
                if(typeOfBomb < 50)
                {
                    //créer une bomb pour ligne
                    board.currentDot.MakeRowBomb();
                }
                else if(typeOfBomb >= 50)
                {
                    //créer une bomb pour colonne
                    board.currentDot.MakeColumnBomb();
                }*/
                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) ||
                    (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
            //si l'autre Dot match
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    otherDot.isMatched = false;
                    //quelle type de bomb générer
                    /*int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50)
                    {
                        //créer une bomb pour ligne
                        otherDot.MakeRowBomb();
                    }
                    else if (typeOfBomb >= 50)
                    {
                        //créer une bomb pour colonne
                        otherDot.MakeColumnBomb();
                    }*/
                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) ||
                      (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }

                }
            }
        }

    }
}
