using UnityEngine;
using System.Collections;

public class AppleBehavior : MonoBehaviour
{

    private float mSpeed = 100f;
    private GlobalBehavior mGameManager;

    void Start()
    {
        mGameManager = GameObject.Find("GameManager").GetComponent<GlobalBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        GlobalBehavior.WorldBoundStatus status =
            mGameManager.ObjectCollideWorldBound(GetComponent<Renderer>().bounds);

        if (status != GlobalBehavior.WorldBoundStatus.Inside)
        {
            Destroy(this.gameObject);
        }
    }

}
