﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width, height;
    public int offset;//sert à avoir un effet glissant lorsque les fruits tombent
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;

    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        SetUp();

    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offset);
                GameObject backgroundTile=Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";
                int dotToUse = Random.Range(0, dots.Length);

                int maxIterations = 0; //pour éviter une boucle infinite
                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations<100) //pour éviter d'avoir des matchs quand on commence le jeu
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

    private bool MatchesAt(int column, int row, GameObject piece) 
    {
        if(column>1 && row > 1)
        {
            if(allDots[column - 1, row].tag == piece.tag &&
               allDots[column - 2, row].tag == piece.tag) return true;
            if (allDots[column, row-1].tag == piece.tag &&
                allDots[column, row-2].tag == piece.tag) return true;
        }
        else if(column<=1 || row <= 1)
        {
            if (row > 1)
                if (allDots[column, row - 1].tag == piece.tag &&
                   allDots[column, row - 2].tag == piece.tag) return true;
            if (column > 1)
                if (allDots[column-1, row].tag == piece.tag &&
                   allDots[column-2, row].tag == piece.tag) return true;
        }
        return false;
    }

    private void DistroyMatchesAt(int column,int row) //detruire les matchs à une position donnée
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //Vérifier combien de pièces construisant le match
            if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            {
                findMatches.CheckBombs();
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
        StartCoroutine(DecreaseRowco()); // après avoir dtruit les pièces du match, on fait les pièces restantes descendre
        
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
                if (allDots[i, j] == null)
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
        currentState = GameState.move;
    }
}
