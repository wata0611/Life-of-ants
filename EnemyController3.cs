using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hackathon_System;

public class EnemyController3 : MonoBehaviour
{
    enum MovePattern { appear, stay, leave }
    enum StayPattern { right, left }
    [SerializeField] MovePattern pattern = MovePattern.leave;
    [SerializeField] StayPattern dir = StayPattern.left;
    [SerializeField] GameObject appearPos;
    [SerializeField] GameObject[] stayPos;
    [SerializeField] GameObject leavePos;
    [SerializeField] Sprite Flying;
    [SerializeField] Sprite Staying;
    [SerializeField] float stayTimer = 5f;
    [SerializeField] float speed = 5.0f;
    [SerializeField] float difDistance=2.0f;
    [SerializeField] float appearTimeRandom1_1 = 20f;
    [SerializeField] float appearTimeRandom1_2 = 30f;
    [SerializeField] float appearTimeRnadom2_1 = 50f;
    [SerializeField] float appearTimeRandom2_2 = 60f;
    int randomPosNum = 0;
    float countTimer = 0f;
    float appearTime1 = 0f;
    float appearTime2 = 0f;
    Vector3 movePos;
    int appearCount = 0;
    bool leaveArrive = false;
    AudioSource audioSource;
    public AudioClip cry;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        transform.position = leavePos.transform.position;
        appearTime1 = Random.Range(appearTimeRandom1_1,appearTimeRandom1_2);
        appearTime2 = Random.Range(appearTimeRnadom2_1, appearTimeRandom2_2);
        Debug.Log(appearTime1);
        Debug.Log(appearTime2);
    }

    // Update is called once per frame
    void Update()
    {
        if (((GameManager.Instance.TimeMaster.CurrentTime >= appearTime1 && appearCount == 0) ||
            (GameManager.Instance.TimeMaster.CurrentTime >= appearTime2) && appearCount == 1) &&
            pattern == MovePattern.leave)
        {
            randomPosNum = Random.Range(0, stayPos.Length);
            movePos = new Vector3(stayPos[randomPosNum].transform.position.x + difDistance,
                stayPos[randomPosNum].transform.position.y,
                stayPos[randomPosNum].transform.position.z);
            pattern = MovePattern.appear;
            GetComponent<SpriteRenderer>().sprite = Flying;
            audioSource.PlayOneShot(cry);
            appearCount++;
        }
        else if (countTimer >= stayTimer)
        {
            countTimer = 0f;
            leaveArrive = false;
            pattern = MovePattern.leave;
            GetComponent<SpriteRenderer>().sprite = Flying;
            audioSource.PlayOneShot(cry);
        }

        if (pattern == MovePattern.appear)
            Appear();
        else if (pattern == MovePattern.stay)
            Stay();
        else if (pattern == MovePattern.leave)
            Leave();
    }

    //現れている間の行動
    void Stay()
    {
        if (transform.position == movePos)
            dir = StayPattern.right;
        else if (transform.position == stayPos[randomPosNum].transform.position)
            dir = StayPattern.left;

        if (dir == StayPattern.left)
            transform.position = Vector3.MoveTowards(transform.position, movePos, Time.deltaTime);
        else if (dir == StayPattern.right)
            transform.position = Vector3.MoveTowards(transform.position, stayPos[randomPosNum].transform.position, Time.deltaTime);

        countTimer += Time.deltaTime;
        Debug.Log(countTimer);
    }

    //出現
    void Appear()
    {
        if (transform.position == stayPos[randomPosNum].transform.position)
        {
            pattern = MovePattern.stay;
            GetComponent<SpriteRenderer>().sprite = Staying;
        }
        else
            transform.position = Vector3.MoveTowards(transform.position, stayPos[randomPosNum].transform.position, Time.deltaTime * speed);
    }

    //帰る
    void Leave()
    {
        if (transform.position == leavePos.transform.position)
        {
            transform.position = appearPos.transform.position;
            leaveArrive = true;
        }
        else if(!leaveArrive)
            transform.position = Vector3.MoveTowards(transform.position, leavePos.transform.position, Time.deltaTime*speed);
    }

    //触れているアリをすべて殺す
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Ant" && pattern == MovePattern.stay)
            collider.gameObject.GetComponent<Player.Ant>().Death();
    }

}
