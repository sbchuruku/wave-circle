using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWave : MonoBehaviour
{
    public float _fAmp;
    public float _fRowSpeed;
    public float _fColSpeed;
    public bool _bMove = true;

    public Transform _TrChild;
    public LineRenderer _lr;

    List<Vector2> _lrPosList;
    Transform _Tr;
//     bool _bMoveUp = true;
    bool _bReachGoal = false;
    float _fMoveSin = 0;

    Result UIManager;

    void Start()
    {
        _lrPosList = new List<Vector2>();

        _Tr = GetComponent<Transform>();
        _lr = GetComponentInChildren<LineRenderer>();
        UIManager = GameObject.Find("UI Manager").GetComponent<Result>();
    }

    void FixedUpdate()
    {
        if( _bMove && !_bReachGoal && !CGameMgr._bisOver)
        {
            MoveChild();

            MoveCenter();
        }
    }

    private void MoveCenter()
    {
        Vector2 dir = Vector2.zero - (Vector2)_Tr.position;

        _Tr.Translate( dir.normalized * _fRowSpeed * Time.deltaTime, Space.World );

        if( !_bReachGoal )
        {
            float dist = Vector2.Distance( _TrChild.position, Vector3.zero );

            if ( dist < 0.6f )
            {
                _bReachGoal = true;
                _bMove = false;

                CGameMgr._bIsWaveMoving = false;

                for ( int i = 0; i < _lrPosList.Count; i++ )
                {
                    if ( i + 1 >= _lrPosList.Count )
                        break;

                    AddColliderToLine( _lrPosList[i], _lrPosList[i + 1] );
                }
            }
        }
    }

    private void MoveChild()
    {
        Vector2 pos = _TrChild.localPosition;

        pos.y = Mathf.Sin( _fMoveSin ) * _fAmp;
        
        _fAmp *= 0.99f;

        _TrChild.localPosition = pos;

        _fMoveSin += _fColSpeed * Time.deltaTime;

        if( _fMoveSin > Mathf.PI * 2 )
        {
            _fMoveSin -= Mathf.PI * 2;

            Debug.Log( "Score Up" );
            CGameMgr._iScore++;
            UIManager.ScoreRefresh(CGameMgr._iScore);
        }

        if ( !_lrPosList.Contains( _TrChild.position ) )
        {
            _lrPosList.Add( _TrChild.position );
//             _lr.SetVertexCount( _lrPosList.Count );          // Legacy
            _lr.positionCount = _lrPosList.Count;
            _lr.SetPosition( _lrPosList.Count - 1, (Vector3)_lrPosList[_lrPosList.Count - 1] );
        }
    }

    void AddColliderToLine( Vector3 startPos, Vector3 endPos )
    {
        BoxCollider2D col = new GameObject( "Collider" ).AddComponent<BoxCollider2D>();
        col.transform.parent = _lr.transform; // Collider is added as child object of line
        float lineLength = Vector3.Distance( startPos, endPos ); // length of line
        col.size = new Vector3( lineLength, 0.1f, 1f ); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = ( startPos + endPos ) / 2;
        col.transform.position = midPoint; // setting position of collider object
                                           // Following lines calculate the angle between startPos and endPos
        float angle = ( Mathf.Abs( startPos.y - endPos.y ) / Mathf.Abs( startPos.x - endPos.x ) );
        if ( ( startPos.y < endPos.y && startPos.x > endPos.x ) || ( endPos.y < startPos.y && endPos.x > startPos.x ) )
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan( angle );
        col.transform.Rotate( 0, 0, angle );
        col.tag = "Line";
        col.isTrigger = true;
    }
}
