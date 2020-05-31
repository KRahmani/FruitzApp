using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer sprite; //besoin pour changer l'aspec de la pièce de glace
                                   //quand on essaie de la casser

    private GoalManager goalManager;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        goalManager = FindObjectOfType<GoalManager>();
    }

    public void Update()
    {
        if (hitPoints <= 0)
        {
            if (goalManager != null)
            {
                //faire en sorte que les pièces de glace soient aussi prise en compte dans le but du nineau
                goalManager.CompareGoal(this.gameObject.tag);
                goalManager.UpdateGoals();
            }
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();

    }

    void MakeLighter()
    {
        //prendre la couleur courante de la pièce de glace
        Color color = sprite.color;
        //reduire sa visibilié
        float newAlpha = color.a * .5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }


}
