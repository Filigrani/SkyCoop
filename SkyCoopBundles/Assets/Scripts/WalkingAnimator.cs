using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAnimator : MonoBehaviour
{
    public Animator m_Animator;
    public float m_MinimalSpeed = 0.1f;
    public float m_Smoother = 0.1f;

    private Vector3 m_LastPosition = Vector3.zero;

    public bool m_Crouch = false;
    public bool m_TestWalking = true;

    public int m_Action = 0;

    public float m_WalkingSpeed = 1f;
    
    void Start()
    {
        m_LastPosition = gameObject.transform.position;
    }

    public void TestMoving()
    {
        Vector3 Velocity = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            Velocity.z = m_WalkingSpeed;
        } else if(Input.GetKey(KeyCode.S))
        {
            Velocity.z = -m_WalkingSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Velocity.x = m_WalkingSpeed;
        } else if (Input.GetKey(KeyCode.A))
        {
            Velocity.x = -m_WalkingSpeed;
        }
        transform.position += Velocity;

        m_Crouch = Input.GetKey(KeyCode.LeftControl);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Speed = (gameObject.transform.position - m_LastPosition) / Time.deltaTime;
        Speed.y = 0;
        Vector3 Direction = transform.InverseTransformDirection(Speed);
        m_LastPosition = gameObject.transform.position;

        if (m_Animator)
        {
            float AnimatorSpeed = Speed.magnitude;
            if (!m_Crouch)
            {
                AnimatorSpeed = AnimatorSpeed / 4;
            }
            m_Animator.SetFloat("Speed", AnimatorSpeed);
            float PreviousDirectionX = m_Animator.GetFloat("DirectionX");
            float PreviousDirectionY = m_Animator.GetFloat("DirectionY");
            m_Animator.SetBool("IsMoving", Direction.magnitude > m_MinimalSpeed);
            m_Animator.SetBool("Crouch", m_Crouch);
            m_Animator.SetFloat("Action", m_Action);

            m_Animator.SetFloat("DirectionX", Mathf.Lerp(PreviousDirectionX, Mathf.Clamp(Direction.x, -1, 1), m_Smoother));
            m_Animator.SetFloat("DirectionY", Mathf.Lerp(PreviousDirectionY, Mathf.Clamp(Direction.z, -1, 1), m_Smoother));
        }

        if (m_TestWalking)
        {
            TestMoving();
        }
    }
}
