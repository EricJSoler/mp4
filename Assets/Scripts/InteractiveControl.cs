using UnityEngine;	
using System.Collections;

public class InteractiveControl : MonoBehaviour {

	public GameObject mProjectile = null;

	#region user control references
	private float kHeroSpeed = 20.0f;
	private float kHeroRotateSpeed = 45.0f; // 45 degrees/sec
    private float kEggSpawnInterval = 0.1f;
    private Vector3 mClampPosition;
    private float mlastEggFire;
    private GlobalBehavior mGameManager;
    #endregion

    // Use this for initialization
    void Start () {
		// initialize projectile spawning
		if (null == mProjectile)
			mProjectile = Resources.Load ("Prefabs/Egg") as GameObject;
        mlastEggFire = Time.realtimeSinceStartup;
        mGameManager = GameObject.Find("GameManager").GetComponent<GlobalBehavior>();
    }
	
	// Update is called once per frame
	void Update () {
        #region user movement control
        transform.position += Input.GetAxis ("Vertical")  * transform.up * (kHeroSpeed * Time.smoothDeltaTime);

        float clampedX = Mathf.Clamp(this.transform.position.x, mGameManager.WorldMin.x, mGameManager.WorldMax.x);
        float clampedY = Mathf.Clamp(this.transform.position.y, mGameManager.WorldMin.y, mGameManager.WorldMax.y);
        mClampPosition = new Vector3(clampedX, clampedY, 0.0f);
        transform.position = mClampPosition;

        transform.Rotate(Vector3.forward, -1f * Input.GetAxis("Horizontal") * (kHeroRotateSpeed * Time.smoothDeltaTime));
        #endregion

        #region fire egg
        if (Input.GetAxis ("Fire1") > 0f) { // this is Left-Control
            if (Time.realtimeSinceStartup - mlastEggFire > kEggSpawnInterval)
            {
                GameObject e = Instantiate(mProjectile) as GameObject;
                EggBehavior egg = e.GetComponent<EggBehavior>(); // Shows how to get the script from GameObject
                if (null != egg)
                {
                    mlastEggFire = Time.realtimeSinceStartup;
                    e.transform.position = transform.position;
                    egg.SetForwardDirection(transform.up);
                }
            }
		}
        #endregion

    }
}
