using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWaveChild : MonoBehaviour
{
    void OnTriggerEnter2D( Collider2D col )
    {
        if ( col.tag == "Line" )
        {
            Debug.Log( "Game Over" );

            CGameMgr._bisOver = true;
            CGameMgr._bIsWaveMoving = false;

            // Todo : Game Over 처리
            Result result = GameObject.Find( "UI Manager" ).GetComponent<Result>();

            result.GameOverProcess( CGameMgr._iScore );
        }
    }
}
