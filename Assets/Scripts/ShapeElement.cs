using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeElement : MonoBehaviour
{
    public bool[] connection;

    public LevelManager levelManager;

    private int rotation = 90;


    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }
    // Start is called before the first frame update
    void Start()
    {
        levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (!levelManager.LevelCompleted() && !levelManager.RestartScreen_1.activeSelf)
        {
            if (transform.rotation.eulerAngles.z % 90 == 0)
            {
                //NOTE: Substracting the steps here for the stepped game mode
                if (levelManager.modeCheck == 2) --levelManager.steps;
            }
                RotateShapes(.15f);
        }


    }


    //NOTE: This is to allign the connection status of the objects with the rotation
    public void AllignConnections()
    {
        bool temp = connection[0];

        for (int i = 0; i < connection.Length - 1; i++)
        {
            connection[i] = connection[i + 1];
        }

        connection[3] = temp;

    }

    public void RotateShapes(float rotationSpeed)
    {
        if (transform.rotation.eulerAngles.z % 90 == 0)
        {
            LeanTween.rotateAround(gameObject, Vector3.forward, rotation, rotationSpeed);
            AllignConnections();
        }

        levelManager.puzzle.currentCondition = levelManager.GetCurrentCondition();
    }
}
