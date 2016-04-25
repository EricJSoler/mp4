using UnityEngine;
using System.Collections;

public class lvl1_egg : MonoBehaviour {

    private float mSpeed = 100f;
    private lvl1_GB mGameManager;

    void Start()
    {
        mGameManager = GameObject.Find("GameManager").GetComponent<lvl1_GB>();
        mGameManager.addEggCount();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (mSpeed * transform.up * Time.smoothDeltaTime);
        lvl1_GB.WorldBoundStatus status =
            mGameManager.ObjectCollideWorldBound(GetComponent<Renderer>().bounds);

        if (status != lvl1_GB.WorldBoundStatus.Inside)
        {
            mGameManager.subtractEggCount();
            Destroy(this.gameObject);
        }
    }

    public void SetForwardDirection(Vector3 f)
    {
        transform.up = f;
    }
}
