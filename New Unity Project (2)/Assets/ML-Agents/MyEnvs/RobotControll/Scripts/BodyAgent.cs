using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BodyAgent : Agent
{
    public LegAgent leg1, leg2, leg3, leg4;
    public float RotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        RotationSpeed = 0.0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.rotation.x);
        sensor.AddObservation(transform.rotation.y);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(transform.rotation.w);

        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(transform.position.z);

        sensor.AddObservation(leg1.Joint1.transform.rotation.x);
        sensor.AddObservation(leg1.Joint1.transform.rotation.y);
        sensor.AddObservation(leg1.Joint1.transform.rotation.z);
        sensor.AddObservation(leg1.Joint1.transform.rotation.w);

        sensor.AddObservation(leg1.Joint2.transform.rotation.x);
        sensor.AddObservation(leg1.Joint2.transform.rotation.y);
        sensor.AddObservation(leg1.Joint2.transform.rotation.z);
        sensor.AddObservation(leg1.Joint2.transform.rotation.w);

        sensor.AddObservation(leg1.Bone1.transform.rotation.x);
        sensor.AddObservation(leg1.Bone1.transform.rotation.y);
        sensor.AddObservation(leg1.Bone1.transform.rotation.z);
        sensor.AddObservation(leg1.Bone1.transform.rotation.w);

        sensor.AddObservation(leg1.Bone2.transform.rotation.x);
        sensor.AddObservation(leg1.Bone2.transform.rotation.y);
        sensor.AddObservation(leg1.Bone2.transform.rotation.z);
        sensor.AddObservation(leg1.Bone2.transform.rotation.w);

        sensor.AddObservation(leg2.Joint1.transform.rotation.x);
        sensor.AddObservation(leg2.Joint1.transform.rotation.y);
        sensor.AddObservation(leg2.Joint1.transform.rotation.z);
        sensor.AddObservation(leg2.Joint1.transform.rotation.w);

        sensor.AddObservation(leg2.Joint2.transform.rotation.x);
        sensor.AddObservation(leg2.Joint2.transform.rotation.y);
        sensor.AddObservation(leg2.Joint2.transform.rotation.z);
        sensor.AddObservation(leg2.Joint2.transform.rotation.w);

        sensor.AddObservation(leg2.Bone1.transform.rotation.x);
        sensor.AddObservation(leg2.Bone1.transform.rotation.y);
        sensor.AddObservation(leg2.Bone1.transform.rotation.z);
        sensor.AddObservation(leg2.Bone1.transform.rotation.w);

        sensor.AddObservation(leg2.Bone2.transform.rotation.x);
        sensor.AddObservation(leg2.Bone2.transform.rotation.y);
        sensor.AddObservation(leg2.Bone2.transform.rotation.z);
        sensor.AddObservation(leg2.Bone2.transform.rotation.w);

        sensor.AddObservation(leg3.Joint1.transform.rotation.x);
        sensor.AddObservation(leg3.Joint1.transform.rotation.y);
        sensor.AddObservation(leg3.Joint1.transform.rotation.z);
        sensor.AddObservation(leg3.Joint1.transform.rotation.w);

        sensor.AddObservation(leg3.Joint2.transform.rotation.x);
        sensor.AddObservation(leg3.Joint2.transform.rotation.y);
        sensor.AddObservation(leg3.Joint2.transform.rotation.z);
        sensor.AddObservation(leg3.Joint2.transform.rotation.w);

        sensor.AddObservation(leg3.Bone1.transform.rotation.x);
        sensor.AddObservation(leg3.Bone1.transform.rotation.y);
        sensor.AddObservation(leg3.Bone1.transform.rotation.z);
        sensor.AddObservation(leg3.Bone1.transform.rotation.w);

        sensor.AddObservation(leg3.Bone2.transform.rotation.x);
        sensor.AddObservation(leg3.Bone2.transform.rotation.y);
        sensor.AddObservation(leg3.Bone2.transform.rotation.z);
        sensor.AddObservation(leg3.Bone2.transform.rotation.w);

        sensor.AddObservation(leg4.Joint1.transform.rotation.x);
        sensor.AddObservation(leg4.Joint1.transform.rotation.y);
        sensor.AddObservation(leg4.Joint1.transform.rotation.z);
        sensor.AddObservation(leg4.Joint1.transform.rotation.w);

        sensor.AddObservation(leg4.Joint2.transform.rotation.x);
        sensor.AddObservation(leg4.Joint2.transform.rotation.y);
        sensor.AddObservation(leg4.Joint2.transform.rotation.z);
        sensor.AddObservation(leg4.Joint2.transform.rotation.w);

        sensor.AddObservation(leg4.Bone1.transform.rotation.x);
        sensor.AddObservation(leg4.Bone1.transform.rotation.y);
        sensor.AddObservation(leg4.Bone1.transform.rotation.z);
        sensor.AddObservation(leg4.Bone1.transform.rotation.w);

        sensor.AddObservation(leg4.Bone2.transform.rotation.x);
        sensor.AddObservation(leg4.Bone2.transform.rotation.y);
        sensor.AddObservation(leg4.Bone2.transform.rotation.z);
        sensor.AddObservation(leg4.Bone2.transform.rotation.w);
    }



    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Quaternion newRot;
        int i1 = 0;
        
        newRot = leg1.Joint1.transform.rotation;
        newRot[0] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg1.Joint1.transform.rotation = newRot;

        newRot = leg1.Joint2.transform.rotation;
        newRot[0] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg1.Joint2.transform.rotation = newRot;
        
        newRot = leg1.Bone1.transform.rotation;
        newRot[1] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg1.Joint1.transform.rotation = newRot;

        newRot = leg1.Bone2.transform.rotation;
        newRot[1] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg1.Joint2.transform.rotation = newRot;
        
        newRot = leg2.Joint1.transform.rotation;
        newRot[0] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg2.Joint1.transform.rotation = newRot;

        newRot = leg2.Joint2.transform.rotation;
        newRot[0] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg2.Joint2.transform.rotation = newRot;
        
        newRot = leg2.Bone1.transform.rotation;
        newRot[1] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg2.Joint1.transform.rotation = newRot;

        newRot = leg2.Bone2.transform.rotation;
        newRot[1] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg2.Joint2.transform.rotation = newRot;
        
        newRot = leg3.Joint1.transform.rotation;
        newRot[0] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg3.Joint1.transform.rotation = newRot;

        newRot = leg3.Joint2.transform.rotation;
        newRot[0] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg3.Joint2.transform.rotation = newRot;
        
        newRot = leg3.Bone1.transform.rotation;
        newRot[1] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg3.Joint1.transform.rotation = newRot;

        newRot = leg4.Bone2.transform.rotation;
        newRot[1] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg4.Joint2.transform.rotation = newRot;
        
        newRot = leg4.Joint1.transform.rotation;
        newRot[0] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg4.Joint1.transform.rotation = newRot;

        newRot = leg4.Joint2.transform.rotation;
        newRot[0] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg4.Joint2.transform.rotation = newRot;
        
        newRot = leg4.Bone1.transform.rotation;
        newRot[1] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg4.Joint1.transform.rotation = newRot;

        newRot = leg4.Bone2.transform.rotation;
        newRot[1] += actionBuffers.ContinuousActions[i1++] * RotationSpeed * Time.fixedDeltaTime;
        leg4.Joint2.transform.rotation = newRot;

        SetReward(transform.position.y - 1.1f);
    }



    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        int i1 = 0;
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
        continuousActionsOut[i1++] = Random.Range(0f, 1f);
    }
}
