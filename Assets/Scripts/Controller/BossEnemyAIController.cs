using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;

public class BossEnemyAIController : IAIController
{
    public GameObject ladePrefab;
    public GameObject skillPrefab;
    public Transform skillPrefabSpawnPos;
    public float teleDistance = 10f; 
    public float teleCooldown = 5f; 
    public float attackCooldown = 2f; 
    public float activeDistance = 10;
    public HeroKnight player; 
    public Animator animator; // Animator component for controlling animations
    private float lastTeleTime; 
    private float lastAttackTime; 
    private bool isUltimate;
    private bool canTeleport = false;
    private Vector2 targetPosition ;
    bool isAttacking = false;
    bool isTeleporting = false;
    GameObject skill = null;
    public override void Start()
    {
        lastTeleTime = Time.time;
        lastAttackTime = Time.time;
    }

    public override void Update()
    {
        Flip();
            StartToTeleport();
            CheckTimeToAttack();
    }

    ///-----ANIMATION EVENTS-----///
    public void Teleport()
    {
        if(!CanSeePlayer()) return;
        transform.position = player.GetComponent<HeroKnight>().GetLastPosition();
        lastTeleTime = Time.time;
        HandleAppear();
        
    }
    public override void Flip()
    {
        if(CanSeePlayer())
            targetPosition = player.transform.position;

        transform.localScale = new Vector2( targetPosition.x > transform.position.x ? -1 : 1, transform.localScale.y);
    }
    public override bool CanSeePlayer()
    {
        // Calculate the direction from the enemy to the player
        Vector3 direction = player.transform.position - transform.position;
        // Cast a ray in the direction of the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, activeDistance, LayerMask.GetMask("Player"));
        // If the ray hit the player, return true
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }
    void CheckTimeToAttack()
    {
        if(!CanSeePlayer()) return;
        // Kiểm tra nếu đã đến thời điểm tấn công mới
        if (Time.time > lastAttackTime + attackCooldown)
        {
            // Tạo skill thường
            CreateNormalSkill();
            isAttacking = true;
            // Nếu đã nhận đủ 20 damage thì tạo ulti
            if (GetComponent<Health>().GetHealthPoint() <= 80 && !isUltimate)
            {
                StartCoroutine(CreateUltiSkill());
            }

            lastAttackTime = Time.time;
        }
        else 
            isAttacking = false;
    }
    void StartToTeleport()
    {
        // Kiểm tra nếu đã đến thời điểm tele mới
        if (Time.time > lastTeleTime + teleCooldown)
        {
            // Trigger teleport animation
            isTeleporting = true;
            Vector3 lastPlayerPosition = player.GetComponent<HeroKnight>().GetLastPosition();

            // Tính toán khoảng cách đến vị trí đó
            float distanceToPlayer = Vector3.Distance(transform.position, lastPlayerPosition);

            // Nếu khoảng cách đến vị trí đó lớn hơn khoảng cách tối thiểu để tele, thì tiến hành di chuyển
            if (distanceToPlayer > teleDistance)
            {
                if(skill!= null) Destroy(skill.gameObject);
                canTeleport = true;
                HandleTelePort();
            }
            
        }else isTeleporting = false;
    }
    public override void HandleAppear()
    {
        animator.SetTrigger("Appear");
    }
    void HandleTelePort()
    {
        animator.SetTrigger("Teleport");
    }
    void CreateNormalSkill()
    {
        // Tạo skill thường
        Vector3 lastPlayerPosition = player.GetComponent<HeroKnight>().GetLastPosition();
        // Tạo hai tia lade
        skill = Instantiate(skillPrefab, skillPrefabSpawnPos.position, Quaternion.identity);
    }

    IEnumerator CreateUltiSkill()
    {
        // Lấy vị trí người chơi đứng cuối cùng trước khi chiêu bay xuống
        Vector3 lastPlayerPosition = player.GetComponent<HeroKnight>().GetLastPosition();
        // Tạo ulti
        GameObject ulti = new GameObject("Ulti");
        // Tạo tia lade từ trên trời xuống
        GameObject lade = Instantiate(ladePrefab, lastPlayerPosition + new Vector3(0, 5, 0), Quaternion.identity);
        lade.transform.parent = ulti.transform;
        // Phát hiện trigger để bắt đầu animation tấn công ulti
        animator.SetBool("isUltimate", true);
        // Chờ đợi kết thúc animation
        yield return new WaitForSeconds(2f);
        // Xóa ulti
        Destroy(ulti);
    }
}