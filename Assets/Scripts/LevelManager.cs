using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    // TODO: Implement random level generation instead of the manual levels
    // TODO: Make the fields private serializable instead of public

    //public bool generate = true;
    public bool restartScreenOn = false;

    public GameObject optionalModes;
    public GameObject mainMode;
    public GameObject originalMode;
    public Image timerGraphic;
    public GameObject RestartScreen_1;
    public UImanager uiManager;

    public GameObject failTime;
    public GameObject failStep;


    // === Private Variables ===

    public int steps = 25;
    public int modeCheck = 0;

    private GameObject foreground;
    private int progressionLevel = 1;
    private int currentActiveLevel = 1;
    private float timer = 20f;
    private bool startTimer = false;
    private bool canQuit = true;



    private void Awake()
    {

        //PlayerPrefs.DeleteAll();

        //Save player progress for the initial stage and give difficulty options once three levels are compleeted

        if (PlayerPrefs.HasKey("levels_completed"))
        {
            progressionLevel = PlayerPrefs.GetInt("levels_completed");

        }

        else
        {
            PlayerPrefs.SetInt("levels_completed", 1);
            PlayerPrefs.Save();
        }

        if (progressionLevel < 4)

        {
            optionalModes.SetActive(false);
            mainMode.SetActive(false);
            originalMode.SetActive(true);
            SceneManager.LoadScene(progressionLevel, LoadSceneMode.Additive);
        }

        else

        {
            //SceneManager.LoadSceneAsync(levelNumber, LoadSceneMode.Additive);
            optionalModes.SetActive(true);
            mainMode.SetActive(true);
            originalMode.SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // Start the current level in player's progress in the background

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

            if (progressionLevel < 4) BackToMenu();
            else QuitOptionalLevel();



        }

        if (Input.GetMouseButtonDown(0) && LevelCompleted())
        {
            StartCoroutine(LevelCompletedScreen());
        }

        if (startTimer)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0 || steps < 0)
        {
            startTimer = false;

            LevelLost();

            timer = 20;
            steps = 25;
        }

        if (modeCheck == 1) timerGraphic.fillAmount = timer/20;
        if (modeCheck == 2) timerGraphic.fillAmount = (float)steps/25;


        //Debug.Log(timer);
    }

    // Get the dimensions of the initialized puzzle to then use for camera adjustment
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


    // The win condition is reached when the number of total connections is equal to the number of current connections
    // Devided by two because every connection goes both ways
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

    // Shuffle shapes whenever a level is initialized
    //TODO: Make shuffling faster/async
    IEnumerator ShuffleShapes()
    {
        foreach (var shape in puzzle.shapes)
        {
            int rand = Random.Range(0, 4);

            for (int i = 0; i < rand; i++)
            {
                //Delay, to let the shapes rotate just in case

                yield return new WaitForSeconds(.01f);
                shape.RotateShapes(0);
            }
        }

        foreach (var shapeElement in GameObject.FindGameObjectsWithTag("Shapes"))
        {
            LeanTween.scale(shapeElement, Vector3.one, .5f);
        }

        //Initialize the timer and the step count for difficult levels
        //TODO: Make this variable depending on the level/connection count/difficulty
        timer = 20f;
        steps = 25;

        //Check which fail condition mode it is and let the timer start later
        if (modeCheck == 1 || modeCheck == 2) timerGraphic.gameObject.SetActive(true);
        if (modeCheck == 1) startTimer = true;

    }

    //HACK: Move the camera to the center of the puzzle for the transition like in the original game
    public void SlideCamera()
    {
        LeanTween.moveX(Camera.main.gameObject, puzzle.width / 2f - .5f, .25f);
    }

    public void GameWon()
    {
        if (GetWinCondition() == GetCurrentCondition() && progressionLevel < 4)
        {
            StartCoroutine(RestartLevelCOR(progressionLevel));
        }

        else if (GetWinCondition() == GetCurrentCondition() && canQuit)
        {
            QuitOptionalLevel();
        }

        //else Debug.Log("not yet");


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

        //foreach (var item in puzzle.shapes)
        //{
        //    //Debug.Log(item.gameObject.name);
        //}

        StartCoroutine(ShuffleShapes());

        puzzle.winCondition = GetWinCondition();
        puzzle.currentCondition = GetCurrentCondition();

        Camera.main.transform.position = new Vector3(puzzle.width / 2f - .5f, puzzle.height / 2f - .5f, -10);

        foreground = GameObject.FindGameObjectWithTag("Foreground");

    }

    IEnumerator RestartLevelCOR(int level)
    {
        //Debug.Log("Level Passed!");

        LeanTween.move(foreground, Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 5f)), 0);

        yield return new WaitForSeconds(.01f);
        LeanTween.scale(foreground, Vector3.one * 50, 2f);

        yield return new WaitForSeconds(1f);


        if (level < 3)
        {
            SceneManager.UnloadSceneAsync(level);

            SceneManager.LoadScene(level + 1, LoadSceneMode.Additive);

            yield return new WaitForSeconds(.1f);

            progressionLevel++;

            PlayerPrefs.SetInt("levels_completed", progressionLevel);
            PlayerPrefs.Save();


            RestartLevel();
        }

        else
        {
            progressionLevel++;

            PlayerPrefs.SetInt("levels_completed", progressionLevel);
            PlayerPrefs.Save();

            optionalModes.SetActive(true);
            mainMode.SetActive(true);
            originalMode.SetActive(false);
            BackToMenu();
            uiManager.SlideUIBack();
            yield return new WaitForSeconds(.5f);
            SceneManager.UnloadSceneAsync(level);
        }
    }

    public bool LevelCompleted()
    {
        if (GetWinCondition() == GetCurrentCondition()) return true;
        else return false;
    }

    IEnumerator LevelCompletedScreen()
    {
        startTimer = false;

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

        //foreach (var item in puzzle.shapes)
        //{
        //    Debug.Log(item.gameObject.name);
        //}

        StartCoroutine(ShuffleShapes());

        puzzle.winCondition = GetWinCondition();
        puzzle.currentCondition = GetCurrentCondition();



        Camera.main.transform.position = new Vector3(-8f, puzzle.height / 2f - .5f, -10);

        foreground = GameObject.FindGameObjectWithTag("Foreground");
    }

    public void LoadLevelOptional(int levelNumber = 1)
    {
        uiManager.SlideUI();

        int mode = levelNumber % 10;
        levelNumber /= 10;
        modeCheck = mode;


        if (modeCheck == 1)
        {
            failTime.SetActive(true);
            failStep.SetActive(false);

        }

        else if(modeCheck == 2)
        {
            failTime.SetActive(false);
            failStep.SetActive(true);

        }

        currentActiveLevel = levelNumber;
        StartCoroutine(StartOptionalLevel(levelNumber));
    }

    IEnumerator StartOptionalLevel(int level)
    {
        Debug.Log("Level Passed!");

        //LeanTween.move(foreground, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f)), 0);

        //yield return new WaitForSeconds(.01f);
        //LeanTween.scale(foreground, Vector3.one * 50, 2f);

        //yield return new WaitForSeconds(1f);

        //SceneManager.UnloadSceneAsync(level);

        SceneManager.LoadScene(level, LoadSceneMode.Additive);

        yield return new WaitForSeconds(1f);

        //PlayerPrefs.SetInt("levels_completed", progressionLevel);
        //PlayerPrefs.Save();

        //progressionLevel++;


        StartOptionalLevel();


    }

    public void StartOptionalLevel()
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

        SlideCamera();



        puzzle.winCondition = GetWinCondition();
        puzzle.currentCondition = GetCurrentCondition();

        canQuit = true;

        Camera.main.transform.position = new Vector3(puzzle.width / 2f - .5f, puzzle.height / 2f - .5f, -10);

        //foreground = GameObject.FindGameObjectWithTag("Foreground");
    }

    public void LevelLost()
    {
        RestartScreen_1.SetActive(true);
        restartScreenOn = true;
    }

    public void RestartInstance()
    {
        StartCoroutine(ShuffleShapes());
    }

    public void QuitOptionalLevel()
    {
        StartCoroutine(QuitOptionalLevelCOR());
    }

    IEnumerator QuitOptionalLevelCOR()
    {
        timerGraphic.gameObject.SetActive(false);
        BackToMenu();
        uiManager.SlideUIBack();
        canQuit = false;
        yield return new WaitForSeconds(.5f);
        SceneManager.UnloadSceneAsync(currentActiveLevel);

    }
}
