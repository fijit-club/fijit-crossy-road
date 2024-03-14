using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerMovementScript : MonoBehaviour {
    public bool canMove = false;
    public float timeForMove = 0.2f;
    public float jumpHeight = 1.0f;

    public int minX = -4;
    public int maxX = 4;

    public GameObject[] leftSide;
    public GameObject[] rightSide;

    public float leftRotation = -45.0f;
    public float rightRotation = 90.0f;
    [SerializeField] private LayerMask layersToIgnore;
    
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

    private Transform _nearestTrunkPoint;

    public void Start() {
        current = transform.position;
        moving = false;
        startY = transform.position.y;

        body = GetComponentInChildren<Rigidbody>();

        mesh = GameObject.Find("Player/Chicken Parent");

        score = 0;
        gameStateController = GameObject.Find("GameStateController").GetComponent<GameStateControllerScript>();
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            _initMousePosition = Input.mousePosition;
            _initMousePosition.z = -4.23f;

            _initMousePosition.x /= Screen.width;
            _initMousePosition.y /= Screen.height;
        }
        
        // If player is moving, update the player position, else receive input from user.
        if (moving)
        {
            MovePlayer();
            if (Input.GetMouseButtonDown(0))
            {
                _cachedMove = true;
            }
        }
        else {
            // Update current to match integer position (not fractional).
            current = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));

            if (_cachedMove)
            {
                HandleInput();
                _cachedMoveOccured = true;
            }
            
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
            print(direction.x);
            print(direction.y);
            if (Physics.Raycast(transform.position, new Vector3(0, 0, 3), out var hit, 3f, layersToIgnore))
            {
                EnterTrunk(hit);
            }
            else
                Move(new Vector3(0, 0, 3));
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
                    Move(new Vector3(0, 0, -3));
            }
            else
            {
                if (Physics.Raycast(transform.position, new Vector3(0, 0, 3), out var hit, 3f, layersToIgnore))
                {
                    EnterTrunk(hit);
                }
                else
                    Move(new Vector3(0, 0, 3));
            }
        }
        else if (Mathf.Abs(direction.y) < Mathf.Abs(direction.x))
        {
            if (direction.x < 0f)
                Move(new Vector3(-3, 0, 0));
            else
                Move(new Vector3(3, 0, 0));
        }
    }

    private void EnterTrunk(RaycastHit hit)
    {
        _enteringTrunk = true;
        var trunkFloatingScript = hit.transform.parent.GetComponent<TrunkFloatingScript>();
        var trunkPoints = trunkFloatingScript.trunkPoints;
        _nearestTrunkPoint = trunkPoints[0]; // TODO add a check NEAREST POINT FUNCTION
        trunkFloatingScript.nearestPointIndex = 0;
        var dir = _nearestTrunkPoint.position - transform.position;
        Move(dir);
    }

    private void HandleInput() {	
		// Handle mouse click

        if (Input.GetMouseButtonDown(0) || _cachedMove)
        {
            transform.GetChild(1).DOScaleY(.5f, .1f).SetEase(Ease.OutBounce);
            _initMousePosition = Input.mousePosition;
            _initMousePosition.z = -4.23f;

            _initMousePosition.x /= Screen.width;
            _initMousePosition.y /= Screen.height;
        }
        
		if (Input.GetMouseButtonUp(0) || _cachedMove) {
            transform.GetChild(1).DOScaleY(1f, .1f).SetEase(Ease.OutBounce);
            _finalMousePosition = Input.mousePosition;
            _finalMousePosition.z = -4.23f;

            _finalMousePosition.x /= Screen.width;
            _finalMousePosition.y /= Screen.height;
			HandleMouseClick();
			return;
		}
		
        if (Input.GetKeyDown(KeyCode.W)) {
            Move(new Vector3(0, 0, 1));
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            Move(new Vector3(0, 0, -1));
        }
        else if (Input.GetKeyDown(KeyCode.A)) {
            if (Mathf.RoundToInt(current.x) > minX)
                Move(new Vector3(-1, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            if (Mathf.RoundToInt(current.x) < maxX)
                Move(new Vector3(1, 0, 0));
        }
    }

    private void Move(Vector3 distance) {
        var newPosition = current + distance;

        // Don't move if blocked by obstacle.
        if (Physics.CheckSphere(newPosition + new Vector3(0.0f, 0.5f, 0.0f), 0.1f, ~layersToIgnore)) 
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

        if (_enteringTrunk)
        {
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
            if (_enteringTrunk)
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
        // When game over, disable moving.
        canMove = false;

        // Call GameOver at game state controller (instead of sending messages).
        gameStateController.GameOver();
    }

    public void Reset() {
        // TODO This kind of reset is dirty, refactor might be needed.
        //transform.position = new Vector3(0, 1, 0);
        transform.localScale = new Vector3(1, 1, 1);
        transform.rotation = Quaternion.identity;
        score = 0;
    }
}
