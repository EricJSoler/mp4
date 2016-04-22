using UnityEngine;
using System.Collections;

public class EggBehavior : MonoBehaviour {
	
	private float mSpeed = 100f;
    private GlobalBehavior mGameManager;

    void Start()
	{
        mGameManager = GameObject.Find("GameManager").GetComponent<GlobalBehavior>();
        mGameManager.addEggCount();
    }

	// Update is called once per frame
	void Update () {
		transform.position += (mSpeed * transform.up * Time.smoothDeltaTime);
        GlobalBehavior.WorldBoundStatus status =
            mGameManager.ObjectCollideWorldBound(GetComponent<Renderer>().bounds);

        if (status != GlobalBehavior.WorldBoundStatus.Inside)
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
