using UnityEngine;
using System.Collections;

public class WaterSplashScript : MonoBehaviour {

    /// <summary>
    /// The splash prefab to be instantiated.
    /// </summary>
    public GameObject splashPrefab;

    /// <summary>
    /// The duration of splash object to be kept before destroyed, in seconds.
    /// </summary>
    public float splashLifetime;

    void OnTriggerEnter(Collider other)
    {
        // Ignores other colliders unless it is player
        if (other.tag == "Player")
        {
            // Note to self: splash direction is ignored

            //other.SendMessage("GameOver");
        }
    }

}
