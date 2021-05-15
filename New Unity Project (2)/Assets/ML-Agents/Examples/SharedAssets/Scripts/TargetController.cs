using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using UnityEngine.Events;

namespace Unity.MLAgentsExamples
{
    /// <summary>
    /// Utility class to allow target placement and collision detection with an agent
    /// Add this script to the target you want the agent to touch.
    /// Callbacks will be triggered any time the target is touched with a collider tagged as 'tagToDetect'
    /// </summary>
    public class TargetController : MonoBehaviour
    {
        [Header("Collider Tag To Detect")]
        public bool untouched;
        public string tagToDetect = "agent"; //collider tag to detect 

        [Header("Target Placement")]
        public float spawnRadius; //The radius in which a target can be randomly spawned.
        public bool respawnIfTouched; //Should the target respawn to a different position when touched

        [Header("Target Fell Protection")]
        public bool respawnIfFallsOffPlatform = true; //If the target falls off the platform, reset the position.
        public float fallDistance = 5; //distance below the starting height that will trigger a respawn 


        public Vector3 m_startingPos; //the starting position of the target
        private Agent m_agentTouching; //the agent currently touching the target

        [System.Serializable]
        public class TriggerEvent : UnityEvent<Collider>
        {
        }

        [Header("Trigger Callbacks")]
        public TriggerEvent onTriggerEnterEvent = new TriggerEvent();
        public TriggerEvent onTriggerStayEvent = new TriggerEvent();
        public TriggerEvent onTriggerExitEvent = new TriggerEvent();

        [System.Serializable]
        public class CollisionEvent : UnityEvent<Collision>
        {
        }

        [Header("Collision Callbacks")]
        public CollisionEvent onCollisionEnterEvent = new CollisionEvent();
        public CollisionEvent onCollisionStayEvent = new CollisionEvent();
        public CollisionEvent onCollisionExitEvent = new CollisionEvent();

        // Start is called before the first frame update
        void OnEnable()
        {
            m_startingPos = transform.position;
            if (respawnIfTouched)
            {
                //MoveTargetToRandomPosition();
            }
        }

        void Update()
        {
            if (respawnIfFallsOffPlatform)
            {
                if (transform.position.y < m_startingPos.y - fallDistance)
                {
                    //Debug.Log($"{transform.name} Fell Off Platform");
                    //MoveTargetToRandomPosition();
                }
            }
        }

        private void FixedUpdate()
        {

            if (untouched)
            {
                GetComponent<Rigidbody>().AddForce((myHand.position - new Vector3(0, 0.3f, 0) - transform.position) * 10);
                //Debug.Log((myHand.position - new Vector3(0, 0.1f, 0) - transform.position));
                //transform.position = myHand.position - new Vector3(0, 0.5f, 0);
            }
        }

        public ArmAgent myAgent;
        /// <summary>
        /// Moves target to a random position within specified radius.
        /// </summary>
        public void MoveTargetToRandomPosition()
        {/*
            var newTargetPos = m_startingPos + (Random.insideUnitSphere * spawnRadius);
            newTargetPos.y = m_startingPos.y;
            transform.position = newTargetPos;*/
            GetComponent<Rigidbody>().useGravity = true;
            untouched = false;
            var newTargetPos = new Vector3(Random.Range(-3f, -2.3f), 3.91f, Random.Range(-0.5f, 0.63f)) + m_startingPos;
            //var newTargetPos = new Vector3((-3f -2.3f) / 2f, 3.91f, (-0.5f + 0.63f) / 2f) + m_startingPos;
            transform.position = newTargetPos;
            //Debug.Log("done");
        }

        public Transform myHand;
        private void OnCollisionEnter(Collision col)
        {
            //Debug.Log(col.transform.name);
            if (col.transform.tag == "magnet")
            {
                //Debug.Log(col.transform.position);
                //untouched = true;
                //Debug.Log("Hey");
                //transform.parent = col.transform;
                //myHand = col.transform;
                //GetComponent<Rigidbody>().useGravity = false;
                //myAgent.touched = true;
                //myAgent.pose1Time = Time.time + 2;
            }
            if (col.transform.CompareTag(tagToDetect))
            {
                onCollisionEnterEvent.Invoke(col);
                if (respawnIfTouched)
                {
                    //MoveTargetToRandomPosition();
                }
            }
        }

        private void OnCollisionStay(Collision col)
        {
            if (untouched)
            {
                //GetComponent<Rigidbody>().AddForce((myHand.position - new Vector3(0, -1.0f, 0) - transform.position) * 1);
            }
            if (col.transform.CompareTag(tagToDetect))
            {
                onCollisionStayEvent.Invoke(col);
            }
        }

        private void OnCollisionExit(Collision col)
        {
            if (col.transform.CompareTag(tagToDetect))
            {
                onCollisionExitEvent.Invoke(col);
            }
        }

        public void SetHand(GameObject col)
        {

            untouched = true;
            myHand = col.transform;
            myAgent.touched = true;
            myAgent.pose1Time = Time.time + 2;
            myAgent.pose0Time = Time.time;
            //Debug.Log("done");
        }
        private void OnTriggerEnter(Collider col)
        {
            if (col.transform.tag == "magnet")
            {
                //Debug.Log(col.transform.position);
                //Debug.Log("Hey");
                //transform.parent = col.transform;
                //GetComponent<Rigidbody>().useGravity = false;
                //myAgent.touched = true;
                //myAgent.pose1Time = Time.time;
            }
            if (col.tag == "tray")
            {
                myAgent.Done();
            }
            if (col.CompareTag(tagToDetect))
            {
                onTriggerEnterEvent.Invoke(col);
            }
        }

        private void OnTriggerStay(Collider col)
        {
            if (untouched)
            {
                //GetComponent<Rigidbody>().AddForce((myHand.position - new Vector3(0, -1.0f, 0) - transform.position) * 1);
            }
            if (col.CompareTag(tagToDetect))
            {
                onTriggerStayEvent.Invoke(col);
            }
        }

        private void OnTriggerExit(Collider col)
        {
            if (col.CompareTag(tagToDetect))
            {
                onTriggerExitEvent.Invoke(col);
            }
        }
    }
}
