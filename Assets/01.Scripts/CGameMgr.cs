using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGameMgr : MonoBehaviour
{
    public float _fRadius;
    public GameObject _ringPref;
    public Transform _trRoot;
    public Transform _trGaugeLine;
//     public GameObject _objLineBG;
    public Transform _trPosSel;
    public float _fGaugeSpeed;
    public float _fPosSelRotSpeed;
    public static bool _bIsWaveMoving = false;
    public static int _iScore;
    public static bool _bisOver;

    public Transform rootTransform;

    [Space(20)]
    public float _fMaxAmp;
    public float _fMaxColSpeed;

    bool _bSetGauge = false;
    bool _bGaugeUp = true;
    int _iSetGaugeStep;
    bool _bPosSel = false;
    float _fPos;

    CWave _objGaugeSetWave;

    public AudioSource audioSource;

    public AudioClip spaceAudioClip;
 
    public void Reset()
    {
        foreach (Transform child in rootTransform)
        {
            Destroy(child.gameObject);
        }
        _trGaugeLine.gameObject.SetActive( false );
//         _objLineBG.SetActive( false );
        _trPosSel.gameObject.SetActive(false);
        _iSetGaugeStep = 0;
        _bisOver = false;
        _iScore = 0;
        Result result = GameObject.Find("UI Manager").GetComponent<Result>();
        result.ScoreRefresh(_iScore);
    }

    void Start()
    {
        Reset();
    }

    void FixedUpdate()
    {
        GaugeMove();

        SelectStartPos();
    }

    void Update()
    {
        if( !_bisOver && Input.GetKeyDown(KeyCode.Space) )
        {
            audioSource.volume = 0.3f;
            audioSource.PlayOneShot(spaceAudioClip);
            GaugeNextStep();
        }
    }

    public void SetGaugeStart()
    {
        _trGaugeLine.gameObject.SetActive( true );
//         _objLineBG.SetActive( true );
        _trGaugeLine.position = Vector2.zero;
        _trGaugeLine.rotation = Quaternion.identity;
        _iSetGaugeStep = 2;
        _bSetGauge = true;
    }

    void SelectStartPos()
    {
        if( !_bPosSel && _trPosSel.gameObject.activeSelf )
            return;

        _trPosSel.Rotate( new Vector3(0,0,_fPosSelRotSpeed*Time.deltaTime) );
    }

    public void GaugeNextStep()
    {
        if( _bIsWaveMoving )
            return;

        switch( _iSetGaugeStep )
        {
            case 0:
                {
                    _trPosSel.gameObject.SetActive(true);
                    _bPosSel = true;
                    _iSetGaugeStep = 1;
                }
                break;

            case 1:
                {
                    if( null != _objGaugeSetWave && _objGaugeSetWave._bMove )
                        break;

                    SetGaugeStart();
                    CreateWave();
                }
                break;

            case 2:
                {
                    _trGaugeLine.Rotate( new Vector3(0, 0, 90) );
                    _iSetGaugeStep = 3;

                    float value = (_trGaugeLine.position.y + 2.8f) / 5.6f;

                    SetAmp( _fMaxAmp * value );
                }
                break;

            case 3:
                {
                    _bSetGauge = false;
                    _iSetGaugeStep = 0;
                    _trGaugeLine.gameObject.SetActive( false );
//                     _objLineBG.SetActive( false );
                    _objGaugeSetWave._bMove = true;
                    _bIsWaveMoving = true;

                    float value = ( _trGaugeLine.position.y + 2.8f ) / 5.6f;

                    SetColSpeed( _fMaxColSpeed * value );
                }
                break;

            default:
                break;
        }
    }

    public void CreateWave()
    {
        if( _bIsWaveMoving )
            return;

//         float randAngle = Random.Range( 0f, 360f );

        float randAngle = _trPosSel.eulerAngles.z;

        GameObject obj = GameObject.Instantiate( _ringPref, new Vector2(_fRadius * Mathf.Cos((Mathf.PI/180)*randAngle ), _fRadius * Mathf.Sin((Mathf.PI/180)*randAngle)), Quaternion.identity ) as GameObject;

        obj.transform.parent = _trRoot;

        obj.transform.Rotate( new Vector3(0,0,randAngle) );

        _objGaugeSetWave = obj.GetComponent<CWave>();
        Color lrColor = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
//         _objGaugeSetWave._lr.SetColors( lrColor, lrColor );          // Legacy
        _objGaugeSetWave._lr.startColor = lrColor;
        _objGaugeSetWave._lr.endColor = lrColor;

        _trPosSel.gameObject.SetActive( false );
        _bPosSel = false;
    }

    void SetAmp( float amp )
    {
        if( null != _objGaugeSetWave )
        {
            _objGaugeSetWave._fAmp = amp;
        }
    }

    void SetColSpeed( float colspeed )
    {
        if ( null != _objGaugeSetWave )
        {
            _objGaugeSetWave._fColSpeed = colspeed;
        }
    }

    private void GaugeMove()
    {
        if ( _bSetGauge )
        {
            switch ( _iSetGaugeStep )
            {
                case 2:
                    {
                        if ( _bGaugeUp )
                        {
                            _trGaugeLine.Translate( Vector2.up * _fGaugeSpeed * Time.deltaTime );

                            if ( _trGaugeLine.position.y > 2.8f )
                            {
                                _bGaugeUp = !_bGaugeUp;
                            }
                        }
                        else
                        {
                            _trGaugeLine.Translate( Vector2.down * _fGaugeSpeed * Time.deltaTime );

                            if ( _trGaugeLine.position.y < -2.8f )
                            {
                                _bGaugeUp = !_bGaugeUp;
                            }
                        }
                    }
                    break;

                case 3:
                    {
                        if ( _bGaugeUp )
                        {
                            _trGaugeLine.Translate( Vector2.down * _fGaugeSpeed * Time.deltaTime );

                            if ( _trGaugeLine.position.x > 2.8f )
                            {
                                _bGaugeUp = !_bGaugeUp;
                            }
                        }
                        else
                        {
                            _trGaugeLine.Translate( Vector2.up * _fGaugeSpeed * Time.deltaTime );

                            if ( _trGaugeLine.position.x < -2.8f )
                            {
                                _bGaugeUp = !_bGaugeUp;
                            }
                        }
                    }
                    break;

                case 0:
                default:
                    break;
            }
        }
    }
}
