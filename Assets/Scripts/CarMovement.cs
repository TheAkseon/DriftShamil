using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _carRigidbody;
    public WheelColliders WheelColliders;
    public WheelMeshes WheelMeshes;
    public WheelParticles WheelParticles;
    public GameObject SmokePrefab;
    public GameObject TireTrail;

    public float GasInput;
    public float SteeringInput;
    public float BrakeInput;

    public float MotorPower;
    public float BreakPower;

    private float _currentMoveSpeed;
    public AnimationCurve SteeringCurve;
    public float SlipAngle;

    private void Start()
    {
        _carRigidbody = GetComponent<Rigidbody>();
        InstantiateSmoke();
    }

    private void InstantiateSmoke()
    {
        if (SmokePrefab)
        {
            WheelParticles.FRWheel = Instantiate(SmokePrefab, WheelColliders.FrontRightWheel.transform.position - Vector3.up * WheelColliders.FrontRightWheel.radius, Quaternion.identity, WheelColliders.FrontRightWheel.transform)
                .GetComponent<ParticleSystem>();
            WheelParticles.FLWheel = Instantiate(SmokePrefab, WheelColliders.FrontLeftWheel.transform.position - Vector3.up * WheelColliders.FrontRightWheel.radius, Quaternion.identity, WheelColliders.FrontLeftWheel.transform)
                .GetComponent<ParticleSystem>();
            WheelParticles.RRWheel = Instantiate(SmokePrefab, WheelColliders.RearRightWheel.transform.position - Vector3.up * WheelColliders.FrontRightWheel.radius, Quaternion.identity, WheelColliders.RearRightWheel.transform)
                .GetComponent<ParticleSystem>();
            WheelParticles.RLWheel = Instantiate(SmokePrefab, WheelColliders.RearLeftWheel.transform.position - Vector3.up * WheelColliders.FrontRightWheel.radius, Quaternion.identity, WheelColliders.RearLeftWheel.transform)
                .GetComponent<ParticleSystem>();
        }
        if (TireTrail)
        {
            WheelParticles.FRWheelTrail = Instantiate(TireTrail, WheelColliders.FrontRightWheel.transform.position - Vector3.up * WheelColliders.FrontRightWheel.radius, Quaternion.identity, WheelColliders.FrontRightWheel.transform)
                .GetComponent<TrailRenderer>();
            WheelParticles.FLWheelTrail = Instantiate(TireTrail, WheelColliders.FrontLeftWheel.transform.position - Vector3.up * WheelColliders.FrontRightWheel.radius, Quaternion.identity, WheelColliders.FrontLeftWheel.transform)
                .GetComponent<TrailRenderer>();
            WheelParticles.RRWheelTrail = Instantiate(TireTrail, WheelColliders.RearRightWheel.transform.position - Vector3.up * WheelColliders.FrontRightWheel.radius, Quaternion.identity, WheelColliders.RearRightWheel.transform)
                .GetComponent<TrailRenderer>();
            WheelParticles.RLWheelTrail = Instantiate(TireTrail, WheelColliders.RearLeftWheel.transform.position - Vector3.up * WheelColliders.FrontRightWheel.radius, Quaternion.identity, WheelColliders.RearLeftWheel.transform)
                .GetComponent<TrailRenderer>();
        }
    }

    private void CheckParticles()
    {
        WheelHit[] wheelHits = new WheelHit[4];
        WheelColliders.FrontRightWheel.GetGroundHit(out wheelHits[0]);
        WheelColliders.FrontLeftWheel.GetGroundHit(out wheelHits[1]);

        WheelColliders.RearRightWheel.GetGroundHit(out wheelHits[2]);
        WheelColliders.RearLeftWheel.GetGroundHit(out wheelHits[3]);

        float slipAllowance = 0.2f;
        if ((Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance))
        {
            WheelParticles.FRWheel.Play();
            WheelParticles.FRWheelTrail.emitting = true;
        }
        else
        {
            WheelParticles.FRWheel.Stop();

            WheelParticles.FRWheelTrail.emitting = false;
        }
        if ((Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance))
        {
            WheelParticles.FLWheel.Play();

            WheelParticles.FLWheelTrail.emitting = true;
        }
        else
        {
            WheelParticles.FLWheel.Stop();

            WheelParticles.FLWheelTrail.emitting = false;
        }
        if ((Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance))
        {
            WheelParticles.RRWheel.Play();

            WheelParticles.RRWheelTrail.emitting = true;
        }
        else
        {
            WheelParticles.RRWheel.Stop();

            WheelParticles.RRWheelTrail.emitting = false;
        }
        if ((Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance))
        {
            WheelParticles.RLWheel.Play();

            WheelParticles.RLWheelTrail.emitting = true;
        }
        else
        {
            WheelParticles.RLWheel.Stop();

            WheelParticles.RLWheelTrail.emitting = false;
        }
    }

    private void Update()
    {
        _currentMoveSpeed = _carRigidbody.velocity.magnitude;

        CheckInput();
        ApplyWheelPositions();
        ApplyMotorPower();
        ApplyBrake();
        ApplySteeringSystem();
        CheckParticles();
    }

    private void ApplyMotorPower()
    {
        WheelColliders.RearLeftWheel.motorTorque = MotorPower * GasInput;
        WheelColliders.RearRightWheel.motorTorque = MotorPower * GasInput;
    }

    private void ApplySteeringSystem()
    {
        float steeringAngle = SteeringInput * SteeringCurve.Evaluate(_currentMoveSpeed);

        if (SlipAngle < 120f)
        {
            steeringAngle += Vector3.SignedAngle(transform.forward, _carRigidbody.velocity + transform.forward, Vector3.up);
        }
        steeringAngle = Mathf.Clamp(steeringAngle, -90f, 90f);

        WheelColliders.FrontLeftWheel.steerAngle = steeringAngle;
        WheelColliders.FrontRightWheel.steerAngle = steeringAngle;
    }

    private void CheckInput()
    {
        GasInput = Input.GetAxis("Vertical");
        SteeringInput = Input.GetAxis("Horizontal");

        SlipAngle = Vector3.Angle(transform.forward, _carRigidbody.velocity - transform.forward);

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
[System.Serializable]
public class WheelParticles
{
    public ParticleSystem FRWheel;
    public ParticleSystem FLWheel;
    public ParticleSystem RRWheel;
    public ParticleSystem RLWheel;

    public TrailRenderer FRWheelTrail;
    public TrailRenderer FLWheelTrail;
    public TrailRenderer RRWheelTrail;
    public TrailRenderer RLWheelTrail;

}
