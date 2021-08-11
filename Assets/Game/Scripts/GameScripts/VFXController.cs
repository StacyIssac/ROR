using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    public float stopTime;
    public bool useFast = false;
    public bool useDestroy = false;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(useFast)
        {
            GetComponent<VisualEffect>().playRate = 10f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(useDestroy)
        {
            if(timer > stopTime)
            {
                Destroy(this.gameObject);
            }
            timer += Time.deltaTime;
        }
    }
}
