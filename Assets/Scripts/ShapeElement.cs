using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeElement : MonoBehaviour
{
    public bool[] connection;
    private int rotation = 90;

    public LevelManager levelManager;

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
        if (!levelManager.LevelCompleted()) RotateShapes(.15f);
    }

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
