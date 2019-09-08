using UnityEngine;
using UnityEngine.AI;
using VRStandardAssets.Maze;

namespace VRStandardAssets.Maze
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // Navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // Character we are controlling
        public Transform followCamera;
        public float followCameraOffset = 1.0f;
        public RectTransform startButton;
        public float startButtonOffset = 1.0f;

        private Rigidbody m_Rigidbody;
        private Player m_Player;
        private Vector3 m_TargetPosition;
        private enum PlayerState
        {
            Initial,
            Idle,
            Walk,
            Jump,
            Crouching,
            Winning,
            Dead
        };
        private PlayerState m_PlayerState;


        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Player = GetComponent<Player>();
            m_PlayerState = PlayerState.Initial;

            agent.updateRotation = false;
            agent.updatePosition = true;

            m_TargetPosition = transform.position;
        }


        private void Update()
        {
            agent.SetDestination(m_TargetPosition);

            if (m_Player.Dead)
            {
                m_Rigidbody.isKinematic = false;
                m_PlayerState = PlayerState.Dead;
            }

            if (m_PlayerState == PlayerState.Initial)
            {
                m_Rigidbody.isKinematic = true;
                m_PlayerState = PlayerState.Idle;
                UpdatePosition();
            }
            else if (m_PlayerState == PlayerState.Idle)
            {
                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    character.Move(agent.desiredVelocity, false, false);
                    m_Rigidbody.isKinematic = false;
                    m_PlayerState = PlayerState.Walk;
                }
            }
            else if (m_PlayerState == PlayerState.Walk)
            {
                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    character.Move(agent.desiredVelocity, false, false);
                }
                else
                {
                    character.Move(Vector3.zero, false, false);
                    m_Rigidbody.isKinematic = true;
                    m_PlayerState = PlayerState.Idle;
                    //キャラが停止したらカメラを追従させる
                    UpdatePosition();
                }

            }
            else if (m_PlayerState == PlayerState.Dead)
            {
                if (!m_Player.Dead)
                {
                    m_Rigidbody.isKinematic = true;
                    m_PlayerState = PlayerState.Idle;
                }
            }
        }

        private void UpdatePosition()
        {
            followCamera.position = transform.position - transform.forward * followCameraOffset + new Vector3(0, 1.0f, 0);
            followCamera.rotation = transform.rotation;
            startButton.position = transform.position + transform.forward * startButtonOffset + new Vector3(0, 3.0f, 0);
            startButton.rotation = transform.rotation;
        }

        public void SetTarget(Vector3 targetPosition)
        {
            m_TargetPosition = targetPosition;
            if (targetPosition == transform.position)
            {
                //ゲーム再開
                m_PlayerState = PlayerState.Initial;
            }
        }
    }
}
