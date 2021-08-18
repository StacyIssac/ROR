using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class PlayerSkills : MonoBehaviour
{
    PlayerController playerController;

    [Header("血量")]
    public int HP;
    public int maxHP;
    public GameObject failPanel;

    [Header("等级")]
    public int maxExp;
    public float shootValAdd;
    public int HPValAdd;
    [HideInInspector]
    public int exp = 0;
    [HideInInspector]
    public int level = 1;
    
    [Header("能量")]
    public int energy = 0;

    [Header("攻击")]
    public SkillButtonController shootButton;
    public float shootValue = 10f;
    public float shootRange = 5f;
    public float shootLength = 10f;
    public float capsuleLength = 0.1f;
    public float height;
    public float shootCDTime = 0.5f;
    float shootTimer;
    bool canShoot;
    Vector3 hitTarget;
    RaycastHit hit;
    RaycastHit enemyHit;
    Ray shootRay;

    [Header("快跑")]
    public CinemachineFreeLook cam1;
    //public CinemachineFreeLook cam2;
    public GameObject playerObj;
    public float runningValue;
    float runSpeed;
    int canRunning = -1;

    [Header("瞬移")]
    public SkillButtonController rushButton;
    public float rushCDTime;
    public float rushDis;
    public float rushSpeed;
    public float rushMaxTime = 0.2f;
    public GameObject startObj;
    public GameObject endObj;
    float rushTimer;
    float rushTime = 0;
    bool canRush = false;
    Vector3 rushVec = Vector3.zero;
    RaycastHit wallHit;

    [Header("眩晕")]
    public SkillButtonController vertigoButton;
    public GameObject vertigoObj;
    public float vertigoShootTime;
    public float vertigoCDTime;
    public float createDis;
    public float vertigoSpeed;
    public int vertigoValue;
    public float vertigoRadius;
    float vertigoTimer;

    [Header("追踪导弹")]
    public SkillButtonController trackButton;
    public GameObject trackObj;
    public float trackCDTime;
    public float trackSpeed;
    public float trackValue;
    public float trackTempTimer = 1;
    public float traceNum = 20;
    int putTrace = 0;
    int enemyNums = 0;
    int indexNum = 0;
    float trackTimer;
    bool isTrace = false;
    List<GameObject> enemyObjs = new List<GameObject>();

    [Header("伤害")]
    public GameObject popupDamage;
    public GameObject attackObj;
    public Image attackImage1, attackImage2;
    public float imageAlpha;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        //moveSpeed = playerController.moveSpeed;
        runSpeed = playerController.moveSpeed;
        HP = maxHP;

        //���ü���CD
        rushTimer = 0;
        vertigoTimer = 0;
        trackTimer = 0;

        rushButton.skillCDTime = rushCDTime;
        vertigoButton.skillCDTime = vertigoCDTime;
        trackButton.skillCDTime = trackCDTime;
        shootButton.skillCDTime = shootCDTime;
        imageAlpha = attackImage1.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        //�ȼ�
        if(exp == maxExp)
        {
            level++;
            exp = maxExp - exp;
            shootValue += shootValAdd;
            maxHP += HPValAdd;
            HP += HPValAdd;
        }

        //Ѫ��
        if(HP < 0)
        {
            failPanel.SetActive(true);
            Time.timeScale = 0;
        }
        else if(HP > maxHP)
        {
            HP = maxHP;
        }
        
        if (canRunning == 1)
        {
            playerController.moveSpeed = runningValue;
            //cam1.priority = 0;
            cam1.Priority = 0;
            
        }
        else
        {
            playerController.moveSpeed = runSpeed;
            cam1.Priority = 10;
        }

        Running();

        //���
        if (rushTimer < 0)
        {
            Rush();
        }
        else
        {
            rushTimer -= Time.deltaTime;
        }

        //ѣ��
        if (vertigoTimer < 0)
        {
            Vertigo();
        }
        else
        {
            vertigoTimer -= Time.deltaTime;
        }

        //׷��
        if (trackTimer < 0)
        {
            TrackCheck();
        }
        else
        {
            trackTimer -= Time.deltaTime;
        }

        if(canShoot)
        {
            if (enemyHit.collider != null)
            {
                //׼����׼������ʾHP
                enemyHit.transform.GetComponent<EnemyStatus>().canSeeHP = true;
            }
        }

        if(Input.GetMouseButton(0) && hitTarget!= null && shootTimer < 0)
        {
            shootTimer = shootCDTime;
            shootButton.isSkill = true;
            Instantiate(attackObj, hitTarget, Quaternion.identity);

            if(canShoot)
            {
                CanShoot();
            }
        }
        else
        {
            shootTimer -= Time.deltaTime;
        }


        Trace();

        if (canRush)
        {
            if (rushTime == 0)
            {
                //��������ģ��
                playerObj.SetActive(false);
                Instantiate(startObj, transform.position, Quaternion.identity);
                rushTime += Time.deltaTime;
            }
            else if (rushTime > 0 && rushTime < rushMaxTime)
            {
                //�����ƶ�
                if(WallCheck())
                {
                    rushTime = rushMaxTime;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + rushVec * rushDis, rushSpeed);

                }
                rushTime += Time.deltaTime;
            }
            else if (rushTime >= rushMaxTime)
            {
                //��ʾ����ģ��
                playerObj.SetActive(true);
                Instantiate(endObj, transform.position, Quaternion.identity);
                rushTime = 0;
                canRush = false;
                canRunning = 0;
            }
        }
    }

    void FixedUpdate()
    {
        Shooting();
    }

    void Shooting()
    {
        Vector2 point = new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        shootRay = Camera.main.ScreenPointToRay(point);
        Physics.Raycast(shootRay, out hit);
        if(hit.transform != null)
        {
            hitTarget = hit.point;
            //��ý���
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                enemyHit = hit;
                //CanShoot();
                canShoot = true;
            }
            else
            {
                if(hit.transform.CompareTag("Ground"))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                }
                else
                {
                    Debug.DrawLine(transform.position, hit.point, Color.black);
                }
                
                if (enemyHit.collider != null)
                {
                    enemyHit.transform.GetComponent<EnemyStatus>().canSeeHP = false;
                }
                canShoot = false;
            }
        }
        else
        {
            Debug.DrawLine(transform.position, shootRay.origin + shootRay.direction * shootLength, Color.blue);
            hitTarget = shootRay.origin + shootRay.direction * shootLength;
            canShoot = false;
        }
    }

    void CanShoot()
    {
        
        
        enemyHit.transform.gameObject.GetComponent<EnemyStatus>().HP -= shootValue;
        enemyHit.transform.gameObject.GetComponent<EnemyStatus>().hasAttack = true;
        CreateDamageVal(hitTarget, (int)shootValue);

        //����ʱ��ϱ���״̬
        canRunning = -1;
    }

    void Running()
    {
        if((Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftControl)) && Input.GetAxis("Vertical") > 0.1f)
        {
            canRunning = -canRunning;
        }
        else if(Input.GetAxis("Vertical") <= 0.1f)
        {
            canRunning = -1;
        }
    }

    void Rush()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            rushTimer = rushCDTime;
            rushButton.isSkill = true;

            //���ƶ�����
            if (playerController.direction.magnitude >= 0.1f)
            {
                //���ƶ�����˲��
                rushVec = (Quaternion.Euler(0f, playerController.targetAngle, 0f) * Vector3.forward).normalized;
            }
            else //���ƶ�����
            {
                //��ͷָ����˲��
                if(shootRay.direction.y > 0)
                {
                    rushVec = Vector3.Normalize(new Vector3(playerController.moveDir.x, shootRay.direction.y, playerController.moveDir.z));
                }
                else
                {
                    rushVec = Vector3.Normalize(new Vector3(playerController.moveDir.x, 0, playerController.moveDir.z));
                }
            }
            canRush = true;
        }
    }

    void Vertigo()
    {
        if(Input.GetMouseButtonDown(1))
        {
            vertigoTimer = vertigoCDTime;
            vertigoButton.isSkill = true;

            //����һ������
            var moveDir = Vector3.Normalize(shootRay.direction);
            var tempVertigo = Instantiate(vertigoObj, transform.position + new Vector3(moveDir.x * createDis, 2, moveDir.z * createDis), Quaternion.identity);

            StartCoroutine(ShootVertigo(tempVertigo));
        }
    }

    void TrackCheck()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            trackTimer = trackCDTime;
            trackButton.isSkill = true;

            //����һ�ΰ���Rʱ����������
            if (putTrace == 0 && !isTrace)
            {
                putTrace = 1;
                isTrace = true;
                Debug.Log("open");
            }
        }
    }

    void Trace()
    {
        if (Input.GetKeyDown(KeyCode.R) && putTrace == 1 && !isTrace)
        {
            putTrace = 2;
            Debug.Log("shoot");
        }

        if (putTrace == 1)
        {
            //��ý���
            if (enemyHit.transform != null && enemyNums < traceNum)
            {
                if(enemyHit.transform.tag == "Enemy")
                {
                    if (!enemyHit.transform.gameObject.GetComponent<EnemyStatus>().isTrace)
                    {
                        enemyHit.transform.gameObject.GetComponent<EnemyStatus>().isTrace = true;
                        enemyObjs.Add(enemyHit.transform.gameObject);
                    }
                }
            }
            isTrace = false;
        }
        else if (putTrace == 2)
        {
            //���䵼��
            indexNum = 0;
            for (int i = 0; i < traceNum; i++)
            {
                if (indexNum < enemyObjs.Count)
                {
                    Debug.Log(enemyObjs.Count);
                    var rotation = Quaternion.Euler(0f, Random.Range(0, 180f), Random.Range(0f, 90f));
                    StartCoroutine(CreateTrace(rotation, indexNum));
                    indexNum++;
                }
                else
                {
                    indexNum = 0;
                }
            }

            Invoke("UnTrack", 5f);
            putTrace = 0;
        }
    }

    bool WallCheck()
    {
        Ray wallRay = new Ray(transform.position, new Vector3(playerController.moveDir.x, shootRay.direction.y, playerController.moveDir.z));
        Debug.DrawRay(transform.position, new Vector3(playerController.moveDir.x, shootRay.direction.y, playerController.moveDir.z));
        if (Physics.Raycast(wallRay, out wallHit))
        {
            //��ý���
            if (Physics.Linecast(transform.position, wallHit.transform.position, out wallHit) && wallHit.transform.tag == "Ground" && wallHit.distance < 0.5)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator CreateTrace(Quaternion rotation, int i)
    {
        yield return new WaitForSeconds(0.5f);

        var tempBullet = Instantiate(trackObj, transform.position + new Vector3(0, 2, 0), rotation);
        tempBullet.GetComponent<TrackBulletController>().Target = enemyObjs[i].transform;
    }

    IEnumerator ShootVertigo(GameObject vertigoObj)
    {
        yield return new WaitForSeconds(vertigoShootTime);

        vertigoObj.GetComponent<VertigoController>().moveForce = Vector3.Normalize(hitTarget - vertigoObj.transform.position) * vertigoSpeed;
    }

    void UnTrack()
    {
        for (int i = 0; i < enemyObjs.Count; i++)
        {
            if(enemyObjs[i] != null)
            {
                enemyObjs[i].GetComponent<EnemyStatus>().isTrace = false;
            }
        }

        enemyObjs.Clear();
    }

    void CreateDamageVal(Vector3 pos, int value)
    {
        GameObject mObject = (GameObject)Instantiate(popupDamage, pos, Quaternion.identity);
        mObject.GetComponent<AttackValue>().Value = value;
        mObject.GetComponent<AttackValue>().mTarget = pos;
    }

    public void IsAttack(int attackVal)
    {
        HP -= attackVal;
        attackImage1.color = new Color(attackImage1.color.r, attackImage1.color.g, attackImage1.color.b, imageAlpha);
        attackImage2.color = new Color(attackImage2.color.r, attackImage2.color.g, attackImage2.color.b, imageAlpha);
    }
}
