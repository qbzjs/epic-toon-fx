using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Beyblade : MonoBehaviour
{
   

	[SerializeField, Range(0f, 100f)]
	float maxSpeed = 10f;

	[SerializeField, Range(0f, 100f)]
	float maxAcceleration = 10f, maxAirAcceleration = 1f;

	[SerializeField, Range(0f, 10f)]
	float jumpHeight = 2f;

	[SerializeField, Range(0, 5)]
	int maxAirJumps = 0;

	[SerializeField, Range(0, 90)]
	float maxGroundAngle = 25f;

	[SerializeField, Range(0f, 100f)]
	float maxSnapSpeed = 100f;

	[SerializeField, Min(0f)]
	float probeDistance = 1f;

	[SerializeField]
	LayerMask probeMask = -1, stairsMask = -1;

	[SerializeField] float rotateY=30f;
	[SerializeField] Transform rotateYObject;

	[Space(10)]
	[Header("Torque")]
	[SerializeField] float MaxThrustInducedTorque = 5f;
	[SerializeField] AnimationCurve InducedTorqueCurve;
	[Header("Auto Levelling")]
	[SerializeField] bool AutoLevel_Enabled = false;
	[SerializeField] float AutoLevel_UpVectorInfulence = 0f;
	[SerializeField] float AutoLevel_AngularVelocityVelocityInfulence = 0f;

[HideInInspector]	public Rigidbody body;

	[SerializeField]public List<MeshCollider> meshColliders;
	Vector3 velocity, desiredVelocity;



	Vector3 contactNormal, steepNormal;

	int groundContactCount, steepContactCount;

	bool OnGround => groundContactCount > 0;

	bool OnSteep => steepContactCount > 0;

	int jumpPhase;

	float minGroundDotProduct;

	int stepsSinceLastGrounded, stepsSinceLastJump;

	void OnValidate()
	{
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
	
	}

	void Awake()
	{
		body = GetComponent<Rigidbody>();
		OnValidate();
	//	body.centerOfMass = Vector3.down*0.5f;
	}
	Vector2 playerInput;
	void Update()
	{
		rotateYObject.Rotate(0, 6.0f * rotateY * Time.deltaTime, 0);
	}
	public void setPlayerInput(float horizontal,float vertical)
    {
		playerInput.x = horizontal;
		playerInput.y = vertical;
		playerInput = Vector2.ClampMagnitude(playerInput, 1f);

		desiredVelocity =
			new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
	}
	public void setPlayerForward( bool isgo,float forwardSpeed)
    {
        if (isgo)
        {
			playerInput.y = 1;
        }
        else
        {
			playerInput.y = 0;
		}

		
		playerInput = Vector2.ClampMagnitude(playerInput, 1f);

		desiredVelocity.z = playerInput.y *forwardSpeed;

	}
	public void setPlayerRight(bool isgo, float rightSpeed)
	{
		if (isgo)
		{
			playerInput.x = -1;
		}
		else
		{
			playerInput.x = 0;
		}


		playerInput = Vector2.ClampMagnitude(playerInput, 1f);

		desiredVelocity.x = playerInput.x * rightSpeed;

	}
	public void setPlayerInputEndlessRunner(float inputForaward, float inputRight,float speedFoward,float speedRight)
    {
		playerInput.y = inputForaward;
		playerInput.x = inputRight;
		//playerInput = Vector2.ClampMagnitude(playerInput, 1f);

		desiredVelocity =  Vector3.zero;
		desiredVelocity.z=playerInput.y * speedFoward;
		desiredVelocity.x=playerInput.x * speedRight;
	}
	public void denemeSetPlayer(Vector3 dir)
    {
		desiredVelocity = dir;

	}

	public bool isdead;
	void FixedUpdate()
	{
		if (isdead)
        {
			return;
        }
		UpdateState();
		AdjustVelocity();

		body.velocity = velocity;

	
		ClearState();
	}

	void ClearState()
	{
		groundContactCount = steepContactCount = 0;
		contactNormal = steepNormal = Vector3.zero;
	}

	void UpdateState()
	{
		stepsSinceLastGrounded += 1;
		stepsSinceLastJump += 1;
		velocity = body.velocity;
		if (OnGround || SnapToGround() )
		{
			stepsSinceLastGrounded = 0;
			if (stepsSinceLastJump > 1)
			{
				jumpPhase = 0;
			}
			if (groundContactCount > 1)
			{
				contactNormal.Normalize();
			}
		}
		else
		{
			contactNormal = Vector3.up;
		}
	}

	bool SnapToGround()
	{
		if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2)
		{
			return false;
		}
		float speed = velocity.magnitude;
		if (speed > maxSnapSpeed)
		{
			return false;
		}
		if (!Physics.Raycast(
			body.position+0.1f*Vector3.up, Vector3.down, out RaycastHit hit,
			probeDistance, probeMask
		))
		{
			return false;
		}
		if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer))
		{
			return false;
		}
	
		groundContactCount = 1;
		contactNormal = hit.normal;
		float dot = Vector3.Dot(velocity, hit.normal);
		if (dot > 0f)
		{
			velocity = (velocity - hit.normal * dot).normalized * speed;
		}
		return true;
	}

	bool CheckSteepContacts()
	{
		if (steepContactCount > 1)
		{
			steepNormal.Normalize();
			if (steepNormal.y >= minGroundDotProduct)
			{
				steepContactCount = 0;
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
			}
		}
		return false;
	}

	 Vector3 adjustment = new Vector3();
	void AdjustVelocity()
	{
		Vector3 xAxis = ProjectDirectionOnPlane(Vector3.right, contactNormal).normalized;
		Vector3 zAxis = ProjectDirectionOnPlane(Vector3.forward, contactNormal).normalized;


		float currentX = Vector3.Dot(velocity, xAxis);
		float currentZ = Vector3.Dot(velocity, zAxis);

		float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
		float maxSpeedChange = acceleration * Time.deltaTime;

		float newX =
			Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
		float newZ =
			Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

		velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);

		#region pitch and rool

		float incudedRool = InducedTorqueCurve.Evaluate(Mathf.Abs(velocity.x) / maxSpeed) * Mathf.Sign(velocity.x);
		float incudedPitch = InducedTorqueCurve.Evaluate(Mathf.Abs(velocity.z) / maxSpeed) * Mathf.Sign(velocity.z);
		body.AddTorque(incudedPitch * MaxThrustInducedTorque, 0f, -incudedRool * MaxThrustInducedTorque);

        if (AutoLevel_Enabled)
        {


			//Vector3 levelingVector = Quaternion.FromToRotation(transform.up, contactNormal).eulerAngles;

			//if (levelingVector.x > 180f) levelingVector.x -= 360f;
			//if (levelingVector.y > 180f) levelingVector.y -= 360f;
			//if (levelingVector.z > 180f) levelingVector.z -= 360f;
			//if (levelingVector.x < -180f) levelingVector.x += 360f;
			//if (levelingVector.y < -180f) levelingVector.y += 360f;
			//if (levelingVector.z < -180f) levelingVector.z += 360f;




			Vector3 levelingVector = contactNormal- transform.up;// new Vector3(-transform.up.x, 0f, -transform.up.z);
			float autoLevelRoolComponent = levelingVector.x * AutoLevel_UpVectorInfulence +
										body.angularVelocity.z * AutoLevel_AngularVelocityVelocityInfulence;
			
			float autoLevelPitchComponent = levelingVector.z * AutoLevel_UpVectorInfulence +
										-body.angularVelocity.x * AutoLevel_AngularVelocityVelocityInfulence;
			body.AddTorque(autoLevelPitchComponent, 0f, -autoLevelRoolComponent);

        }
        #endregion
    }
	public float  ForwardSpeedReturn()
    {
		Vector3 zAxis = ProjectDirectionOnPlane(Vector3.forward, contactNormal).normalized;
		float currentZ = Vector3.Dot(velocity, zAxis);
		return currentZ;
	}
	public void changeVelo( Vector3 dir )
    {
		Vector3 xAxis = ProjectDirectionOnPlane(Vector3.right, contactNormal).normalized;
		Vector3 zAxis = ProjectDirectionOnPlane(Vector3.forward, contactNormal).normalized;
	

		float changeX = Vector3.Dot(dir, xAxis);
		float changeZ = Vector3.Dot(dir, zAxis);

	

		body.velocity = changeX * xAxis + changeZ * zAxis;

	}
 

	void OnCollisionEnter(Collision collision)
	{
		EvaluateCollision(collision);
	}

	void OnCollisionStay(Collision collision)
	{
		EvaluateCollision(collision);
	}

	void EvaluateCollision(Collision collision)
	{
		float minDot = GetMinDot(collision.gameObject.layer);

		for (int i = 0; i < collision.contactCount; i++)
		{
			Vector3 normal = collision.GetContact(i).normal;
			if (normal.y >= minDot)
			{
				groundContactCount += 1;
				contactNormal += normal;
			}
			else if (normal.y > -0.01f)
			{
				steepContactCount += 1;
				steepNormal += normal;
			}
		}
	}

	Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
	{
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}

	float GetMinDot(int layer)
	{
		return minGroundDotProduct;
	}
}
