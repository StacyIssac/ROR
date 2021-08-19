using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowController : MonoBehaviour
{
    public float turnSmoothTime;
    
    GameObject player;
    float turnSmoothVelocity = 0;
    float angle;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //旋转
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, player.transform.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //上下移动
        //transform.Translate(transform.position + new Vector3(0, Mathf.Sin(Time.deltaTime), 0));
    }
}
