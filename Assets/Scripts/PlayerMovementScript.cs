using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NotSpaceInvaders;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerMovementScript : MonoBehaviour {
    public bool canMove = false;
    public float timeForMove = 0.2f;
    public float jumpHeight = 1.0f;

    public int minX = -4;
    public int maxX = 4;

    public GameObject[] leftSide;
    public GameObject[] rightSide;

    public int coins;
    
    public float leftRotation = -45.0f;
    public float rightRotation = 90.0f;
    
    [SerializeField] private LayerMask layersToIgnore;
    [SerializeField] private LayerMask layersToIgnore2;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TMP_Text[] playScores;
    [SerializeField] private CameraMovement camMove;
    [SerializeField] private GameObject splash;
    [SerializeField] private TMP_Text[] coinsText;
    
    private bool moving;
    private float elapsedTime;

    private Vector3 current;
    private Vector3 target;
    private float startY;

    private Rigidbody body;
    private GameObject mesh;

    private GameStateControllerScript gameStateController;
    private int score;

    private bool _cachedMove;
    private bool _cachedMoveOccured;

    private Vector3 _initMousePosition;
    private Vector3 _finalMousePosition;

    private bool _enteringTrunk;
    private bool _enteredTrunk;

    private Transform _nearestTrunkPoint;
    private int _nearestPointIndex;
    private List<Transform> _trunkPoints;
    private TrunkFloatingScript _trunkFloatingScript;

    public void Start() {
        current = transform.position;
        moving = false;
        startY = transform.position.y;

        body = GetComponentInChildren<Rigidbody>();

        mesh = GameObject.Find("Player/Chicken Parent");

        score = 0;
        gameStateController = GameObject.Find("GameStateController").GetComponent<GameStateControllerScript>();
    }
    
    private static bool ButtonPress()
    {
        if (!EventSystem.current.currentSelectedGameObject) return false;
        bool pointerOverUI = EventSystem.current.currentSelectedGameObject.CompareTag("BUTTON");
        return pointerOverUI;
    }

    public void Update()
    {
        if (pauseMenu.activeInHierarchy || gameOver.activeInHierarchy) return;
        if (Input.GetMouseButtonDown(0) && !ButtonPress())
        {
            _initMousePosition = Input.mousePosition;
            _initMousePosition.z = -4.23f;

            _initMousePosition.x /= Screen.width;
            _initMousePosition.y /= Screen.height;
            
            foreach (var playScore in playScores)
                playScore.text = score.ToString();
        }
        
        // If player is moving, update the player position, else receive input from user.
        if (moving)
        {
            MovePlayer();
            if (Input.GetMouseButtonDown(0) && !ButtonPress())
            {
                _cachedMove = true;
            }
        }
        else {
            // Update current to match integer position (not fractional).
            current = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));

            if (canMove)
                HandleInput();
        }

        score = Mathf.Max(score, (int)current.z);
        gameStateController.score = score / 3;
    }
	
	private void HandleMouseClick() {
        var direction = _finalMousePosition - _initMousePosition;

        if (Mathf.Abs(direction.x) < .1f && Mathf.Abs(direction.y) < .1f)
        {
            if (Physics.Raycast(transform.position, new Vector3(0, 0, 3), out var hit, 3f, layersToIgnore))
            {
                EnterTrunk(hit);
            }
            else
            {
                _enteredTrunk = false;
                Move(new Vector3(0, 0, 3));
            }
        }
        
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            if (direction.y < 0f)
            {
                if (Physics.Raycast(transform.position, new Vector3(0, 0, -3), out var hit, 3f, layersToIgnore))
                {
                    EnterTrunk(hit);
                }
                else
                {
                    _enteredTrunk = false;
                    Move(new Vector3(0, 0, -3));
                }
            }
            else
            {
                if (Physics.Raycast(transform.position, new Vector3(0, 0, 3), out var hit, 3f, layersToIgnore))
                {
                    EnterTrunk(hit);
                }
                else
                {
                    _enteredTrunk = false;
                    Move(new Vector3(0, 0, 3));
                }
            }
        }
        else if (Mathf.Abs(direction.y) < Mathf.Abs(direction.x))
        {
            if (!_enteredTrunk || _trunkPoints.Count == 1)
            {
                if (direction.x < 0f)
                    Move(new Vector3(-3, 0, 0));
                else
                    Move(new Vector3(3, 0, 0));
            }
            else
            {
                if (_nearestPointIndex == 0)
                {
                    if (direction.x < 0f)
                    {
                        var dir = _trunkPoints[1].position - transform.position;
                        _nearestPointIndex = 1;
                        _trunkFloatingScript.nearestPointIndex = 1;
                        _nearestTrunkPoint = _trunkFloatingScript.trunkPoints[1];
                        print(_nearestTrunkPoint.name);
                        Move(dir);
                    }
                    else
                        Move(new Vector3(3, 0, 0));
                }
                else if (_nearestPointIndex == 1)
                {
                    if (direction.x > 0f)
                    {
                        var dir = _trunkPoints[0].position - transform.position;
                        _nearestPointIndex = 0;
                        _trunkFloatingScript.nearestPointIndex = 0;
                        _nearestTrunkPoint = _trunkFloatingScript.trunkPoints[0];
                        print(_nearestTrunkPoint.name);
                        Move(dir);
                    }
                    else
                        Move(new Vector3(-3, 0, 0));
                }
            }
        }
    }
    
    Transform GetClosestEnemy(Transform[] enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in enemies)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    private void EnterTrunk(RaycastHit hit)
    {
        _enteringTrunk = true;
        _enteredTrunk = true;
        _trunkFloatingScript = hit.transform.parent.GetComponent<TrunkFloatingScript>();
        _trunkPoints = _trunkFloatingScript.trunkPoints;
        _nearestTrunkPoint = GetClosestEnemy(_trunkPoints.ToArray()); // TODO add a check NEAREST POINT FUNCTION
        _nearestPointIndex = _trunkFloatingScript.nearestPointIndex = _trunkPoints.IndexOf(_nearestTrunkPoint);;
        var dir = _nearestTrunkPoint.position - transform.position;
        Move(dir);
    }

    private void HandleInput() {	
		// Handle mouse click

        if (Input.GetMouseButtonDown(0) && !ButtonPress())
        {
            transform.GetChild(1).DOScaleY(.5f, .1f).SetEase(Ease.OutBounce);
            _initMousePosition = Input.mousePosition;
            _initMousePosition.z = -4.23f;

            _initMousePosition.x /= Screen.width;
            _initMousePosition.y /= Screen.height;
        }
        
		if (Input.GetMouseButtonUp(0) && !ButtonPress()) {
            transform.GetChild(1).DOScaleY(1f, .1f).SetEase(Ease.OutBounce);
            _finalMousePosition = Input.mousePosition;
            _finalMousePosition.z = -4.23f;

            _finalMousePosition.x /= Screen.width;
            _finalMousePosition.y /= Screen.height;
			HandleMouseClick();
		}
    }

    private void Move(Vector3 distance) {
        var newPosition = current + distance;

        // Don't move if blocked by obstacle.
        if (Physics.CheckSphere(newPosition + new Vector3(0.0f, 0.5f, 0.0f), 0.1f, ~layersToIgnore2)) 
            return;

        target = newPosition;

        moving = true;
        elapsedTime = 0;
        body.isKinematic = true;

        switch (MoveDirection) {
            case "north":
                mesh.transform.DORotate(new Vector3(0, 0, 0), .1f);
                break;
            case "south":
                mesh.transform.DORotate(new Vector3(0, 180, 0), .1f);
                break;
            case "east":
                mesh.transform.DORotate(new Vector3(0, 270, 0), .1f);
                break;
            case "west":
                mesh.transform.DORotate(new Vector3(0, 90, 0), .1f);
                break;
            default:
                break;
        }

        // Rotate arm and leg.
        foreach (var o in leftSide) {
            o.transform.Rotate(leftRotation, 0, 0);
        }

        foreach (var o in rightSide) {
            o.transform.Rotate(rightRotation, 0, 0);
        }
    }

    private void MovePlayer() {
        elapsedTime += Time.deltaTime;

        if (_enteringTrunk || _enteredTrunk)
        {
            print("ENTERING NEAREST POINT");
            target = _nearestTrunkPoint.position;
        }
        
        float weight = (elapsedTime < timeForMove) ? (elapsedTime / timeForMove) : 1;
        float x = Lerp(current.x, target.x, weight);
        float z = Lerp(current.z, target.z, weight);
        float y = Sinerp(current.y, startY + jumpHeight, weight);

        Vector3 result = new Vector3(x, y, z);
        transform.position = result; // note to self: why using transform produce better movement?
        //body.MovePosition(result);

        if (Math.Abs(weight - 1) < .01f)
        {
            result = target;
            if (_enteringTrunk || _enteredTrunk)
                transform.position = _nearestTrunkPoint.position;
        }
            
        if (result == target) {
            if (_cachedMoveOccured)
            {
                _cachedMoveOccured = false;
                _cachedMove = false;
            }
            _enteringTrunk = false;
            moving = false;
            current = target;
            body.isKinematic = false;
            body.AddForce(0, -10, 0, ForceMode.VelocityChange);

            // Return arm and leg to original position.
            foreach (var o in leftSide) {
                //o.transform.rotation = Quaternion.Lerp(o.transform.rotation, Quaternion.identity, 10f * Time.deltaTime);
            }

            foreach (var o in rightSide) {
                //o.transform.rotation = Quaternion.Lerp(o.transform.rotation, Quaternion.identity, 10f * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Coin"))
        {
            coins++;
            foreach (var coinText in coinsText)
            {
                coinText.text = coins.ToString();
            }
        }
        
        if (col.CompareTag("Death") || col.CompareTag("Water"))
        {
            canMove = false;
            StartCoroutine(DelayedDeath());
        }

        if (col.CompareTag("Water"))
        {
            camMove.enabled = false;
            GetComponent<BoxCollider>().enabled = false;

            var pos = transform.position;
            pos.y = 1f;
            Instantiate(splash, pos, Quaternion.Euler(-90, 0, 0));
        }
    }

    private IEnumerator DelayedDeath()
    {
        yield return new WaitForSeconds(2f);
        GameOver();
    }

    private float Lerp(float min, float max, float weight) {
        return min + (max - min) * weight;
    }

    private float Sinerp(float min, float max, float weight) {
        return min + (max - min) * Mathf.Sin(weight * Mathf.PI);
    }

    public bool IsMoving {
        get { return moving; }
    }

    public string MoveDirection {
        get
        {
            if (moving) {
                float dx = target.x - current.x;
                float dz = target.z - current.z;
                if (dz > 0)
                    return "north";
                else if (dz < 0)
                    return "south";
                else if (dx > 0)
                    return "west";
                else
                    return "east";
            }
            else
                return null;
        }
    }

    public void GameOver() {
        canMove = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gameOver.SetActive(true);
        Bridge.GetInstance().UpdateCoins(coins);
        Bridge.GetInstance().SendScore(score);
    }

    public void Reset() {
        // TODO This kind of reset is dirty, refactor might be needed.
        //transform.position = new Vector3(0, 1, 0);
        transform.localScale = new Vector3(1, 1, 1);
        transform.rotation = Quaternion.identity;
        score = 0;
    }
}
