using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.Barracuda;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;

public class walkerAgent : Agent
{


    //The walking speed to try and achieve
    private float m_TargetWalkingSpeed = m_maxWalkingSpeed; // целевая скорость к которой агент должен стремиться. Используется только в сценариях, где агент должен перемещаться

    const float m_maxWalkingSpeed = 15; // Максимальная скорость которую агенту разрешено развивать

    //The current target walking speed. Clamped because a value of zero will cause NaNs
    public float TargetWalkingSpeed
    {
        get { return m_TargetWalkingSpeed; }
        set { m_TargetWalkingSpeed = Mathf.Clamp(value, .1f, m_maxWalkingSpeed); }
    }

    //Should the agent sample a new goal velocity each episode?
    //If true, TargetWalkingSpeed will be randomly set between 0.1 and m_maxWalkingSpeed in OnEpisodeBegin()
    //If false, the goal velocity will be m_maxWalkingSpeed
    private bool m_RandomizeWalkSpeedEachEpisode; 

    //The direction an agent will walk during training.
    [Header("Target To Walk Towards")] public Transform dynamicTargetPrefab; //Target prefab to use in Dynamic envs
    public Transform staticTargetPrefab; //Target prefab to use in Static envs
    private Transform m_Target; //Target the agent will walk towards during training.

    //This will be used as a stabilized model space reference point for observations
    //Because ragdolls can move erratically during training, using a stabilized reference transform improves learning
    OrientationCubeController m_OrientationCube; // объект который всегда направлен в сторону цели

    [Header("Body Parts")] [Space(10)] public Transform body; // Transform - компонента которая хранит об объекте инфу о положении, поворотах и скейле.
    //body ссылается на параллелепипид тела
    public Transform joint1; // самый верхний сустав первой ноги который прикрпелён к телу
    public Transform leg1; // самая первая кость первой ноги которая цепляется к первому суставу
    public Transform foreJoint1; // коленный сустав первой ноги которая цепляется к первой кости
    public Transform foreLeg1; // вторая кость первой ноги которая цепляется к коленному суставу
    public Transform footJoint1; // сустав который связывает вторую кость и ступню
    public Transform foot1; // кость ступни
    public Transform foreFootJoint1; // наконечник на кости ступни
    public Transform joint2; // аналогично для ног 2, 3 и 4
    public Transform leg2;
    public Transform foreJoint2;
    public Transform foreLeg2;
    public Transform footJoint2;
    public Transform foot2;
    public Transform foreFootJoint2;
    public Transform joint3;
    public Transform leg3;
    public Transform foreJoint3;
    public Transform foreLeg3;
    public Transform footJoint3;
    public Transform foot3;
    public Transform foreFootJoint3;
    public Transform joint4;
    public Transform leg4;
    public Transform foreJoint4;
    public Transform foreLeg4;
    public Transform footJoint4;
    public Transform foot4;
    public Transform foreFootJoint4;
    /*public Transform finger1;
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

    public GameObject head; // объект головы. Использовался чтобы понять, куда агент смотрит

    JointDriveController m_JdController; // юнитёвый компонент управления суставами.

    public bool touched; // true, если агент дотрагивается до цели

    public bool posing; // true, если агент должен принимать какую-то определённую позу

    
    public override void Initialize() // функция которая вызывается в начале запуска эксперимента. Устанавливает связи, инициализирует таргеты итп
    {/*
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
        gobj.GetComponent<TargetController>().m_startingPos = transform.position;*/

        SpawnTarget(dynamicTargetPrefab, transform.position);
        m_OrientationCube = GetComponentInChildren<OrientationCubeController>();

        //head = Instantiate(head);

        angle = 0.0f;
        speed = 1.0f;

        m_JdController = GetComponent<JointDriveController>();

        //Setup each body part
        m_JdController.SetupBodyPart(body);
        m_JdController.SetupBodyPart(joint1);
        m_JdController.SetupBodyPart(leg1);
        m_JdController.SetupBodyPart(foreJoint1);
        m_JdController.SetupBodyPart(foreLeg1);
        m_JdController.SetupBodyPart(footJoint1);
        m_JdController.SetupBodyPart(foot1);
        m_JdController.SetupBodyPart(foreFootJoint1);
        m_JdController.SetupBodyPart(joint2);
        m_JdController.SetupBodyPart(leg2);
        m_JdController.SetupBodyPart(foreJoint2);
        m_JdController.SetupBodyPart(foreLeg2);
        m_JdController.SetupBodyPart(footJoint2);
        m_JdController.SetupBodyPart(foot2);
        m_JdController.SetupBodyPart(foreFootJoint2);
        m_JdController.SetupBodyPart(joint3);
        m_JdController.SetupBodyPart(leg3);
        m_JdController.SetupBodyPart(foreJoint3);
        m_JdController.SetupBodyPart(foreLeg3);
        m_JdController.SetupBodyPart(footJoint3);
        m_JdController.SetupBodyPart(foot3);
        m_JdController.SetupBodyPart(foreFootJoint3);
        m_JdController.SetupBodyPart(joint4);
        m_JdController.SetupBodyPart(leg4);
        m_JdController.SetupBodyPart(foreJoint4);
        m_JdController.SetupBodyPart(foreLeg4);
        m_JdController.SetupBodyPart(footJoint4);
        m_JdController.SetupBodyPart(foot4);
        m_JdController.SetupBodyPart(foreFootJoint4);
        /*
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


    void SpawnTarget(Transform prefab, Vector3 pos) // спавнит целевой кубик
    {
        m_Target = Instantiate(prefab, pos, Quaternion.identity, transform);
        //m_Target.GetComponent<TargetController>().walker_agent = this;
    }

    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor) // для заданной части тела собирает его наблюдения
    {
        //GROUND CHECK
        sensor.AddObservation(bp.groundContact.touchingGround); // Is this bp touching the ground

        if (bp.rb.transform != body)
        {
            sensor.AddObservation(bp.currentStrength / m_JdController.maxJointForceLimit);
        }
    }

    public override void OnEpisodeBegin() // функция запускается в начале каждого эпизода(эпизод длится 4000 шагов). НЕ РАВНО ЭПИЗОДАМ ОБУЧЕНИЯ
    {
        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }

        //Random start rotation to help generalize
        body.position = gameObject.transform.position + new Vector3(0, 1, 0);
        body.rotation = Quaternion.Euler(0, 0, 0);

        UpdateOrientationObjects();

        //Set our goal walking speed
        TargetWalkingSpeed =
            m_RandomizeWalkSpeedEachEpisode ? Random.Range(0.1f, m_maxWalkingSpeed) : TargetWalkingSpeed;
    }

    public int i1, i2;

    /// <summary>
    /// Loop over body parts to add them to observation.
    /// </summary>
    public override void CollectObservations(VectorSensor sensor) // функция которая собирает сразу все наблюдения для агентов
    {
        var cubeForward = m_OrientationCube.transform.forward;

        //velocity we want to match
        var velGoal = cubeForward * TargetWalkingSpeed;
        //ragdoll's avg vel
        var avgVel = GetAvgVelocity();

        //current ragdoll velocity. normalized
        sensor.AddObservation(Vector3.Distance(velGoal, avgVel));
        //avg body vel relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(avgVel));
        //vel goal relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(velGoal));
        //rotation delta
        sensor.AddObservation(Quaternion.FromToRotation(body.right, cubeForward));

        //Add pos of target relative to orientation cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformPoint(m_Target.transform.position));

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
    }

    //public Vector3 previous_target_pos, next_target_pos;

    /* public float action1, action2, action3, action4, action5;
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
    */
    public bool test;
    public override void OnActionReceived(ActionBuffers actionBuffers) // функция вызывается каждый раз когда агент выполняет действия (в нашем случае зачастую каждый шаг)
    {
        var bpDict = m_JdController.bodyPartsDict;
        if (!test)
        {
            // The dictionary with all the body parts in it are in the jdController

            var continuousActions = actionBuffers.ContinuousActions;
            var i = -1;

            bpDict[joint1].SetJointTargetRotation(continuousActions[++i], 0, continuousActions[++i]);
            bpDict[joint3].SetJointTargetRotation(continuousActions[i], 0, continuousActions[i]);
            bpDict[foreJoint1].SetJointTargetRotation(0, 0, continuousActions[++i]);
            bpDict[foreJoint3].SetJointTargetRotation(0, 0, continuousActions[i]);
            bpDict[footJoint1].SetJointTargetRotation(0, 0, continuousActions[++i]);
            bpDict[footJoint3].SetJointTargetRotation(0, 0, continuousActions[i]);
            //bpDict[foot1].SetJointTargetRotation(continuousActions[++i], 0, 0);
            bpDict[joint2].SetJointTargetRotation(continuousActions[++i], 0, continuousActions[++i]);
            bpDict[joint4].SetJointTargetRotation(continuousActions[i], 0, continuousActions[i]);
            bpDict[foreJoint2].SetJointTargetRotation(0, 0, continuousActions[++i]);
            bpDict[foreJoint4].SetJointTargetRotation(0, 0, continuousActions[i]);
            bpDict[footJoint2].SetJointTargetRotation(0, 0, continuousActions[++i]);
            bpDict[footJoint4].SetJointTargetRotation(0, 0, continuousActions[i]);
            //bpDict[foot3].SetJointTargetRotation(continuousActions[++i], 0, 0);
            //bpDict[foot4].SetJointTargetRotation(continuousActions[++i], 0, 0);

            bpDict[joint1].SetJointStrength(continuousActions[++i]);
            bpDict[joint3].SetJointStrength(continuousActions[i]);
            bpDict[foreJoint1].SetJointStrength(continuousActions[++i]);
            bpDict[foreJoint3].SetJointStrength(continuousActions[i]);
            bpDict[footJoint1].SetJointStrength(continuousActions[++i]);
            bpDict[footJoint3].SetJointStrength(continuousActions[i]);
            //bpDict[foot1].SetJointStrength(continuousActions[++i]);
            bpDict[joint2].SetJointStrength(continuousActions[++i]);
            bpDict[joint4].SetJointStrength(continuousActions[i]);
            bpDict[foreJoint2].SetJointStrength(continuousActions[++i]);
            bpDict[foreJoint4].SetJointStrength(continuousActions[i]);
            bpDict[footJoint2].SetJointStrength(continuousActions[++i]);
            bpDict[footJoint4].SetJointStrength(continuousActions[i]);
            //bpDict[foot2].SetJointStrength(continuousActions[++i]);
            //bpDict[foot3].SetJointStrength(continuousActions[++i]);
            //bpDict[foot4].SetJointStrength(continuousActions[++i]);
        }
        else
        {
            bpDict[joint4].SetJointTargetRotation(angle, 0, 0);
            bpDict[joint4].SetJointStrength(1.0f);

        }
    }

    public float angle, speed;/*
    private void FixedUpdate()
    {
        angle += speed * Time.fixedDeltaTime;
        if(angle > 1.0f)
        {
            speed = -1.0f;
        } else if (angle < -1.0f)
        {
            speed = 1.0f;
        }
    }*/

    public override void Heuristic(in ActionBuffers actionsOut) // функция которая используется для поведения агента, если агент не учится и у него нет обученных поведений
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
        for (int i = 0; i < 14; i++)
        {
            continuousActionsOut[i] = Random.Range(-1f, 1f);
            //Debug.Log(continuousActionsOut[i]);
        }
    }
    void FixedUpdate() // функция вызывается каждый тик среды. Тут мы вычисляем и добавляем награду.
    {
        //head.transform.position = body.position + body.right;
        angle += speed * Time.fixedDeltaTime;
        if (angle > 1.0f)
        {
            speed = -1.0f;
        }
        else if (angle < -1.0f)
        {
            speed = 1.0f;
        }
        UpdateOrientationObjects();

        var cubeForward = m_OrientationCube.transform.forward;

        // Set reward for this step according to mixture of the following elements.
        // a. Match target speed
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        var matchSpeedReward = GetMatchingVelocityReward(cubeForward * TargetWalkingSpeed, GetAvgVelocity());

        // b. Rotation alignment with target direction.
        //This reward will approach 1 if it faces the target direction perfectly and approach zero as it deviates
        var lookAtTargetReward = (Vector3.Dot(cubeForward, body.right) + 1) * 0.5f;

        AddReward(matchSpeedReward * lookAtTargetReward); // эта функция даёт агенту награду.
    }
    void UpdateOrientationObjects()
    {
        m_OrientationCube.UpdateOrientation(body, m_Target);
        /*if (m_DirectionIndicator)
        {
            m_DirectionIndicator.MatchOrientation(m_OrientationCube.transform);
        }*/
    }
    public float GetMatchingVelocityReward(Vector3 velocityGoal, Vector3 actualVelocity)
    {
        //distance between our actual velocity and goal velocity
        var velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(actualVelocity, velocityGoal), 0, TargetWalkingSpeed);

        //return the value on a declining sigmoid shaped curve that decays from 1 to 0
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        return Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / TargetWalkingSpeed, 2), 2);
    }
    public void TouchedTarget()
    {
        AddReward(1f);
        EndEpisode();
        Debug.Log("wow");
    }

    Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;
        Vector3 avgVel = Vector3.zero;

        //ALL RBS
        int numOfRb = 0;
        foreach (var item in m_JdController.bodyPartsList)
        {
            numOfRb++;
            velSum += item.rb.velocity;
        }

        avgVel = velSum / numOfRb;
        return avgVel;
    }
    /*
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
        bpDict[forePhalanx4].SetJointTargetRotation(curGrip, 0, 0);

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

        if (pose0Time < Time.time)
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
                if (bodyPart.rb.transform != turner)
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
        }
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
        }/*


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
            }
        }
        else
        {
            //AddReward(Mathf.Exp(-(next_target_pos - hand.position).magnitude));// + Mathf.Exp(Mathf.Cos(hand.rotation.x)) / 1000f + Mathf.Exp(Mathf.Cos(hand.rotation.z)) / 1000f);
        }
        /*
        Vector3 rot_target = gobj.transform.position - turner.position;

        rot_target.y = 0;

        reward += Mathf.Exp(-Vector3.Angle(rot_target, -turner.forward));

        AddReward(reward);
    }
*/
}