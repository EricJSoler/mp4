using UnityEngine;
using System.Collections;

public class lvl1_apple : MonoBehaviour
{

    private float mSpeed = 100f;
    private lvl1_GB mGameManager;

    void Start()
    {
        mGameManager = GameObject.Find("GameManager").GetComponent<lvl1_GB>();
    }

    // Update is called once per frame
    void Update()
    {
        lvl1_GB.WorldBoundStatus status =
            mGameManager.ObjectCollideWorldBound(GetComponent<Renderer>().bounds);

        if (status != lvl1_GB.WorldBoundStatus.Inside)
        {
            Destroy(this.gameObject);
        }
    }

}
