﻿using UnityEngine;
using System.Collections;

public class lvl1_enemy : MonoBehaviour {

    private enum EnemyState
    {
        Normal,
        Run,
        Stunned,
        Scary
    }

    private float kReferenceSpeed = Random.Range(20.0f, 40.0f);
    public const float kTowardsCenter = 0.5f;
    public const float kRotateSpeed = -9.0f; // 9 degrees/sec counterclockwise
    public const float kRunRotationSpeed = -30.0f;
    public const float kStunnedDuration = 5.0f;
    private lvl1_GB mGameManager;
    private lvl1_IC mHero;
    private SpriteRenderer mRenderder;
    private EnemyState mState;
    private int mHits;
    private float mTimeStunned;
    private bool wasScary;

    public float mSpeed;

    // Use this for initialization
    void Start()
    {
        NewDirection();
        mGameManager = GameObject.Find("GameManager").GetComponent<lvl1_GB>();
        mHero = GameObject.Find("Hero").GetComponent<lvl1_IC>();
        mRenderder = gameObject.GetComponent<SpriteRenderer>();
        mState = EnemyState.Normal;
        mSpeed = kReferenceSpeed;
        mHits = 0;
        wasScary = false;
    }

    // Update is called once per frame
    void Update()
    {

        // Clamp 
        lvl1_GB.WorldBoundStatus status =
            mGameManager.ObjectCollideWorldBound(GetComponent<Renderer>().bounds);
        if (status != lvl1_GB.WorldBoundStatus.Inside)
            NewDirection();

        //State Change
        #region State Change
        Vector3 toHero = transform.position - mHero.transform.position;
        float dot = Vector3.Dot(toHero, mHero.transform.up);
        if (mState == EnemyState.Stunned && Time.realtimeSinceStartup - mTimeStunned < kStunnedDuration)
        {
            mState = EnemyState.Stunned;
            if (null != mRenderder)
                mRenderder.sprite = Resources.Load("Textures/stunned_student", typeof(Sprite)) as Sprite;
        }
        else
        {
            if (toHero.magnitude <= 30.0f && dot > 0) // RUN
            {
                mState = EnemyState.Run;
                if (null != mRenderder)
                    mRenderder.sprite = Resources.Load("Textures/running_student", typeof(Sprite)) as Sprite;
            }
            else if (mState == EnemyState.Scary || wasScary)
            {
                mState = EnemyState.Scary;
                if (null != mRenderder)
                    mRenderder.sprite = Resources.Load("Textures/scary_student", typeof(Sprite)) as Sprite;
            }
            else // Normal
            {
                mState = EnemyState.Normal;
                if (null != mRenderder)
                    mRenderder.sprite = Resources.Load("Textures/student", typeof(Sprite)) as Sprite;
            }
        }
        #endregion


        // Movement
        #region movements
        if (mState == EnemyState.Normal || mState == EnemyState.Scary)
            Move();
        else if (mState == EnemyState.Run)
            RunAway();
        else
            Rotate();
        #endregion
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "lvl1_egg(Clone)")
        {
            mHits++;
            var source = GetComponents<AudioSource>();
            AudioSource blast = source[0];
            AudioSource no = source[1];
            blast.Play();

            if (mHits > 2) // 3 hits and die
            {
                mGameManager.subtractEnemyCount();
                mGameManager.scoreIncrease();
                Destroy(this.gameObject);
            }
            else // stunned
            {
                no.Play();
                mTimeStunned = Time.realtimeSinceStartup;
                mState = EnemyState.Stunned;
                if (null != mRenderder)
                    mRenderder.sprite = Resources.Load("Textures/stunned_student", typeof(Sprite)) as Sprite;
            }
            mGameManager.subtractEggCount();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name == "lvl1_apple(Clone)")
        {
            mHits -= 3;
            mState = EnemyState.Scary;
            wasScary = true;
            if (null != mRenderder)
                mRenderder.sprite = Resources.Load("Textures/scary_student", typeof(Sprite)) as Sprite;
            Destroy(other.gameObject);
        }
    }

    private void Move()
    {
        if (mGameManager.isEnemyMoving()) // Start Enemy Movement
            transform.position += (mSpeed * Time.smoothDeltaTime * 0.3f) * transform.up;
    }

    private void RunAway()
    {
        Vector3 toHero = transform.position - mHero.transform.position;
        toHero.Normalize();

        Vector3 cross = Vector3.Cross(toHero, mHero.transform.up);
        transform.Rotate(Vector3.forward, Mathf.Sign(cross.z) * kRunRotationSpeed * Time.smoothDeltaTime);
        transform.position += (mSpeed * Time.smoothDeltaTime) * transform.up;
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.forward, -1f * (kRotateSpeed * Time.smoothDeltaTime));
    }

    // New direction will be something randomly within +- 45-degrees away from the direction
    // towards the center of the world
    private void NewDirection()
    {
        lvl1_GB globalBehavior = GameObject.Find("GameManager").GetComponent<lvl1_GB>();

        // we want to move towards the center of the world
        Vector2 v = globalBehavior.WorldCenter - new Vector2(transform.position.x, transform.position.y);
        // this is vector that will take us back to world center
        v.Normalize();
        Vector2 vn = new Vector2(v.y, -v.x); // this is a direciotn that is perpendicular to V

        float useV = 1.0f - Mathf.Clamp(kTowardsCenter, 0.01f, 1.0f);
        float tanSpread = Mathf.Tan(useV * Mathf.PI / 2.0f);

        float randomX = Random.Range(0f, 1f);
        float yRange = tanSpread * randomX;
        float randomY = Random.Range(-yRange, yRange);

        Vector2 newDir = randomX * v + randomY * vn;
        newDir.Normalize();
        transform.up = newDir;
    }
}
