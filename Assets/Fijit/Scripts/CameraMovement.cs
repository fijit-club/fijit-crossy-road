using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform player;

    private Vector3 _offset;
    
    private void Start()
    {
        _offset = transform.position - player.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.position + _offset, 2f * Time.deltaTime);
    }
}
