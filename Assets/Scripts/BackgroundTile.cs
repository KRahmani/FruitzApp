using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer sprite; //besoin pour changer l'aspec de la pièce de glace
                                   //quand on essaie de la casser

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if (hitPoints <= 0)
        {
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
