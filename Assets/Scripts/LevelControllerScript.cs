using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelControllerScript : MonoBehaviour {
    public int minZ = 3;
    public int lineAhead = 40;
    public int lineBehind = 20;

    public GameObject[] linePrefabs;
    public GameObject coins;

    private float _waterSpeed;
    
    private Dictionary<int, GameObject> lines;

    private GameObject player;

    private bool _firstSpawned;
    private int _lastIndex;
    private GameObject _lastLine;

    public void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        lines = new Dictionary<int, GameObject>();

        _waterSpeed = Random.Range(2f, 4f);
    }
	
    public void Update() {
        var playerZ = (int)player.transform.position.z;
        for (var z = Mathf.Max(minZ, playerZ - lineBehind); z <= playerZ + lineAhead; z += 1) {
            if (!lines.ContainsKey(z)) {
                GameObject coin;
                int x = Random.Range(0, 2);
                if (x == 1) {
                    coin = Instantiate(coins);
                    int randX = Random.Range(-4, 4);
                    coin.transform.position = new Vector3(randX, 1, 1.5f);
                }

                int spawnIndex;
                if (_firstSpawned)
                    spawnIndex = Random.Range(0, linePrefabs.Length);
                else
                {
                    _firstSpawned = true;
                    spawnIndex = 0;
                }
                
                var line = Instantiate(
                    linePrefabs[spawnIndex],
                    new Vector3(0, 0, z * 3 - 5),
                    Quaternion.identity
                );

                var trunkGeneratorScript = line.GetComponent<TrunkGeneratorScript>();
                TrunkGeneratorScript lastTrunkGeneratorScript = null;
                if (_lastLine != null)
                {
                    lastTrunkGeneratorScript = _lastLine.GetComponent<TrunkGeneratorScript>();
                }

                if (_lastIndex == 1 && spawnIndex == 1 && lastTrunkGeneratorScript != null)
                {
                    if (lastTrunkGeneratorScript.direction == TrunkGeneratorScript.Direction.Right)
                        trunkGeneratorScript.direction = TrunkGeneratorScript.Direction.Left;
                    else
                        trunkGeneratorScript.direction = TrunkGeneratorScript.Direction.Right;
                }

                line.transform.localScale = new Vector3(1, 1, 3);
                lines.Add(z, line);

                _lastIndex = spawnIndex;
                _lastLine = line;
            }
        }

        foreach (var line in new List<GameObject>(lines.Values)) {
            if (line != null)
            {
                var lineZ = line.transform.position.z;
                if (lineZ < playerZ - lineBehind)
                {
                    lines.Remove((int) lineZ);
                    Destroy(line);
                }
            }
        }
	}

    public void Reset() {
        // TODO This kind of reset is dirty, refactor might be needed.
        if (lines != null) {
            foreach (var line in new List<GameObject>(lines.Values)) {
                Destroy(line);
            }
            Start();
        }
    }
}
