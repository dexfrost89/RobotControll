using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.Barracuda;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;

public class ArmAgent : Agent
{

    [Header("Body Parts")] [Space(10)] public Transform arm_base;
    public Transform turner;
    public Transform bone1;
    public Transform bone2;
    public Transform bone3;
    public Transform hand;
    public Transform finger1;
    public Transform phalanx1;
    public Transform forePhalanx1;
    public Transform finger2;
    public Transform phalanx2;
    public Transform forePhalanx2;
    public Transform finger3;
    public Transform phalanx3;
    public Transform forePhalanx3;
    public Transform finger4;
    public Transform phalanx4;
    public Transform forePhalanx4;

    JointDriveController m_JdController;
    GameObject gobj;

    public TargetController tc;



    public override void Initialize()
    {
        curangle = 0.0f;
        gobj = Instantiate(tc.gameObject, transform);
        gobj.GetComponent<TargetController>().m_startingPos = transform.position;

        m_JdController = GetComponent<JointDriveController>();

        //Setup each body part
        m_JdController.SetupBodyPart(arm_base);
        m_JdController.SetupBodyPart(turner);
        m_JdController.SetupBodyPart(bone1);
        m_JdController.SetupBodyPart(bone2);
        m_JdController.SetupBodyPart(bone3);
        m_JdController.SetupBodyPart(hand);
        m_JdController.SetupBodyPart(finger1);
        m_JdController.SetupBodyPart(phalanx1);
        m_JdController.SetupBodyPart(forePhalanx1);
        m_JdController.SetupBodyPart(finger2);
        m_JdController.SetupBodyPart(phalanx2);
        m_JdController.SetupBodyPart(forePhalanx2);
        m_JdController.SetupBodyPart(finger3);
        m_JdController.SetupBodyPart(phalanx3);
        m_JdController.SetupBodyPart(forePhalanx3);
        m_JdController.SetupBodyPart(finger4);
        m_JdController.SetupBodyPart(phalanx4);
        m_JdController.SetupBodyPart(forePhalanx4);
    }


    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor)
    {
        if (bp.rb.transform != arm_base)
        {
            sensor.AddObservation(bp.currentStrength / m_JdController.maxJointForceLimit);
        }
    }

    public int i1, i2;

    /// <summary>
    /// Loop over body parts to add them to observation.
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(gobj.transform.position - hand.position);

        foreach (var bodyPart in m_JdController.bodyPartsList)
        {
            CollectObservationBodyPart(bodyPart, sensor);
        }
    }


    public override void OnEpisodeBegin()
    {
        gobj.GetComponent<TargetController>().MoveTargetToRandomPosition();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // The dictionary with all the body parts in it are in the jdController
        var bpDict = m_JdController.bodyPartsDict;

        var continuousActions = actionBuffers.ContinuousActions;
        var i = -1;
        // Pick a new target joint rotation
        bpDict[turner].SetJointTargetRotation(0, curangle, 0);
        bpDict[bone1].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[bone2].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[bone3].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[hand].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[finger1].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[finger2].SetJointTargetRotation(continuousActions[i], 0, 0);
        bpDict[finger3].SetJointTargetRotation(continuousActions[i], 0, 0);
        bpDict[finger4].SetJointTargetRotation(continuousActions[i], 0, 0);
        bpDict[phalanx1].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[phalanx2].SetJointTargetRotation(continuousActions[i], 0, 0);
        bpDict[phalanx3].SetJointTargetRotation(continuousActions[i], 0, 0);
        bpDict[phalanx4].SetJointTargetRotation(continuousActions[i], 0, 0);
        bpDict[forePhalanx1].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[forePhalanx2].SetJointTargetRotation(continuousActions[i], 0, 0);
        bpDict[forePhalanx3].SetJointTargetRotation(continuousActions[i], 0, 0);
        bpDict[forePhalanx4].SetJointTargetRotation(continuousActions[i], 0, 0);

        // Update joint strength
        bpDict[turner].SetJointStrength(continuousActions[++i]);
        bpDict[bone1].SetJointStrength(continuousActions[++i]);
        bpDict[bone2].SetJointStrength(continuousActions[++i]);
        bpDict[bone3].SetJointStrength(continuousActions[++i]);
        bpDict[hand].SetJointStrength(continuousActions[++i]);
        bpDict[finger1].SetJointStrength(continuousActions[++i]);
        bpDict[finger2].SetJointStrength(continuousActions[i]);
        bpDict[finger3].SetJointStrength(continuousActions[i]);
        bpDict[finger4].SetJointStrength(continuousActions[i]);
        bpDict[phalanx1].SetJointStrength(continuousActions[++i]);
        bpDict[phalanx2].SetJointStrength(continuousActions[i]);
        bpDict[phalanx3].SetJointStrength(continuousActions[i]);
        bpDict[phalanx4].SetJointStrength(continuousActions[i]);
        bpDict[forePhalanx1].SetJointStrength(continuousActions[++i]);
        bpDict[forePhalanx2].SetJointStrength(continuousActions[i]);
        bpDict[forePhalanx3].SetJointStrength(continuousActions[i]);
        bpDict[forePhalanx4].SetJointStrength(continuousActions[i]);

        curangle += speed * Time.fixedDeltaTime;
        if(curangle > 1.0f) {
            speed *= -1.0f;
        } else if(curangle < -1.0f) {
            speed *= -1.0f;
        }
    }



    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        for(int i = 0; i < 17; i++)
            if(i > 8)
                continuousActionsOut[i] = 1.0f;
            else
                continuousActionsOut[i] = -0.9f;//Random.Range(-1f, 1f);
    }

    public float speed;
    public float curangle;

    void FixedUpdate()
    {
        AddReward(Mathf.Exp(-(gobj.transform.position - hand.position).magnitude));
    }

}
