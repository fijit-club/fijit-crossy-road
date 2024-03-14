using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrunkGeneratorScript : MonoBehaviour {
    public enum Direction { Left = -1, Right = 1 };

    public bool randomizeValues = false;

    public Direction direction;
    public float speed = 2.0f;
    public float length = 2.0f;
    public float interval = 2.0f;
    public float leftX = -20.0f;
    public float rightX = 20.0f;

    public GameObject[] trunkPrefabs;

    private GameObject trunkPrefab;
    
    private float elapsedTime;

    private List<GameObject> trunks;

    private Vector3 _screenCenter;
    private bool _spawnFirst;

    public void Start() {
	    if (randomizeValues) {
            speed = Random.Range(2.0f, 4f);
            length = Random.Range(1, 4);
            interval = length / speed + Random.Range(2.0f, 4.0f);
        }

        trunkPrefab = trunkPrefabs[Random.Range(0, 2)];
        
        elapsedTime = 0.0f;
        trunks = new List<GameObject>();
	}

    private void Spawn()
    {
        var position = transform.position + new Vector3(direction == Direction.Left ? rightX : leftX, 0, 0);
        var o = (GameObject)Instantiate(trunkPrefab, position, Quaternion.identity);
        o.GetComponent<TrunkFloatingScript>().speedX = (int)direction * speed;

        var scale = o.transform.localScale;
        o.transform.localScale = new Vector3(scale.x, scale.y, scale.z * 3);
        trunks.Add(o);
    }
	
    public void Update() {
        _screenCenter = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width / 2, Screen.height / 2, 3.17f));

        float distance = (_screenCenter.z > transform.position.z)
            ? _screenCenter.z - transform.position.z
            : transform.position.z - _screenCenter.z;
        if (distance > 24f) return;

        if (!_spawnFirst)
        {
            _spawnFirst = true;
            Spawn();
        }
        elapsedTime += Time.deltaTime;

        if (elapsedTime > interval) {
            elapsedTime = 0.0f;

            var position = transform.position + new Vector3(direction == Direction.Left ? rightX : leftX, 0, 0);
            var o = (GameObject)Instantiate(trunkPrefab, position, Quaternion.identity);
            o.GetComponent<TrunkFloatingScript>().speedX = (int)direction * speed;

            var scale = o.transform.localScale;
            o.transform.localScale = new Vector3(scale.x, scale.y, scale.z * 3);

            trunks.Add(o);
        }

        foreach (var o in trunks.ToArray()) {
            if (direction == Direction.Left && o.transform.position.x < leftX || direction == Direction.Right && o.transform.position.x > rightX) {
                Destroy(o);
                trunks.Remove(o);
            }
        }
	}

    void OnDestroy() {
        foreach (var o in trunks) {
            Destroy(o);
        }
    }
}
