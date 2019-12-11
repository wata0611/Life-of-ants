using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hackathon_System;

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject[] patrol;
    [SerializeField] protected GameObject player;
    [SerializeField] protected ActionPattern pattern = ActionPattern.patrol;
    [SerializeField] protected float waitTime = 1f;
    [SerializeField] protected float detectDistance = 2f;
    [SerializeField] protected float playerDistance=3f;
    [SerializeField] protected float detectCorner = 0f;
    protected enum ActionPattern { attack, patrol, back}
    protected GameObject chaseAnt;
    int patNum = 0;
    protected bool moveEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = patrol[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveEnabled)
        {
            if (pattern == ActionPattern.patrol)
            {
                Patrol();
                var max = GameManager.Instance.Player.AntsInstancesCount;
                for (int i = 0; i < max; i++)
                {
                    var ant = GameManager.Instance.Player.AntInstancesAt(i);
                    DetectAnt(ant);
                }
                Rotate(patrol[patNum]);
            }
            else if (pattern == ActionPattern.attack)
            {
                Chase(chaseAnt.transform);
                Rotate(chaseAnt);
            }
        }
        if (DistancePlayer(player.transform) <= playerDistance)
            pattern = ActionPattern.patrol;
    }

    //巡回
    public virtual void Patrol()
    {
        if (transform.position==patrol[patNum].transform.position)
            patNum = (patNum + 1) % patrol.Length;
        transform.position= Vector3.MoveTowards(transform.position, patrol[patNum].transform.position, Time.deltaTime);
    }

    //アリに触れたら食べる
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Ant" && pattern == ActionPattern.attack)
        {
            StartCoroutine(AttackTimer(chaseAnt));
        }
    }

    //アリの追跡
    protected virtual void Chase(Transform antTransform)
    {
        transform.position = Vector3.MoveTowards(transform.position, antTransform.position, Time.deltaTime * 2);
    }

    //アリを食べる
    IEnumerator AttackTimer(GameObject eatenAnt)
    {
        moveEnabled = false;
        eatenAnt.GetComponent<Player.Ant>().Death();
        pattern = ActionPattern.patrol;
        yield return new WaitForSeconds(waitTime);
        moveEnabled = true;
    }

    //アリの検知
    protected void DetectAnt(Player.Ant ant)
    {
        if (ViewingAngle(ant.transform) >= detectCorner && DistanceAnt(ant.transform) < detectDistance)
        {
            pattern = ActionPattern.attack;
            chaseAnt = ant.gameObject;
        }
    }

    //視認角度計算
    protected float ViewingAngle(Transform antTransform)
    {
        Vector3 direction = new Vector3(antTransform.position.x - transform.position.x, antTransform.position.y - transform.position.y, antTransform.position.z - transform.position.z);
        return Vector2.Dot(transform.up, direction.normalized);
    }

    //アリとの距離計算
    protected float DistanceAnt(Transform antTransform)
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x-antTransform.position.x,2)+ Mathf.Pow(transform.position.y - antTransform.position.y, 2)+ Mathf.Pow(transform.position.z - antTransform.position.z, 2));
    }

    //playerとの距離計算
    protected float DistancePlayer(Transform player)
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - player.position.x, 2) + Mathf.Pow(transform.position.y - player.position.y, 2) + Mathf.Pow(transform.position.z - player.position.z, 2));
    }

    //ターゲットに向かって回転
    protected void Rotate(GameObject target)
    {
        Vector3 newDir = target.transform.position - transform.position;
        newDir.z = 0;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, Time.deltaTime);
        transform.rotation = Quaternion.FromToRotation(Vector3.up, newDir.normalized);
    }

}
