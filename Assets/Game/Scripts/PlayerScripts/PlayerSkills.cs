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
    public int energyAdd = 4;

    [Header("攻击")]
    public float shootValue = 10f;
    public float shootRange = 5f;
    public float shootLength = 10f;
    public float capsuleLength = 0.1f;
    public float height;
    public float shootCDTime = 0.5f;
    public float shootDisDissolve;
    float shootTimer;
    bool isEnemy;
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
    public Transform traceShootPos;
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
    public float critChance;
    [HideInInspector]
    public bool canCheckEnemyState = false;
    [HideInInspector]
    public float otherAttackVal = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        //moveSpeed = playerController.moveSpeed;
        runSpeed = playerController.moveSpeed;
        HP = maxHP;

        //时间归零
        rushTimer = 0;
        vertigoTimer = 0;
        trackTimer = 0;

        rushButton.skillCDTime = rushCDTime;
        vertigoButton.skillCDTime = vertigoCDTime;
        trackButton.skillCDTime = trackCDTime;
        //imageAlpha = attackImage1.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        //等级
        if(exp == maxExp)
        {
            level++;
            exp = maxExp - exp;
            shootValue += shootValAdd;
            maxHP += HPValAdd;
            HP += HPValAdd;
        }

        //血量
        if(HP < 0)
        {
            failPanel.SetActive(true);
            Time.timeScale = 0;
        }
        else if(HP > maxHP)
        {
            HP = maxHP;
        }
        
        //摄像机切换
        if (canRunning == 1)
        {
            playerController.moveSpeed = runningValue;
            cam1.Priority = 0;
            //cam1.m_Lens.FieldOfView = 80;
            
        }
        else
        {
            playerController.moveSpeed = runSpeed;
            cam1.Priority = 15;
            //cam1.m_Lens.FieldOfView = 40;
        }

        Running();

        //瞬移技能冷却
        if (rushTimer < 0)
        {
            Rush();
        }
        else
        {
            rushTimer -= Time.deltaTime;
        }

        //眩晕技能冷却
        if (vertigoTimer < 0)
        {
            Vertigo();
        }
        else
        {
            vertigoTimer -= Time.deltaTime;
        }

        //追踪技能冷却
        if (trackTimer < 0)
        {
            TrackCheck();
        }
        else
        {
            trackTimer -= Time.deltaTime;
        }

        //检测到怪物
        if(isEnemy)
        {
            if (enemyHit.collider != null)
            {
                //显示怪物的HP
                enemyHit.transform.GetComponent<EnemyStatus>().canSeeHP = true;
            }
        }
        else
        {
            if (enemyHit.collider != null)
            {
                //关闭HP显示
                enemyHit.transform.GetComponent<EnemyStatus>().canSeeHP = false;
            }
        }

        //点击鼠标左键射击
        if(Input.GetMouseButton(0) && hitTarget!= null && shootTimer < 0)
        {
            shootTimer = shootCDTime;
            //击中特效
            Instantiate(attackObj, hitTarget, Quaternion.identity);
            //关闭奔跑状态
            canRunning = -1;

            if (isEnemy)
            {
                CanShoot();
            }
        }
        else
        {
            shootTimer -= Time.deltaTime;
        }


        Trace();

        //瞬移
        if (canRush)
        {
            if (rushTime == 0)
            {
                //隐藏人物
                playerObj.SetActive(false);
                //消失特效
                Instantiate(startObj, transform.position, Quaternion.identity);
                rushTime += Time.deltaTime;
            }
            else if (rushTime > 0 && rushTime < rushMaxTime)
            {
                //墙面检测
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
                //显示人物
                playerObj.SetActive(true);
                //出现特效
                Instantiate(endObj, transform.position, Quaternion.identity);
                //参数归零
                rushTime = 0;
                canRush = false;
                canRunning = 0;
            }
        }
    }

    void FixedUpdate()
    {
        RayCheck();
    }

    void RayCheck()
    {
        Vector2 point = new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        shootRay = Camera.main.ScreenPointToRay(point);
        Physics.Raycast(shootRay, out hit);
        //射线检测到物体
        if(hit.transform != null)
        {
            hitTarget = hit.point;
            //射线检测为敌人
            if (hit.transform.CompareTag("Enemy"))
            {
                //画出射线
                Debug.DrawLine(transform.position, hit.point, Color.red);
                enemyHit = hit;
                //CanShoot();
                isEnemy = true;
            }
            else
            {
                //画出射线
                if (hit.transform.CompareTag("Ground"))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                }
                else
                {
                    Debug.DrawLine(transform.position, hit.point, Color.black);
                }
                
                isEnemy = false;
            }
        }
        else
        {
            //画出射线
            Debug.DrawLine(transform.position, shootRay.origin + shootRay.direction * shootLength, Color.blue);
            hitTarget = shootRay.origin + shootRay.direction * shootLength;
            isEnemy = false;
        }
    }

    void CanShoot()
    {
        //距离
        var dis = Vector3.Distance(transform.position, enemyHit.transform.position);

        //如果敌人处于眩晕状态
        if(canCheckEnemyState)
        {
            if (enemyHit.transform.gameObject.GetComponent<EnemyStatus>().hasVertigo)
            {
                enemyHit.transform.gameObject.GetComponent<EnemyStatus>().IsAttack(otherAttackVal);
            }
        }

        enemyHit.transform.gameObject.GetComponent<EnemyStatus>().IsAttack(shootValue, critChance);
        enemyHit.transform.gameObject.GetComponent<EnemyStatus>().hasAttack = true;
        CreateDamageVal(hitTarget, (int)shootValue);
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

            //人物在移动
            if (playerController.direction.magnitude >= 0.1f)
            {
                //瞬移到移动方向
                rushVec = (Quaternion.Euler(0f, playerController.targetAngle, 0f) * Vector3.forward).normalized;
            }
            else //人物未移动
            {
                //瞬移到摄像机方向
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

            //创建眩晕弹
            var moveDir = Vector3.Normalize(shootRay.direction);
            var tempVertigo = Instantiate(vertigoObj, transform.position + new Vector3(moveDir.x * createDis, 2, moveDir.z * createDis), Quaternion.identity);
            tempVertigo.GetComponent<VertigoController>().critChance = critChance;
            StartCoroutine(ShootVertigo(tempVertigo));
        }
    }

    void TrackCheck()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            trackTimer = trackCDTime;
            trackButton.isSkill = true;

            //开启追踪
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
            //定位敌人
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
            //设置每颗导弹的目标
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
            //如果前方有墙
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
        //创建眩晕弹
        var tempBullet = Instantiate(trackObj, traceShootPos.position, rotation);
        tempBullet.GetComponent<TrackBulletController>().Target = enemyObjs[i].transform;
        tempBullet.GetComponent<TrackBulletController>().critChance = critChance;
    }

    IEnumerator ShootVertigo(GameObject vertigoObj)
    {
        yield return new WaitForSeconds(vertigoShootTime);
        //创建追踪弹
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

    public void AddEnergy()
    {
        energy += energyAdd;
    }

    //设置射速
    public void SetShootCDTime(float val)
    {
        shootCDTime -= val;
    }

    //设置暴击率
    public void SetCritChance(float val)
    {
        critChance -= critChance;
    }

    //设置移速
    public void SetSpeed(float val)
    {
        runningValue += val;
    }

    //设置血量
    public void SetHP(int val)
    {
        maxHP += val;
        HP += val;
    }

    //设置衰减
    public void SetDissolve(float val)
    {
        shootDisDissolve -= val;
    }

    //设置能量增长
    public void SetEnergyAdd(int val)
    {
        energyAdd += val;
    }

    //设置攻击力
    public void SetShootVal(float val)
    {
        shootValue = val;
    }
}
