using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadCarGenerator : MonoBehaviour {
    public enum Direction { Left = -1, Right = 1 };

    public bool randomizeValues = false;

    public Direction direction;
    public float speed = 2.0f;
    public float interval = 6.0f;
    public float leftX = -20.0f;
    public float rightX = 20.0f;

    public GameObject[] carPrefabs;

    private float elapsedTime;

    private Vector3 _screenCenter;

    private List<GameObject> cars;

    private bool _spawnFirst;

    public void Start() {
        if (randomizeValues) {
            direction = Random.value < 0.5f ? Direction.Left : Direction.Right;
            speed = Random.Range(6f, 8f);
            interval = Random.Range(2.0f, 4.0f);
        }

        elapsedTime = 0.0f;
        cars = new List<GameObject>();
        _screenCenter = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width / 2, Screen.height / 2, 3.17f));
    }

    private void Spawn()
    {
        var position = transform.position + new Vector3(direction == Direction.Left ? rightX : leftX, 0.6f, 0);
        var o = (GameObject)Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], position, Quaternion.Euler(-90, 90, 0));
        o.GetComponent<CarScript>().speedX = (int)direction * speed;

        if (direction < 0)
            o.transform.rotation = Quaternion.Euler(-90, 270, 0);
        else
            o.transform.rotation = Quaternion.Euler(-90, 90, 0);
            
        cars.Add(o);
    }

    public void Update()
    {
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

            // TODO extract 0.375f and -0.5f to outside -- probably along with genericization
            var position = transform.position + new Vector3(direction == Direction.Left ? rightX : leftX, 0.6f, 0);
            var o = (GameObject)Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], position, Quaternion.Euler(-90, 90, 0));
            o.GetComponent<CarScript>().speedX = (int)direction * speed;

            if (direction < 0)
                o.transform.rotation = Quaternion.Euler(-90, 270, 0);
            else
                o.transform.rotation = Quaternion.Euler(-90, 90, 0);
            
            cars.Add(o);
        }

        foreach (var o in cars.ToArray()) {
            if (direction == Direction.Left && o.transform.position.x < leftX || direction == Direction.Right && o.transform.position.x > rightX) {
                Destroy(o);
                cars.Remove(o);
            }
        }
    }

    public void OnDestroy() {
        foreach (var o in cars) {
            Destroy(o);
        }
    }
}
