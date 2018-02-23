using UnityEngine;

[RequireComponent(typeof(PlayerMotor)), RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour {
	[SerializeField] private LayerMask groundLayers;
	[SerializeField] private float speed = 8, sensitivity = 1, jumpForce = 10, gravityForce = 1000, freefallDrag = 0;

	[SerializeField, Header("Jetpack")] private bool useJetpack;
	public bool UseJetpack => useJetpack;
	[SerializeField] private float jetpackForce = 1000, jetpackDrag = 2, fuelUseSpeed = 1f, fuelRegenSpeed = 1 / 3f;
	public float FuelAmount { get; private set; } = 1;

	private bool grounded;
	private bool jetpackActive;

	private PlayerMotor motor;
	private CapsuleCollider capsule;
	private new Rigidbody rigidbody;

	private void Awake() {
		motor = GetComponent<PlayerMotor>();
		capsule = GetComponent<CapsuleCollider>();
		rigidbody = GetComponent<Rigidbody>();
	}

	private void Update() {
		grounded = Physics.CheckSphere(transform.position + capsule.center + (capsule.height / 2 - capsule.radius) * Vector3.down, capsule.radius + 0.1f, groundLayers);

		Cursor.lockState = PauseMenu.Paused ? CursorLockMode.None : CursorLockMode.Locked;

		UpdateRotation();
		UpdateCameraRotation();
		UpdateMovement();

		if (useJetpack) UpdateJetpack();
		else UpdateJump();

		UpdateGravity();

		rigidbody.drag = useJetpack && jetpackActive ? jetpackDrag : freefallDrag;
	}

	private void UpdateRotation() {
		float yInput = PauseMenu.Paused ? 0 : Input.GetAxisRaw("Mouse X");

		Vector3 rotation = new Vector3(0, yInput * sensitivity, 0);

		motor.Rotate(rotation);
	}

	private void UpdateCameraRotation() {
		float xInput = PauseMenu.Paused ? 0 : Input.GetAxisRaw("Mouse Y");

		float rotation = xInput * sensitivity;

		motor.RotateCamera(rotation);
	}

	private void UpdateMovement() {
		float xInput = PauseMenu.Paused ? 0 : Input.GetAxisRaw("Horizontal");
		float zInput = PauseMenu.Paused ? 0 : Input.GetAxisRaw("Vertical");

		Vector3 xMovement = xInput * transform.right;
		Vector3 zMovement = zInput * transform.forward;

		Vector3 velocity = speed * (xMovement + zMovement).normalized;

		motor.Move(velocity);
	}

	private void UpdateJump() {
		if (!PauseMenu.Paused && grounded && Input.GetButtonDown("Jump")) {
			Vector3 jumpVector = jumpForce * Vector3.up;

			motor.Jump(jumpVector);
		}
	}

	private void UpdateJetpack() {
		jetpackActive = !PauseMenu.Paused && Input.GetButton("Jump") && FuelAmount > 0f;
		if (jetpackActive) {
			FuelAmount -= fuelUseSpeed * Time.deltaTime;
			if (FuelAmount > 0.01f) {
				Vector3 jetpackVector = jetpackForce * Vector3.up;

				motor.ApplyVerticalForce(jetpackVector);
			}
		} else if (grounded) {
			FuelAmount += fuelRegenSpeed * Time.deltaTime;
		}

		FuelAmount = Mathf.Clamp01(FuelAmount);
	}

	private void UpdateGravity() {
		if (!jetpackActive) {
			Vector3 gravityVector = gravityForce * Vector3.down;

			motor.ApplyVerticalForce(gravityVector);
		}
	}
}