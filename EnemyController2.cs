using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hackathon_System;

public class EnemyController2 : EnemyController
{
    [SerializeField] float backSpeed=0.25f;
    [SerializeField] GameObject start;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = start.transform.position; 
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
            }
            else if (pattern == ActionPattern.attack)
            {
                Chase(chaseAnt.transform);
                Rotate(chaseAnt);
            }
            else if (pattern == ActionPattern.back)
                Back();

            //Debug.Log(pattern);
        }
        if (DistancePlayer(player.transform) <= playerDistance)
            pattern = ActionPattern.back;
    }

    //巡回
    public override void Patrol()
    {
        transform.Rotate(new Vector3(0, 0, Mathf.Sin(2 * Mathf.PI * 5 * Time.deltaTime)));
    }

    //元の位置に戻る
    void Back()
    {
        if (transform.position == start.transform.position)
            pattern = ActionPattern.patrol;
        else
            Rotate(start);
        transform.position = Vector3.MoveTowards(transform.position, start.transform.position, Time.deltaTime * backSpeed);
    }

    //アリを食べる
    IEnumerator AttackTimer2(GameObject eatenAnt)
    {
        moveEnabled = false;
        eatenAnt.GetComponent<Player.Ant>().Death();
        pattern = ActionPattern.back;
        yield return new WaitForSeconds(waitTime);
        moveEnabled = true;
    }

    //アリと接触した時にアリを食べる
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Ant" && pattern == ActionPattern.attack)
        {
            StartCoroutine(AttackTimer2(chaseAnt));
        }
    }
}
