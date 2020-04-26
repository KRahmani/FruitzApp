﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public bool isMatched = false;
    public int targetX;
    public int targetY;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    public float swipeAngle = 0;
    private GameObject otherDot;
    private Board board;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
        //sauvegarder les valeurs actuelles de row et column dans previousRow et previousColumn
        previousRow = row;
        previousColumn = column;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //déplacer vers la position cible
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //déplacer vers la position cible
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherDot != null)
        {
            //Si les deux dot qu'on veut échanger ne forme pas trois mêmes dots consécutives, on les remets à leurs places (on ne les échange pas)
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            otherDot = null;
        }
    }

    //la position du premier clique (quand on clique sur la souris)
    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(firstTouchPosition);
    }
    //La dernière position (quand on relache la souris)
    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    //calcler l'angle entre la position du premier clique et celle du dernier clique 
    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x)* 180/Mathf.PI;
        //Debug.Log(swipeAngle);
        MovePieces();
    }

    //calculer où on doit déplacer la pièce sur laquelle on a cliqué
    void MovePieces()
    {
        
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            //Déplacement à droite
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            //Déplacement en haut
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Déplacement à gauche
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Déplacement en bas
            otherDot = board.allDots[column, row-1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }

    //cherche si trois "Dot" consécutives sont les mêmes (ie ont la même couleur)
    void FindMatches()
    {
        //horizontalement
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if(leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
            {
                leftDot1.GetComponent<Dot>().isMatched = true;
                rightDot1.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }
        //Verticalement
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column , row+1];
            GameObject downDot1 = board.allDots[column, row-1];
            if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
            {
                upDot1.GetComponent<Dot>().isMatched = true;
                downDot1.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }
    }
}