using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExplodingPlatformController : MonoBehaviour
{
    [Header("Explosion Variables")]
    [SerializeField] private float deathRadius;
    [SerializeField] private float pushRadius;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float maxPushAmount;
    [SerializeField] private float minPushAmount;

    [Header("Component References")]
    public Animator anim;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !anim.enabled)
        {
            anim.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.back, deathRadius);
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.back, pushRadius);
    }

    public void DoExplosionEffects()
    {
        bool playerDead = false;//Physics2D.OverlapCircle(transform.position, deathRadius, playerLayer);
        bool pushPlayer = Physics2D.OverlapCircle(transform.position, pushRadius, playerLayer);
        if (playerDead)
        {
            PlayerController.instance.KillPlayer();
        }
        else if (pushPlayer)
        {
            PushPlayerFromForce();
        }
    }

    private void PushPlayerFromForce()
    {
        Vector2 forceDir = (PlayerController.instance.transform.position - transform.position).normalized;
        float distanceFromSource = Vector2.Distance(transform.position, PlayerController.instance.transform.position);
        float forceAmount = ((1 - (distanceFromSource / pushRadius)) * (maxPushAmount - minPushAmount)) + minPushAmount;
        Debug.Log(forceAmount);
        PlayerController.instance.PushFromExplosion(forceDir * forceAmount);
    }
}
