using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public CharacterController Controller { get; private set; }

    [SerializeField] private float moveSpeed;
    private Vector3 lookDirection;

    private void Start()
    {
        Controller = GetComponent<CharacterController>();

        lookDirection = transform.forward;
    }

    private void Update()
    {
        Vector3 direction = Camera.main.transform.right * InputHub.Instance.Move.x + Camera.main.transform.forward * InputHub.Instance.Move.y;
        direction.y = 0f;
        direction = direction.normalized;

        lookDirection = direction != Vector3.zero ? direction : lookDirection;

        float currentSpeed = moveSpeed * InputHub.Instance.Move.magnitude;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDirection), 500f * Time.deltaTime);

        Vector3 velocity = direction * currentSpeed;
        velocity.y = -5f;

        Controller.Move(velocity * Time.deltaTime);
    }
}
