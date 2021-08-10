using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    public float stopTime;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > stopTime)
        {
            Destroy(this.gameObject);
        }
        timer += Time.deltaTime;
    }
}
