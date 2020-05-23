﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}
//utilisé pour le type de pièces
//pour remplir le tableau
public enum TileKind
{
    Breakable,
    Blank,
    Normal

}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;

}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width, height;
    public int offset;//sert à avoir un effet glissant lorsque les fruits tombent
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;
    public TileType[] boardLayout;

    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;

    void Start()
    {
        breakableTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    //pour stocker toutes les positions où il doit y avoir de l'éspace libre
    public void GenerateBlankSpaces()
    {
        for(int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }

    }

    //pour créer des pièces de glace
    public void GenerateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                //créer la pièce de glace sur cette position
                //Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                Vector3 tempPosition = new Vector3(boardLayout[i].x, boardLayout[i].y, 0.00001f);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();

            }
        }

    }

    private void SetUp()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j]) // s'il ne doit pas y avoir de l'espce libre, alors on crée la piéce
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "( " + i + ", " + j + " )";
                    int dotToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0; //pour éviter une boucle infinite
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100) //pour éviter d'avoir des matchs quand on commence le jeu
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                        Debug.Log(maxIterations);
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                }
            }

    }

    private bool MatchesAt(int column, int row, GameObject piece) 
    {
        if(column>1 && row > 1)
        {
            if(allDots[column-1,row]!=null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag &&
                    allDots[column - 2, row].tag == piece.tag) return true;
            }
            if (allDots[column, row-1] != null && allDots[column, row-2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag &&
                    allDots[column, row - 2].tag == piece.tag) return true;
            }

        }
        else if(column<=1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag &&
                        allDots[column, row - 2].tag == piece.tag) return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag &&
                        allDots[column - 2, row].tag == piece.tag) return true;
                }
            }
        }
        return false;
    }
    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;

        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
        {
            foreach (GameObject currentPiece in findMatches.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.row == firstPiece.row)
                {
                    numberHorizontal++;
                }
                if (dot.column == firstPiece.column)
                {
                    numberVertical++;
                }

            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);

    }


    private void CheckToMakeBombs()
    {
        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {
            findMatches.CheckBombs();
        }
        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                //s'il s'agit d'une collone ou d'une ligne, on crée une colorBomb
                if (currentDot != null)
                {
                    //vérifier si la pièce courante crée un match
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
                
            }
            else
            {
                //sinon on crée une adjacentBomb
                if (currentDot != null)
                {
                    //vérifier si la pièce courante crée un match
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isAdjacentBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isAdjacentBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }

            }
        }
    }


    private void DistroyMatchesAt(int column,int row) //detruire les matchs à une position donnée
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //Vérifier combien de pièces construisant le match
            if(findMatches.currentMatches.Count >=4)
            {
                CheckToMakeBombs();
            }
            //si c'est une pièce de glace, il faut essayer de la casser
            if (breakableTiles[column, row] != null)
            {
                //nombre d'essais pour casser la pièce ++
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }

            //créer animation de destruction
            GameObject particle=Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);

            //détruire l'objet après un certain temps pour ne pas occuper de la mémoire
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }

    }

    //vérifier chaque pièce du tableau, pour savoir si elle doit ou non être détruite 
    public void DistroyMatches()
    {
        for(int i = 0; i < width; i++)
            for(int j=0;j<height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DistroyMatchesAt(i, j);
                }
            }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2()); // après avoir dtruit les pièces du match, on fait les pièces restantes descendre
        
    }
    //faire descendre les pièces quand il y a des matchs
    //tenant compte des éspaces libres
    private IEnumerator DecreaseRowCo2()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //si la position doit contenir une pièce et qu'elle est vide 
                if (!blankSpaces[i, j] && allDots[i, j] == null)
                {
                    //on parcourt toutes les position entre cette place vide et le top de la colonne
                    for (int k = j + 1; k < height; k++)
                    {
                        //si on tombe sur une pièce
                        if (allDots[i, k] != null)
                        {
                            //on déplace cette pièce sur la position vide
                            allDots[i, k].GetComponent<Dot>().row = j;
                            //et on vide la position de la pièce
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }

            }
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private IEnumerator DecreaseRowco() //pour faire les pièces descendre quand il y a des matchs
    {
        int nullCount = 0; //variable qui indique le nombre de pièces manquantes pour chaque colonne 
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                    nullCount++;
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f); //durée de la descente des pièces
        StartCoroutine(FillBoardCo());
    }





    private void RefillBoard() 
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    int dotToUse = Random.Range(0, dots.Length);
                    //créer une nouvelle pièce sur la place vide
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;

                }
            }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                    if (allDots[i, j].GetComponent<Dot>().isMatched) return true;
            }
        return false;
    }

    private IEnumerator FillBoardCo() //remplir le tableau, après avoir detruir les piéces créant des matchs
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f); //temps de remplissage du tableau 

        while(MatchesOnBoard()) // tant qu'on trouve des matchs après le remplissage
        {
            yield return new WaitForSeconds(.5f);
            DistroyMatches(); //on élimine les piéces qui créent  des nouveaux matchs 
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.5f);

        if (IsDeadlocked())
        {
            //Debug.Log("Bloqué");
            ShuffleBoard();
        }
        currentState = GameState.move;
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        //retenir la deuxième pièce 
        GameObject holder = allDots[column + (int)direction.x, row+ (int)direction.y] as GameObject;
        //échanger la premièere et la deuxième pièce
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        allDots[column, row] = holder;

    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    //vérifier si on ne sort pas par la droite du tableau
                    if (i < width- 2 )
                    {
                        //vérifier s'il existe un match à droit
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag &&
                               allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    //vérifier si on ne sort pas par le haut du tableau
                    if (j < height - 2)
                    {
                        //vérifier s'il existe un match au dessus
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag &&
                                allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    //vérifier si le tableau n'est pas bloqué (pas de matchs)
    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    //vérifier si on peut aller à droite sans sortir du tableau
                    if (i < width - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.right)) //si o trouve des matches
                        {
                            return false; // le tableau n'est pas bloqué
                        }
                    }
                    //vérifier si on peut aller en haut sans dépasser
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
                return true;
    }

    //mélanger les pièces en cs de blocage
    private void ShuffleBoard()
    {
        //mettre toutes les pièces dans une nouvelle liste
        List<GameObject> newBoard = new List<GameObject>();
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }

        //pour chaque position du tableau
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //si la pièce n'est pas un espace libre
                if (!blankSpaces[i, j])
                {
                    //Choisir une pièce de la liste au hazard 
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    

                    int maxIterations = 0; //pour éviter une boucle infinite
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100) 
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                        Debug.Log(maxIterations);
                    }
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIterations = 0;


                    piece.column = i;
                    piece.row = j;
                    allDots[i, j] = newBoard[pieceToUse];

                    //enlever la pièce de la liste après l'avoir choisie
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }

        //vérifier s'il y a toujours un blocage au niveau du tableau
        if (IsDeadlocked())
        {
            //mélanger le tableau tant qu'on trouve aucun match
            ShuffleBoard();
        }




    }
}
