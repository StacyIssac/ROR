using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlitchController : MonoBehaviour
{
    public Material mmm;
    public float destroyTime;
    float timer = 0;

    [SerializeField, Range(0, 1)]
    float _scanLineJitter = 0;

    public float scanLineJitter
    {
        get { return _scanLineJitter; }
        set { _scanLineJitter = value; }
    }

    [SerializeField, Range(0, 1)]
    float _verticalJump = 0;

    public float verticalJump
    {
        get { return _verticalJump; }
        set { _verticalJump = value; }
    }

    [SerializeField, Range(0, 1)]
    float _horizontalShake = 0;

    public float horizontalShake
    {
        get { return _horizontalShake; }
        set { _horizontalShake = value; }
    }

    [SerializeField, Range(0, 1)]
    float _colorDrift = 0;

    public float colorDrift
    {
        get { return _colorDrift; }
        set { _colorDrift = value; }
    }
    float _verticalJumpTime;

    [System.Obsolete]
    public void Update()
    {
        _verticalJumpTime += Time.deltaTime * _verticalJump * 11.3f;

        var sl_thresh = Mathf.Clamp01(1.0f - _scanLineJitter * 1.2f);
        var sl_disp = 0.002f + Mathf.Pow(_scanLineJitter, 3) * 0.05f;
        mmm.SetVector("_ScanLineJitter", new Vector2(sl_disp, sl_thresh));

        var vj = new Vector2(_verticalJump, _verticalJumpTime);
        mmm.SetVector("_VerticalJump", vj);

        mmm.SetFloat("_HorizontalShake", _horizontalShake * 0.2f);

        var cd = new Vector2(_colorDrift * 0.04f, Time.time * 606.11f);
        mmm.SetVector("_ColorDrift", cd);

        if(timer < destroyTime && gameObject.active)
        {
            if(transform.localScale.y < 1)
            {
                transform.localScale += new Vector3(0.05f, 0.05f, 0);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            timer += Time.deltaTime;
        }
    }
}
