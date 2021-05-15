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
    public Transform hand;/*
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
    public Transform forePhalanx4;*/

    JointDriveController m_JdController;
    GameObject gobj;

    public TargetController tc;

    public bool touched;

    public bool posing;


    public override void Initialize()
    {
        action1 = -1f;
        action2 = -1f;
        action3 = -1f;
        action4 = -1f;

        pose0Time = Time.time + 10000;
        pose1Time = Time.time + 10000;
        pose2Time = Time.time + 10000;
        pose3Time = Time.time + 10000;
        pose4Time = Time.time + 10000;
        curangle = 0.0f;
        curGrip = 1f;
        gobj = Instantiate(tc.gameObject, transform);
        gobj.GetComponent<TargetController>().m_startingPos = transform.position;

        m_JdController = GetComponent<JointDriveController>();

        //Setup each body part
        m_JdController.SetupBodyPart(arm_base);
        m_JdController.SetupBodyPart(turner);
        m_JdController.SetupBodyPart(bone1);
        m_JdController.SetupBodyPart(bone2);
        m_JdController.SetupBodyPart(bone3);
        m_JdController.SetupBodyPart(hand);/*
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
        m_JdController.SetupBodyPart(forePhalanx4);*/
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
        if (touched)
        {
            sensor.AddObservation(next_target_pos - hand.position);
        }
        else
        {
            sensor.AddObservation(previous_target_pos - hand.position);
        }

        foreach (var bodyPart in m_JdController.bodyPartsList)
        {
            CollectObservationBodyPart(bodyPart, sensor);
        }
    }

    public Vector3 previous_target_pos, next_target_pos;

    public float action1, action2, action3, action4, action5;
    public override void OnEpisodeBegin()
    {

        foreach (var bodyPart in m_JdController.bodyPartsList)
        {
            bodyPart.rb.isKinematic = false;
        }
        pose0Time = Time.time + 10000;
        pose1Time = Time.time + 10000;
        pose2Time = Time.time + 10000;
        pose3Time = Time.time + 10000;
        pose4Time = Time.time + 10000;
        //poseTime = Time.time + 5;
        touched = false;
        gobj.GetComponent<TargetController>().MoveTargetToRandomPosition();
        gobj.GetComponent<TargetController>().myAgent = this;
        previous_target_pos = gobj.transform.position;
        next_target_pos = transform.position * 2 - previous_target_pos;
        next_target_pos.y = previous_target_pos.y;
    }


    public float curGrip;

    public void Done()
    {
        AddReward(3000.0f);

        EndEpisode();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // The dictionary with all the body parts in it are in the jdController
        var bpDict = m_JdController.bodyPartsDict;

        var continuousActions = actionBuffers.ContinuousActions;
        var i = -1;
        if (!touched)
        {
            // Pick a new target joint rotation
            /*bpDict[turner].SetJointTargetRotation(0, continuousActions[++i], 0);
            bpDict[bone1].SetJointTargetRotation(continuousActions[++i], 0, 0);
            bpDict[bone2].SetJointTargetRotation(continuousActions[++i], 0, 0);
            bpDict[bone3].SetJointTargetRotation(continuousActions[++i], 0, 0);
            bpDict[hand].SetJointTargetRotation(continuousActions[++i], 0, 0);*/
            /**/
            action1 = continuousActions[++i];
            action2 = continuousActions[++i];
            action3 = continuousActions[++i];
            action4 = continuousActions[++i];
            action5 = continuousActions[++i];

            bpDict[turner].SetJointTargetRotation(0, action1, 0);
            bpDict[bone1].SetJointTargetRotation(action2, 0, 0);
            bpDict[bone2].SetJointTargetRotation(action3, 0, 0);
            bpDict[bone3].SetJointTargetRotation(action4, 0, 0);
            bpDict[hand].SetJointTargetRotation(action5, 0, 0);
            //touched = true;
        }/*
                                                                           * 
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
        bpDict[finger1].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[finger2].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[finger3].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[finger4].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[phalanx1].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[phalanx2].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[phalanx3].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[phalanx4].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[forePhalanx1].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[forePhalanx2].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[forePhalanx3].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[forePhalanx4].SetJointTargetRotation(curGrip, 0, 0);*/

        // Update joint strength
        bpDict[turner].SetJointStrength(1);
        bpDict[bone1].SetJointStrength(1);
        bpDict[bone2].SetJointStrength(1);
        bpDict[bone3].SetJointStrength(1);
        bpDict[hand].SetJointStrength(1);/*
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
        bpDict[finger1].SetJointStrength(1);
        bpDict[finger2].SetJointStrength(1);
        bpDict[finger3].SetJointStrength(1);
        bpDict[finger4].SetJointStrength(1);
        bpDict[phalanx1].SetJointStrength(1);
        bpDict[phalanx2].SetJointStrength(1);
        bpDict[phalanx3].SetJointStrength(1);
        bpDict[phalanx4].SetJointStrength(1);
        bpDict[forePhalanx1].SetJointStrength(1);
        bpDict[forePhalanx2].SetJointStrength(1);
        bpDict[forePhalanx3].SetJointStrength(1);
        bpDict[forePhalanx4].SetJointStrength(1);*/

        curangle += speed * Time.fixedDeltaTime;
        /*if(curangle > 1.0f) {
            speed *= -1.0f;
        } else if(curangle < -1.0f) {
            speed *= -1.0f;
        }*/
    }



    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        /*
        for(int i = 0; i < 17; i++)
            if(i > 8)
                continuousActionsOut[i] = 1.0f;
            else
                continuousActionsOut[i] = -0.9f;//Random.Range(-1f, 1f);*/
        /*for (int i = 0; i < 17; i++)
            continuousActionsOut[i] = Random.Range(-1f, 1f);*/
        for (int i = 0; i < 5; i++)
            if (i > 5)
                continuousActionsOut[i] = 1.0f;
            else
                continuousActionsOut[i] = Random.Range(-1f, 1f);
    }

    public float speed;
    public float curangle;

    public void GoToRandomPose()
    {
        var bpDict = m_JdController.bodyPartsDict;
        
        bpDict[turner].SetJointTargetRotation(0, Random.Range(-1f, 1f), 0);
        bpDict[bone1].SetJointTargetRotation(Random.Range(-1f, 1f), 0, 0);
        bpDict[bone2].SetJointTargetRotation(Random.Range(-1f, 1f), 0, 0);
        bpDict[bone3].SetJointTargetRotation(Random.Range(-1f, 1f), 0, 0);
        bpDict[hand].SetJointTargetRotation(Random.Range(-1f, 1f), 0, 0);/*
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
        bpDict[finger1].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[finger2].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[finger3].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[finger4].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[phalanx1].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[phalanx2].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[phalanx3].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[phalanx4].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[forePhalanx1].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[forePhalanx2].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[forePhalanx3].SetJointTargetRotation(curGrip, 0, 0);
        bpDict[forePhalanx4].SetJointTargetRotation(curGrip, 0, 0);*/

        // Update joint strength
        bpDict[turner].SetJointStrength(0.1f);
        bpDict[bone1].SetJointStrength(0.1f);
        bpDict[bone2].SetJointStrength(0.1f);
        bpDict[bone3].SetJointStrength(0.1f);
        bpDict[hand].SetJointStrength(0.1f);
    }


    public void GetToPose1()
    {
        //action1 = 1.0f;
        action2 = -0.655f;
        action3 = 0.275f;
        action4 = -0.32f;

        var bpDict = m_JdController.bodyPartsDict;

        //bpDict[turner].SetJointTargetRotation(0, action1, 0);
        bpDict[bone1].SetJointTargetRotation(action2, 0, 0);
        bpDict[bone2].SetJointTargetRotation(action3, 0, 0);
        bpDict[bone3].SetJointTargetRotation(action4, 0, 0);
        bpDict[turner].SetJointStrength(-1);
    }
    public void GetToPose2()
    {
        action1 = 1f;
        turner.gameObject.SetActive(true);



        var bpDict = m_JdController.bodyPartsDict;

        bpDict[turner].SetJointTargetRotation(0, action1, 0);
        bpDict[turner].SetJointStrength(1);
    }
    public void GetToPose3()
    {
        gobj.GetComponent<TargetController>().untouched = false;
    }

    public float pose0Time, pose1Time, pose2Time, pose3Time, pose4Time;
    void FixedUpdate()
    {

        arm_base.rotation = Quaternion.Euler(0, 180, 0);

        if(pose0Time < Time.time)
        {
            pose0Time = Time.time + 10000;
            foreach (var bodyPart in m_JdController.bodyPartsList)
            {
                bodyPart.rb.isKinematic = true;
            }
        }
        if (pose1Time < Time.time)
        {

            GetToPose1();
            foreach (var bodyPart in m_JdController.bodyPartsList)
            {
                if(bodyPart.rb.transform != turner)
                    bodyPart.rb.isKinematic = false;
            }
            //Debug.Log(1);
            pose1Time = Time.time + 10000;
            pose2Time = Time.time + 3;
        }
        if (pose2Time < Time.time)
        {
            //Debug.Log(2);
            GetToPose2();
            turner.GetComponent<Rigidbody>().isKinematic = false;
            pose2Time = Time.time + 10000;
            pose3Time = Time.time + 3;
        }
        if (pose3Time < Time.time)
        {
            //Debug.Log(3);
            GetToPose3();
            pose3Time = Time.time + 10000;
            pose4Time = Time.time + 3;
        }
        if (pose4Time < Time.time)
        {
            //Debug.Log(4);
            GetToPose1();
            pose4Time = Time.time + 10000;
            touched = false;
        }
        /*if(Time.time >= poseTime)
        {
            GoToRandomPose();
            poseTime = Time.time + 5;
        }*/
        if (!touched)
        {
            if (Input.GetKey(KeyCode.S))
            {
                curGrip += speed * Time.fixedDeltaTime;
                action2 += speed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.W))
            {
                curGrip -= speed * Time.fixedDeltaTime;
                action2 -= speed * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                action1 += speed * Time.fixedDeltaTime;
                if (action1 > 1.0f)
                {
                    action1 = -1.0f;
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                action1 -= speed * Time.fixedDeltaTime;
                if (action1 < -1.0f)
                {
                    action1 = 1.0f;
                }
            }
            if (Input.GetKey(KeyCode.F))
            {
                action3 += speed * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.R))
            {
                action3 -= speed * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.G))
            {
                action4 += speed * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.T))
            {
                action4 -= speed * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                gobj.GetComponent<TargetController>().untouched = false;
            }
        }
        else
        {

        }
        action1 = Mathf.Clamp(action1, -1.0f, 1.0f);
        action2 = Mathf.Clamp(action2, -1.0f, 1.0f);
        action3 = Mathf.Clamp(action3, -1.0f, 1.0f);
        action4 = Mathf.Clamp(action4, -1.0f, 1.0f);
        /*
        float reward = 0;

        Vector3 target_pos = gobj.transform.position;
        target_pos.y = 4.5f;
        float magn = (target_pos - hand.position).magnitude;
        /*
        if(magn > 0.5f)
        {
            reward -= 1f;
        } else
        {
            reward += 1f;
        }

        /*if (hand.position.y > 4.5f || hand.position.y < 5f)
        {
            reward += 1f;
        }
        else
        {
            reward -= 1f;
        }/**/


        //reward += Mathf.Exp(-(target_pos - hand.position).magnitude);// + Mathf.Exp(Mathf.Cos(hand.rotation.x)) / 1000f + Mathf.Exp(Mathf.Cos(hand.rotation.z)) / 1000f);
        
        if (!touched)
        {
            Vector3 target_pos = gobj.transform.position;
            target_pos.y = 4.5f;
            //AddReward(Mathf.Exp(-(target_pos - hand.position).magnitude));
            /*if (hand.position.y > 4.5f)
            {
                // + Mathf.Exp(Mathf.Cos(hand.rotation.x)) / 1000f + Mathf.Exp(Mathf.Cos(hand.rotation.z)) / 1000f);
            } else
            {
               // AddReward(-1);
            }*/
        }
        else
        {
            //AddReward(Mathf.Exp(-(next_target_pos - hand.position).magnitude));// + Mathf.Exp(Mathf.Cos(hand.rotation.x)) / 1000f + Mathf.Exp(Mathf.Cos(hand.rotation.z)) / 1000f);
        }
        /*
        Vector3 rot_target = gobj.transform.position - turner.position;

        rot_target.y = 0;

        reward += Mathf.Exp(-Vector3.Angle(rot_target, -turner.forward));

        AddReward(reward);*/
    }

}
