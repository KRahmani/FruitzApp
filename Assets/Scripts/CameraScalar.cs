using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalar : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float aspectRtio = 0.625f;
    public float padding = 2;
    public float yOffset = 1;


    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }


    void RepositionCamera(float x, float y)
    {
        //pour placer la caméra au milieu du tableau
        //en fonction du nombre de lignes et colonnes
        Vector3 tempPosition = new Vector3(x / 2, y / 2 + yOffset, cameraOffset);
        transform.position = tempPosition;

        //redimensionner la view de la camera
        //en fonction du nombre de lignes et de colonnes
        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRtio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + padding;
        }
    }

}
