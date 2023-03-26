// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class BossEnemyAIController : MonoBehaviour
// {
//     public float teleDistance = 10f; 
//     public float teleCooldown = 5f; 
//     public float attackCooldown = 2f; 
//     public GameObject player; 
//     private float lastTeleTime; 
//     private float lastAttackTime; 
//     private HeroKnight player;

//     void Start()
//     {
//         lastTeleTime = Time.time;
//         lastAttackTime = Time.time;
//     }

//     void Update()
//     {
//         // Kiểm tra nếu đã đến thời điểm tele mới
//         if (Time.time > lastTeleTime + teleCooldown)
//         {
//             // Tìm vị trí xa nhất mà người chơi đã đứng trong thời gian gần đây
//             Vector3 lastPlayerPosition = player.GetComponent<PlayerController>().GetLastPosition();

//             // Tính toán khoảng cách đến vị trí đó
//             float distanceToPlayer = Vector3.Distance(transform.position, lastPlayerPosition);

//             // Nếu khoảng cách đến vị trí đó lớn hơn khoảng cách tối thiểu để tele, thì tiến hành di chuyển
//             if (distanceToPlayer > teleDistance)
//             {
//                 transform.position = lastPlayerPosition;
//                 lastTeleTime = Time.time;
//             }
//         }

//         // Kiểm tra nếu đã đến thời điểm tấn công mới
//         if (Time.time > lastAttackTime + attackCooldown)
//         {
//             // Tạo skill thường
//             CreateNormalSkill();

//             // Nếu đã nhận đủ 20 damage thì tạo ulti
//             if (GetComponent<Health>().currentHealth <= 80)
//             {
//                 CreateUltiSkill();
//             }

//             lastAttackTime = Time.time;
//         }
//     }

//     void CreateNormalSkill()
//     {
//         // Tạo skill thường
//         GameObject normalSkill = new GameObject("NormalSkill");
//         normalSkill.transform.position = transform.position;

//         // Tạo hai tia lade
//         GameObject lade1 = new GameObject("Lade1");
//         lade1.transform.position = transform.position + new Vector3(0, 1, 0);
//         lade1.transform.parent = normalSkill.transform;

//         GameObject lade2 = new GameObject("Lade2");
//         lade2.transform.position = transform.position + new Vector3(0, -1, 0);
//         lade2.transform.parent = normalSkill.transform;

//         // Gây sát thương
//         player.GetComponent<Health>().TakeDamage(5);
//     }

//     void CreateUltiSkill()
//     {
//         // Lấy vị trí người chơi đứng cuối cùng trước khi chiêu bay xuống
//         Vector3 lastPlayerPosition = player.GetComponent<PlayerController>().GetLastPosition();

//         // Tạo ulti
//         GameObject ulti = new GameObject("Ulti");

//         // Tạo tia lade từ trên trời xuống
//         GameObject lade = Instantiate(ladePrefab, lastPlayerPosition + new Vector3(0, 5, 0), Quaternion.identity);
//         lade.transform.parent = ulti.transform;

//         // Gây sát thương
//         for (int i = 0; i < 3; i++)
//         {
//             player.GetComponent<Health>().TakeDamage(8);
//             yield return new WaitForSeconds(0.5f);
//         }
//     }

// }
