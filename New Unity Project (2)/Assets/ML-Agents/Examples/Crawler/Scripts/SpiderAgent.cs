using UnityEngine;
using Unity.MLAgents;
using Unity.Barracuda;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

[RequireComponent(typeof(JointDriveController))] // Required to set joint forces
public class SpiderAgent : Agent
{
    [Header("Body Parts")] [Space(10)] public Transform body;
    public Transform leg0Upper;
    public Transform leg0Lower;
    public Transform leg1Upper;
    public Transform leg1Lower;
    public Transform leg2Upper;
    public Transform leg2Lower;
    public Transform leg3Upper;
    public Transform leg3Lower;
    /*public Transform leg4Upper;
    public Transform leg4Lower;
    public Transform leg5Upper;
    public Transform leg5Lower;*/
    
    JointDriveController m_JdController;

    [Header("Foot Grounded Visualization")]
    [Space(10)]
    public bool useFootGroundedVisualization;

    public MeshRenderer foot0;
    public MeshRenderer foot1;
    public MeshRenderer foot2;
    public MeshRenderer foot3;
    //public MeshRenderer foot4;
    //public MeshRenderer foot5;
    public Material groundedMaterial;
    public Material unGroundedMaterial;

    public override void Initialize()
    {
        previousFeetPositions = new Vector3[4];
        previousFeetRotations = new Vector3[4];
        previousFeetPositions[0] = foot0.transform.position;
        previousFeetPositions[1] = foot1.transform.position;
        previousFeetPositions[2] = foot2.transform.position;
        previousFeetPositions[3] = foot3.transform.position;
        previousFeetRotations[0] = foot0.transform.rotation.eulerAngles;
        previousFeetRotations[1] = foot1.transform.rotation.eulerAngles;
        previousFeetRotations[2] = foot2.transform.rotation.eulerAngles;
        previousFeetRotations[3] = foot3.transform.rotation.eulerAngles;
        //previousFeetPositions[4] = foot4.transform.position;
        //previousFeetPositions[5] = foot5.transform.position;
        currentFeetPositions = new Vector3[4];
        currentFeetRotations = new Vector3[4];
        v_i_swing = new Vector3[4];
        d_fi = new Vector3[4];

        m_JdController = GetComponent<JointDriveController>();

        m_JdController.SetupBodyPart(body);
        m_JdController.SetupBodyPart(leg0Upper);
        m_JdController.SetupBodyPart(leg0Lower);
        m_JdController.SetupBodyPart(leg1Upper);
        m_JdController.SetupBodyPart(leg1Lower);
        m_JdController.SetupBodyPart(leg2Upper);
        m_JdController.SetupBodyPart(leg2Lower);
        m_JdController.SetupBodyPart(leg3Upper);
        m_JdController.SetupBodyPart(leg3Lower);
        /*m_JdController.SetupBodyPart(leg4Upper);
        m_JdController.SetupBodyPart(leg4Lower);
        m_JdController.SetupBodyPart(leg5Upper);
        m_JdController.SetupBodyPart(leg5Lower);*/
    }

    public override void OnEpisodeBegin()
    {
        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }

        body.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor)
    {
        sensor.AddObservation(bp.groundContact.touchingGround);

        sensor.AddObservation((bp.rb.transform.position - bp.previousPos) / Time.fixedDeltaTime);

        sensor.AddObservation(bp.rb.transform.position.y);

        Vector3 angles = bp.rb.transform.eulerAngles;
        angles.x = degree_to_rad(angles.x);
        angles.y = degree_to_rad(angles.y);
        angles.z = degree_to_rad(angles.z);

        sensor.AddObservation(angles);

        sensor.AddObservation(((angles.y - degree_to_rad(bp.previousRot.y)) % (2f * Mathf.PI)) / Time.fixedDeltaTime);

        if (bp.rb.transform != body)
        {
            sensor.AddObservation(bp.currentStrength / m_JdController.maxJointForceLimit);
        }
        bp.previousPos = bp.rb.transform.position;
        bp.previousRot = bp.rb.transform.rotation.eulerAngles;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        RaycastHit hit;
        float maxRaycastDist = 10;
        if (Physics.Raycast(body.position, Vector3.down, out hit, maxRaycastDist))
        {
            sensor.AddObservation(hit.distance / maxRaycastDist);
        }
        else
            sensor.AddObservation(1);

        foreach (var bodyPart in m_JdController.bodyPartsList)
        {
            CollectObservationBodyPart(bodyPart, sensor);
        }

        for(int i = 0; i < 4; i++) {
            sensor.AddObservation((currentFeetPositions[i] - previousFeetPositions[i]) / Time.fixedDeltaTime);
            sensor.AddObservation(currentFeetPositions[i].y);
            Vector3 angles = currentFeetRotations[i];
            angles.x = degree_to_rad(angles.x);
            angles.y = degree_to_rad(angles.y);
            angles.z = degree_to_rad(angles.z);
            sensor.AddObservation(angles);
            sensor.AddObservation(((angles.y - degree_to_rad(previousFeetRotations[i].y)) % (2f * Mathf.PI)) / Time.fixedDeltaTime);
        }

        previousFeetPositions[0] = foot0.transform.position;
        previousFeetPositions[1] = foot1.transform.position;
        previousFeetPositions[2] = foot2.transform.position;
        previousFeetPositions[3] = foot3.transform.position;
        previousFeetRotations[0] = foot0.transform.rotation.eulerAngles;
        previousFeetRotations[1] = foot1.transform.rotation.eulerAngles;
        previousFeetRotations[2] = foot2.transform.rotation.eulerAngles;
        previousFeetRotations[3] = foot3.transform.rotation.eulerAngles;
        //8 * (1 + 3 + 1 + 3 + 1 + 1) + (1 + 3 + 1 + 3 + 1) + 4 * (3 + 1 + 3 + 1) + 1
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var bpDict = m_JdController.bodyPartsDict;

        var continuousActions = actionBuffers.ContinuousActions;
        var i = -1;
        bpDict[leg0Upper].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[leg1Upper].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[leg2Upper].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[leg3Upper].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        //bpDict[leg4Upper].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        //bpDict[leg5Upper].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[leg0Lower].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[leg1Lower].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[leg2Lower].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[leg3Lower].SetJointTargetRotation(continuousActions[++i], 0, 0);
        //bpDict[leg4Lower].SetJointTargetRotation(continuousActions[++i], 0, 0);
        //bpDict[leg5Lower].SetJointTargetRotation(continuousActions[++i], 0, 0);

        bpDict[leg0Upper].SetJointStrength(continuousActions[++i]);
        bpDict[leg1Upper].SetJointStrength(continuousActions[++i]);
        bpDict[leg2Upper].SetJointStrength(continuousActions[++i]);
        bpDict[leg3Upper].SetJointStrength(continuousActions[++i]);
        //bpDict[leg4Upper].SetJointStrength(continuousActions[++i]);
        //bpDict[leg5Upper].SetJointStrength(continuousActions[++i]);
        bpDict[leg0Lower].SetJointStrength(continuousActions[++i]);
        bpDict[leg1Lower].SetJointStrength(continuousActions[++i]);
        bpDict[leg2Lower].SetJointStrength(continuousActions[++i]);
        bpDict[leg3Lower].SetJointStrength(continuousActions[++i]);
        //bpDict[leg4Lower].SetJointStrength(continuousActions[++i]);
        //bpDict[leg5Lower].SetJointStrength(continuousActions[++i]);
    }

    public Vector3 PreviousPosition, PreviousRotation;
    public Vector3[] previousFeetPositions, previousFeetRotations;

    private void FixedUpdate() {
        AddReward(r_walk(new Vector3(0, 0, 1.0f)));
        PreviousPosition = transform.position;
        PreviousRotation = transform.eulerAngles;
        currentFeetPositions[0] = foot0.transform.position;
        currentFeetPositions[1] = foot1.transform.position;
        currentFeetPositions[2] = foot2.transform.position;
        currentFeetPositions[3] = foot3.transform.position;
        currentFeetRotations[0] = foot0.transform.rotation.eulerAngles;
        currentFeetRotations[1] = foot1.transform.rotation.eulerAngles;
        currentFeetRotations[2] = foot2.transform.rotation.eulerAngles;
        currentFeetRotations[3] = foot3.transform.rotation.eulerAngles;
        //previousFeetPositions[4] = foot4.transform.position;
        //previousFeetPositions[5] = foot5.transform.position;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        for(int i = 0; i < 20; i++)
            continuousActionsOut[i] = Random.Range(-1f, 1f);
    }

    public float sinh(float x) {
        return (Mathf.Exp(x) - Mathf.Exp(-x)) / 2f;
    }

    public float cosh(float x) {
        return (Mathf.Exp(x) + Mathf.Exp(-x)) / 2f;
    }

    public float tanh(float x) {
        return sinh(x) / cosh(x);
    }

    public float atanh(float x) {
        return Mathf.Log((1f + x) / (1f - x)) / 2f;
    }

    public float w_from_m(float m) {
        return atanh(Mathf.Sqrt(0.95f)) / m;
    }

    public float c_from_vtm(float v, float t, float m) {
        return Mathf.Pow(tanh(Mathf.Abs((v - t) * w_from_m(m))), 2);
    }

    public float degree_to_rad(float x) {
        float result = x % 360.0f;
        if(result < -180f) {
            result += 360f;
        } else if (result > 180f) {
            result -= 360f;
        }

        return result / 360f * 2f * Mathf.PI;
    }

    public float r_up() {
        return 1.0f - c_from_vtm(Mathf.Sqrt(Mathf.Pow(degree_to_rad(body.eulerAngles.x), 2) + Mathf.Pow(degree_to_rad(body.eulerAngles.z), 2)), 0.0f, 0.4f);
    }

    public float r_still() {
        Vector3 current_position = transform.position;
        Vector3 v_xy = (current_position - PreviousPosition) / Time.fixedDeltaTime;
        v_xy.y = 0;
        return - v_xy.magnitude;
    }

    public float k_from_t() {
        Vector3 current_rotation = body.eulerAngles;

        float angle_difference = current_rotation.y - PreviousRotation.y;
        if(angle_difference > 180f) {
            angle_difference -= 360f;
        } else if (angle_difference < -180f) {
            angle_difference += 360f;
        }

        return 1.0f - c_from_vtm(degree_to_rad(angle_difference) / Time.fixedDeltaTime, 0.0f, 0.5f);
    }

    public float r_tot(float r) {
        return Mathf.Min(k_from_t() * r, r);
    }

    public float r_stand_upright() {
        return r_tot(r_still() + r_up());
    }

    public Vector3 previousF;
    public Vector3[] currentFeetPositions, currentFeetRotations, d_fi, v_i_swing;

    public float r_feet(Vector3 direction) {
        //v_i_swing

        //d_fi
        

        currentFeetPositions[0] = foot0.transform.position;
        currentFeetPositions[1] = foot1.transform.position;
        currentFeetPositions[2] = foot2.transform.position;
        currentFeetPositions[3] = foot3.transform.position;
        //currentFeetPositions[4] = foot4.transform.position;
        //currentFeetPositions[5] = foot5.transform.position;
        
        
        Vector3 currentF = new Vector3(0, 0, 0);
        float minimum = 100.0f;

        for(int i = 0; i < 4; i++) {
            d_fi[i] = (currentFeetPositions[i] - previousFeetPositions[i]) / Time.fixedDeltaTime;
            if(minimum > currentFeetPositions[i].y) {
                currentF = currentFeetPositions[i];
                minimum = currentFeetPositions[i].y;
            }
        }

        Vector3 d_F = (currentF - previousF) / Time.fixedDeltaTime;


        for(int i = 0; i < 4; i++) {
            v_i_swing[i] = d_fi[i] - d_F;
        }

        float result = 0.0f;

        for(int i = 0; i < 4; i++) {
            result += Vector3.Dot(direction, v_i_swing[i]);
        }

        previousF = currentF;

        return r_tot(result / 6.0f);
    }

    public float r_torso(Vector3 direction) {
        Vector3 current_position = transform.position;
        Vector3 v_xy = (current_position - PreviousPosition) / Time.fixedDeltaTime;
        
        return r_tot(Vector3.Dot(direction, v_xy));
    }

    public float r_walk(Vector3 direction) {
        //Debug.Log(r_torso(direction) + 0.5f * r_feet(direction) + 0.1f * r_up());
        return r_torso(direction) + 0.5f * r_feet(direction) + 0.1f * r_up();
    }

}
