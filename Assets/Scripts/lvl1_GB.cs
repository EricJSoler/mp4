using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class lvl1_GB : MonoBehaviour {

    #region World Bound support
    private Bounds mWorldBound;  // this is the world bound
    private Vector2 mWorldMin;  // Better support 2D interactions
    private Vector2 mWorldMax;
    private Vector2 mWorldCenter;
    private Camera mMainCamera;
    #endregion

    #region  support runtime enemy creation
    // to support time ...
    private float mlastEnemySpawn;
    private const float kEnemySpawnInterval = 3.0f; // 3 enemies/second

    // spwaning enemy ...
    public GameObject mEnemyToSpawn = null;
    public const int kInitialEnemyCount = 5;
    #endregion

    private int mEnemyCount;
    private int mEggCount;
    private bool mMovingEnemy;
    private float nextAppleSpawn;
    private int mEnemiesHit;

    // Use this for initialization
    void Start()
    {

        #region world bound support
        mMainCamera = Camera.main;
        mWorldBound = new Bounds(Vector3.zero, Vector3.one);
        UpdateWorldWindowBound();
        #endregion

        mEggCount = 0;
        mEnemyCount = 0;
        mMovingEnemy = true;
        mEnemiesHit = 0;

        #region initialize enemy spawning
        if (null == mEnemyToSpawn)
            mEnemyToSpawn = Resources.Load("Prefabs/lvl1_enemy") as GameObject;
        for (int i = 0; i < kInitialEnemyCount; i++)
        {
            GameObject e = (GameObject)Instantiate(mEnemyToSpawn);
            e.transform.position = new Vector3((Random.value * mWorldMax.x) + (Random.value * -mWorldMax.x),
                (Random.value * mWorldMax.y) + (Random.value * -mWorldMax.y), 0f);
            mEnemyCount++;
        }
        #endregion

        nextAppleSpawn = 0f;
       
    }
    public void scoreIncrease()
    {
        mEnemiesHit++;
    }
    // Update is called once per frame
    void Update()
    {
        GameObject win = GameObject.Find("Win");
        UnityEngine.UI.Text winText = win.GetComponent<UnityEngine.UI.Text>();
        winText.enabled = false;

        if (Input.GetKeyDown(KeyCode.Space))
            mMovingEnemy = !mMovingEnemy;

        if (mMovingEnemy)
        {
            SpawnAnEnemy();
            SpawnApple();
        }

        GameObject scoreObject = GameObject.Find("Score");
        UnityEngine.UI.Text score = scoreObject.GetComponent<UnityEngine.UI.Text>();
        score.text = "Students Graded: " + mEnemiesHit;

        //Load mp3 scene upon killing all enemies
        if (mEnemyCount <= 0)
        {
            winText.enabled = true;

            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene("mp3");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Menu");

    }

    #region Game Window World size bound support
    public enum WorldBoundStatus
    {
        CollideTop,
        CollideLeft,
        CollideRight,
        CollideBottom,
        Outside,
        Inside
    };

    /// <summary>
    /// This function must be called anytime the MainCamera is moved, or changed in size
    /// </summary>
    public void UpdateWorldWindowBound()
    {
        // get the main 
        if (null != mMainCamera)
        {
            float maxY = mMainCamera.orthographicSize;
            float maxX = mMainCamera.orthographicSize * mMainCamera.aspect;
            float sizeX = 2 * maxX;
            float sizeY = 2 * maxY;
            float sizeZ = Mathf.Abs(mMainCamera.farClipPlane - mMainCamera.nearClipPlane);

            // Make sure z-component is always zero
            Vector3 c = mMainCamera.transform.position;
            c.z = 0.0f;
            mWorldBound.center = c;
            mWorldBound.size = new Vector3(sizeX, sizeY, sizeZ);

            mWorldCenter = new Vector2(c.x, c.y);
            mWorldMin = new Vector2(mWorldBound.min.x, mWorldBound.min.y);
            mWorldMax = new Vector2(mWorldBound.max.x, mWorldBound.max.y);
        }
    }

    public Vector2 WorldCenter { get { return mWorldCenter; } }
    public Vector2 WorldMin { get { return mWorldMin; } }
    public Vector2 WorldMax { get { return mWorldMax; } }

    public WorldBoundStatus ObjectCollideWorldBound(Bounds objBound)
    {
        WorldBoundStatus status = WorldBoundStatus.Inside;

        if (mWorldBound.Intersects(objBound))
        {
            if (objBound.max.x > mWorldBound.max.x)
                status = WorldBoundStatus.CollideRight;
            else if (objBound.min.x < mWorldBound.min.x)
                status = WorldBoundStatus.CollideLeft;
            else if (objBound.max.y > mWorldBound.max.y)
                status = WorldBoundStatus.CollideTop;
            else if (objBound.min.y < mWorldBound.min.y)
                status = WorldBoundStatus.CollideBottom;
            else if ((objBound.min.z < mWorldBound.min.z) || (objBound.max.z > mWorldBound.max.z))
                status = WorldBoundStatus.Outside;
        }
        else
            status = WorldBoundStatus.Outside;
        return status;
    }
    #endregion

    #region Counting Functions for Echo
    public void addEggCount()
    {
        mEggCount++;
    }

    public void addEnemyCount()
    {
        mEnemyCount++;
    }

    public void subtractEggCount()
    {
        mEggCount--;
    }

    public void subtractEnemyCount()
    {
        mEnemyCount--;
    }
    #endregion

    public bool isEnemyMoving()
    {
        return mMovingEnemy;
    }

    #region enemy spawning support
    private void SpawnAnEnemy()
    {
        //if ((Time.realtimeSinceStartup - mlastEnemySpawn) > kEnemySpawnInterval)
        //{
        //    spawnEnemyHelper();
        //    mlastEnemySpawn = Time.realtimeSinceStartup;
        //}
    }

    private void spawnEnemyHelper()
    {
        GameObject e = (GameObject)Instantiate(mEnemyToSpawn);
        e.transform.position = new Vector3((Random.value * mWorldMax.x) + (Random.value * -mWorldMax.x),
                               (Random.value * mWorldMax.y) + (Random.value * -mWorldMax.y), 0f);
        mEnemyCount++;
    }
    #endregion

    private void SpawnApple()
    {
        float coinflip = Random.value + 1;
        if (Time.realtimeSinceStartup - nextAppleSpawn > 0)
        {
            GameObject apple = Resources.Load("Prefabs/lvl1_apple") as GameObject;
            GameObject e = (GameObject)Instantiate(apple);
            e.transform.position = new Vector3((Random.value * mWorldMax.x) + (Random.value * -mWorldMax.x),
                            (Random.value * mWorldMax.y) + (Random.value * -mWorldMax.y), 0f);
            nextAppleSpawn = Time.realtimeSinceStartup + 5 * coinflip;
        }
    }
}
