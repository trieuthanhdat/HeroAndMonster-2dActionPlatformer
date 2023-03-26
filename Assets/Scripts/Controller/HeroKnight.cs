using UnityEngine;
using System.Collections;
using System;
using RPG.Resources;
using System.Linq;
using System.Collections.Generic;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] GameObject m_AttackPointHolder;
    [SerializeField] int m_hitForce = 5;

    [SerializeField] float m_basicDamagePoint = 10;
    [SerializeField] float m_specialDamagePoint = 20;
    [SerializeField] float skillCoolDownTime = 4f;
    [SerializeField] float dashCoolDownTime = 2f;
    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_dashing = false;
    private bool                m_isUsingSpecial = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_dashDuration = 8.0f / 14.0f;
    private float               m_dashCurrentTime;
    private float               m_dashCoolDownTimeRemaining = 0;
    private float               m_skillCoolDownTimeRemaining = 0;
    private Health              m_health = null;
    private List<Sensor_AttackPoint> m_sensorAttackPoints = new List<Sensor_AttackPoint>();
    [HideInInspector]
    public List<Health> enemies = new List<Health>();
    
    public Animator HeroAnimator { get => m_animator;}
    public int FaceDirection {get =>   m_facingDirection; set => m_facingDirection = value ;}
    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_health = GetComponent<Health>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();

        m_sensorAttackPoints = m_AttackPointHolder.GetComponentsInChildren<Sensor_AttackPoint>().ToList();
    }

    // Update is called once per frame
    void Update ()
    {
        if(m_health.IsDead()) return;
        Timer();
        CheckGrounded();
        HandleInputAndMovement();
    }

    public void Timer()
    {
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if(m_dashing)
            m_dashCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if(m_dashCurrentTime > m_dashDuration)
            m_dashing = false;

        m_dashCoolDownTimeRemaining -= Time.deltaTime;
        m_skillCoolDownTimeRemaining -= Time.deltaTime;
    }


    public void CheckGrounded()
    {
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

    }
    public void HandleInputAndMovement()
    {
        // -- Handle input and movement --
        float inputX = Input.GetAxis(PC2D.Input.HORIZONTAL);
        // Swap direction of sprite depending on walk direction
        Flip(inputX);
        // Move
        Move(inputX);
        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        Debug.Log("m_dashing "+m_dashing +" m_isUsingSpecial "+m_isUsingSpecial +" m_grounded "+m_grounded);
        //Attack
        if(Input.GetButtonDown(PC2D.Input.ATTACK) && m_timeSinceAttack > 0.25f && !m_dashing && !m_isUsingSpecial)
        {
            HandleAttack();
        }

        // Block
        else if (Input.GetButton(PC2D.Input.BLOCK) && !m_dashing && !m_isUsingSpecial)
        {
           HandleBlock();
        }

        else if (Input.GetButtonUp(PC2D.Input.BLOCK) && !m_isUsingSpecial)
        {
            HandleIdleBlock();
        }
        // Dash
        else if (Input.GetButtonDown(PC2D.Input.DASH) && !m_dashing && !m_isWallSliding && !m_isUsingSpecial)
        {
            HandleDash();
        }
        else if(Input.GetButtonDown(PC2D.Input.SPECIAL) && !m_dashing && !m_isWallSliding && m_grounded)
        {
           HandleSpecial();
        } 

        //Jump
        else if (Input.GetButtonDown(PC2D.Input.JUMP) && m_grounded && !m_dashing)
        {
           HandleJump();
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }   
    }



    ///---ANIMETION HANDLER---//
    ///SOUND EFF
    public void PlaySoundEFFWithName(string name)
    {
        MonoAudioManager.instance.PlaySound(name);
    }
    public void HandleHurt(float damage)
    {
        m_animator.SetTrigger("Hurt");
        m_health.TakeDamage(damage);
    }

    public void HandleDeath()
    {
        m_animator.SetBool("noBlood", m_noBlood);
        m_animator.SetTrigger("Death");
        this.enabled = false;
    }

    private void HandleWallSlide()
    {
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);
    }
    private void HandleJump()
    {
        m_animator.SetTrigger("Jump");
        m_grounded = false;
        m_animator.SetBool("Grounded", m_grounded);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
        m_groundSensor.Disable(0.2f);
    }
    private void HandleAttack()
    {
        m_currentAttack++;

        // Loop back to one after third attack
        if (m_currentAttack > 3)
            m_currentAttack = 1;
        // Reset Attack combo if time since last attack is too large
        if (m_timeSinceAttack > 1.0f)
            m_currentAttack = 1;

        // Call one of three attack animations "Attack1", "Attack2", "Attack3"
        m_animator.SetTrigger("Attack" + m_currentAttack);
        // Reset timer
        m_timeSinceAttack = 0.0f;
    }
    
    private void HandleBlock()
    {
         m_animator.SetTrigger("Block");
         m_animator.SetBool("IdleBlock", true);
    }
    private void HandleSpecial()
    {
        if(m_skillCoolDownTimeRemaining > 0)
        {
            return;
        }
        m_isUsingSpecial = true;
        m_animator.SetTrigger("Special");
        m_body2d.velocity = new Vector2(m_facingDirection, m_body2d.velocity.y);
        m_skillCoolDownTimeRemaining = skillCoolDownTime ;
    }
    public float CalculateSkillCoolDownTimeFaction()
    {
        float temp = m_skillCoolDownTimeRemaining;
        if(temp <= 0)
            temp = 0;
        
        return temp/skillCoolDownTime;
    }
    public float CalculateDashCoolDownTimeFaction()
    {
        float temp = m_dashCoolDownTimeRemaining;
        if(temp <= 0)
            temp = 0;
        
        return temp/dashCoolDownTime;
    }
    private void HandleDash()
    {
        if(m_dashCoolDownTimeRemaining > 0)
        {
            m_dashing = false;
            return;
        }

        m_dashing = true;
        m_animator.SetTrigger("Dash");
        m_dashCurrentTime = 0;
        m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        m_dashCoolDownTimeRemaining = dashCoolDownTime;
    }
    private void Flip(float inputX)
    {
        if(m_isUsingSpecial) return;
        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
            m_AttackPointHolder.transform.localScale = new Vector2(1 ,m_AttackPointHolder.transform.localScale.y);
        }
            
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
            m_AttackPointHolder.transform.localScale = new Vector2(-1 ,m_AttackPointHolder.transform.localScale.y);
        }

    }
    private void HandleIdleBlock()
    {
        m_animator.SetBool("IdleBlock", false);
    }
    private void Move(float inputX)
    {
        if (!m_dashing && !m_isUsingSpecial && !m_dashing)
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

    }

    private void HitEnemy(float damage, float hitForce)
    {
        foreach (Health enemyHealth in enemies)
        {
            if (enemyHealth && !enemyHealth.IsDead())
            {
                enemyHealth.TakeDamage( damage,gameObject, enemyHealth.gameObject.GetComponent<IAIController>().animator);
                enemyHealth.GetComponent<Rigidbody2D>().AddForce(new Vector2(m_facingDirection * hitForce , 0), ForceMode2D.Impulse);
            }
        }
    }
    ///--- Animation Events ---///
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
    
    public void HandleAttackHit(int pointAttack)
    {
        foreach(Sensor_AttackPoint sap in m_sensorAttackPoints.Where(sap => sap.forPointAttack == pointAttack))
        {
            if(pointAttack != 4)
                HitEnemy(m_basicDamagePoint, (m_hitForce * pointAttack)/10);
            else
                HitEnemy(m_specialDamagePoint, (m_hitForce * pointAttack)/10);
        }
    }
    void ResetSpecialAttack()
    {
        m_isUsingSpecial = false;
    }
}
