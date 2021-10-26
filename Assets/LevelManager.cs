using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{


    [System.Serializable]
    public class Puzzle
    {
        public int winCondition;
        public int currentCondition;


        public int width;
        public int height;
        public ShapeElement[,] shapes; 
    }

    public Puzzle puzzle;

    public bool generate = true;

    private GameObject foreground;

    private int levelNumber = 0;

    private void Awake()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(LoadLevel_1());
        //levelNumber = 1;

        //Vector2 dimensions = Dimensions();

        //puzzle.width = (int)dimensions.x;
        //puzzle.height = (int)dimensions.y;

        //puzzle.shapes = new ShapeElement[puzzle.width, puzzle.height];

        //foreach (var shapeElement in GameObject.FindGameObjectsWithTag("Shapes"))
        //{
        //    puzzle.shapes[(int)shapeElement.transform.position.x, (int)shapeElement.transform.position.y] = shapeElement.GetComponent<ShapeElement>();
        //}

        //foreach (var item in puzzle.shapes)
        //{
        //    Debug.Log(item.gameObject.name);
        //}

        //StartCoroutine(ShuffleShapes());

        //puzzle.winCondition = GetWinCondition();
        //puzzle.currentCondition = GetCurrentCondition();

        //Camera.main.transform.position = new Vector3(-8f, puzzle.height / 2f - .5f, -10);

        //foreground = GameObject.FindGameObjectWithTag("Foreground");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            // Quit the application
            BackToMenu();
        }

        if (Input.GetMouseButtonDown(0) && LevelCompleted())
        {
            StartCoroutine(LevelCompletedScreen());
        }
    }

    Vector2 Dimensions()
    {
        Vector2 temp = Vector2.zero;
        GameObject[] shapes = GameObject.FindGameObjectsWithTag("Shapes");
        foreach (var s in shapes)
        {
            if (s.transform.position.x > temp.x) temp.x = s.transform.position.x;
            if (s.transform.position.y > temp.y) temp.y = s.transform.position.y;
        }

        temp.x++;
        temp.y++;

        return temp;
    }

    private int GetWinCondition()
    {
        int winCondition = 0;

        foreach (var shape in puzzle.shapes)
        {
            foreach (var connection in shape.connection)
            {
                if (connection)
                    winCondition += 1;
            }
        }

        winCondition /= 2;

        return winCondition;
    }

    public int GetCurrentCondition()
    {
        int value = 0;

        for (int h = 0; h < puzzle.height; h++)
        {
            for (int w = 0; w < puzzle.width; w++)
            {

                //check top connection
                if (h!=puzzle.height - 1)
                if (puzzle.shapes[w,h].connection[0] && puzzle.shapes[w, h + 1].connection[2])
                {
                    value++;
                }

                //check right connection
                if (w != puzzle.width - 1)
                    if (puzzle.shapes[w, h].connection[1] && puzzle.shapes[w + 1, h].connection[3])
                {
                    value++;
                }
            }
        }

        return value;
    }

    IEnumerator ShuffleShapes()
    {
        foreach (var shape in puzzle.shapes)
        {
            int rand = Random.Range(0, 4);

            for (int i = 0; i < rand; i++)
            {
                yield return new WaitForSeconds(.01f);
                shape.RotateShapes(0);
            }
        }

        foreach (var shapeElement in GameObject.FindGameObjectsWithTag("Shapes"))
        {
            LeanTween.scale(shapeElement, Vector3.one, .5f);
        }
    }

    public void SlideCamera()
    {
        LeanTween.moveX(Camera.main.gameObject, puzzle.width / 2f - .5f, .25f);
    }

    public void GameWon()
    {
        if (GetWinCondition() == GetCurrentCondition())
        {


            StartCoroutine(RestartLevelCOR());

            
        }

        else Debug.Log("not yet");


    }

    public void BackToMenu()
    {
        LeanTween.moveX(Camera.main.gameObject, -8f, .25f);
    }

    private void RestartLevel()
    {
        Vector2 dimensions = Dimensions();

        puzzle.width = (int)dimensions.x;
        puzzle.height = (int)dimensions.y;

        puzzle.shapes = new ShapeElement[puzzle.width, puzzle.height];

        foreach (var shapeElement in GameObject.FindGameObjectsWithTag("Shapes"))
        {
            puzzle.shapes[(int)shapeElement.transform.position.x, (int)shapeElement.transform.position.y] = shapeElement.GetComponent<ShapeElement>();
        }

        foreach (var item in puzzle.shapes)
        {
            Debug.Log(item.gameObject.name);
        }

        StartCoroutine(ShuffleShapes());

        puzzle.winCondition = GetWinCondition();
        puzzle.currentCondition = GetCurrentCondition();

        Camera.main.transform.position = new Vector3(puzzle.width / 2f - .5f, puzzle.height / 2f - .5f, -10);
    }

    IEnumerator RestartLevelCOR()
    {
        Debug.Log("Level Passed!");

        LeanTween.move(foreground, Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10f)), 0);

        yield return new WaitForSeconds(.01f);
        LeanTween.scale(foreground, Vector3.one * 50, 2f);

        yield return new WaitForSeconds(1f);

        SceneManager.UnloadSceneAsync("Level_1");

        SceneManager.LoadScene("Level_2", LoadSceneMode.Additive);

        yield return new WaitForSeconds(.1f);
        RestartLevel();
    }

    public bool LevelCompleted()
    {
        if (GetWinCondition() == GetCurrentCondition()) return true;
        else return false;
    }

    public IEnumerator LevelCompletedScreen()
    {
        foreach (var shapeElement in GameObject.FindGameObjectsWithTag("Shapes"))
        {
            LeanTween.scale(shapeElement, Vector3.one * 1.25f, .5f);
        }

        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        GameWon();
    }

    IEnumerator LoadLevel_1()
    {
        yield return new WaitForSeconds(.1f);
        Vector2 dimensions = Dimensions();

        puzzle.width = (int)dimensions.x;
        puzzle.height = (int)dimensions.y;

        puzzle.shapes = new ShapeElement[puzzle.width, puzzle.height];

        foreach (var shapeElement in GameObject.FindGameObjectsWithTag("Shapes"))
        {
            puzzle.shapes[(int)shapeElement.transform.position.x, (int)shapeElement.transform.position.y] = shapeElement.GetComponent<ShapeElement>();
        }

        foreach (var item in puzzle.shapes)
        {
            Debug.Log(item.gameObject.name);
        }

        StartCoroutine(ShuffleShapes());

        puzzle.winCondition = GetWinCondition();
        puzzle.currentCondition = GetCurrentCondition();

        Camera.main.transform.position = new Vector3(-8f, puzzle.height / 2f - .5f, -10);

        foreground = GameObject.FindGameObjectWithTag("Foreground");
    }
}
