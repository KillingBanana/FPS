using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {
	[SerializeField] private new Camera camera;

	private Vector3 velocity, jumpForce, verticalForce, rotation;
	private float cameraRotation, cameraRotationDelta;
	private new Rigidbody rigidbody;

	private void Awake() {
		rigidbody = GetComponent<Rigidbody>();
	}

	private void OnEnable() {
		velocity = Vector3.zero;
		jumpForce = Vector3.zero;
		verticalForce = Vector3.zero;
		rotation = Vector3.zero;
		cameraRotation = 0;
		cameraRotationDelta = 0;
	}

	private void FixedUpdate() {
		if (rotation != Vector3.zero) {
			rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(rotation));
			rotation = Vector3.zero;
		}

		if (camera != null) {
			cameraRotation = Mathf.Clamp(cameraRotation - cameraRotationDelta, -90, 90);
			camera.transform.localEulerAngles = new Vector3(cameraRotation, 0, 0);
			cameraRotationDelta = 0;
		}

		if (jumpForce.magnitude > float.Epsilon) {
			rigidbody.velocity = jumpForce;
			jumpForce = Vector3.zero;
		}

		if (velocity != Vector3.zero) {
			rigidbody.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
			velocity = Vector3.zero;
		}

		if (verticalForce != Vector3.zero) {
			rigidbody.AddForce(verticalForce * Time.fixedDeltaTime, ForceMode.Acceleration);
			verticalForce = Vector3.zero;
		}
	}

	public void Move(Vector3 velocity) => this.velocity = velocity;

	public void Jump(Vector3 jumpForce) => this.jumpForce = jumpForce;

	public void ApplyVerticalForce(Vector3 verticalForce) => this.verticalForce = verticalForce;

	public void Rotate(Vector3 rotation) => this.rotation += rotation;

	public void RotateCamera(float cameraRotationDelta) => this.cameraRotationDelta += cameraRotationDelta;
}