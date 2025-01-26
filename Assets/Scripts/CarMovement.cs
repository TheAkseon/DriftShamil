using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _carRigidbody;
    public WheelColliders WheelColliders;
    public WheelMeshes WheelMeshes;

    public float GasInput;
    public float SteeringInput;
    public float BrakeInput;

    public float MotorPower;
    public float BreakPower;

    private float _currentMoveSpeed;
    public AnimationCurve SteeringCurve;

    private void Start()
    {
        _carRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _currentMoveSpeed = _carRigidbody.velocity.magnitude;

        CheckInput();
        ApplyWheelPositions();
        ApplyMotorPower();
        ApplyBrake();
        ApplySteeringSystem();
    }

    private void ApplyMotorPower()
    {
        WheelColliders.RearLeftWheel.motorTorque = MotorPower * GasInput;
        WheelColliders.RearRightWheel.motorTorque = MotorPower * GasInput;
    }

    private void ApplySteeringSystem()
    {
        float steeringAngle = SteeringInput * SteeringCurve.Evaluate(_currentMoveSpeed);
        WheelColliders.FrontLeftWheel.steerAngle = steeringAngle;
        WheelColliders.FrontRightWheel.steerAngle = steeringAngle;
    }

    private void CheckInput()
    {
        GasInput = Input.GetAxis("Vertical");
        SteeringInput = Input.GetAxis("Horizontal");

        float movingDirection = Vector3.Dot(transform.forward, _carRigidbody.velocity);
        if (movingDirection < -0.5f && GasInput > 0)
        {
            BrakeInput = Mathf.Abs(GasInput);
        }
        else if (movingDirection > 0.5f && GasInput < 0)
        {
            BrakeInput = Mathf.Abs(GasInput);
        }
        else
        {
            BrakeInput = 0;
        }
    }

    private void ApplyBrake()
    {
        WheelColliders.FrontLeftWheel.brakeTorque = BrakeInput * BreakPower * 0.7f;

        WheelColliders.FrontRightWheel.brakeTorque = BrakeInput * BreakPower * 0.7f;

        WheelColliders.RearRightWheel.brakeTorque = BrakeInput * BreakPower * 0.3f;

        WheelColliders.RearLeftWheel.brakeTorque = BrakeInput * BreakPower * 0.3f;
    }

    private void ApplyWheelPositions()
    {
        UpdateWheel(WheelColliders.FrontLeftWheel, WheelMeshes.FrontLeftWheel);
        UpdateWheel(WheelColliders.FrontRightWheel, WheelMeshes.FrontRightWheel);
        UpdateWheel(WheelColliders.RearLeftWheel, WheelMeshes.RearLeftWheel);
        UpdateWheel(WheelColliders.RearRightWheel, WheelMeshes.RearRightWheel);
    }

    private void UpdateWheel(WheelCollider wheelCollider, MeshRenderer wheelMesh)
    {
        Quaternion quaternion;
        Vector3 position;

        wheelCollider.GetWorldPose(out position, out quaternion);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quaternion;
    }
}

[System.Serializable]
public class WheelColliders
{
    public WheelCollider FrontRightWheel;
    public WheelCollider FrontLeftWheel;
    public WheelCollider RearRightWheel;
    public WheelCollider RearLeftWheel;
}

[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FrontRightWheel;
    public MeshRenderer FrontLeftWheel;
    public MeshRenderer RearRightWheel;
    public MeshRenderer RearLeftWheel;
}
