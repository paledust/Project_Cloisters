using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private CursorState_SO cursorState_SO;
    private CURSOR_STATE currentCursorState = CURSOR_STATE.DEFAULT;
    void HideCursor()=>Cursor.visible = false;
    void ShowCursor()=>Cursor.visible = true;
    protected override void Awake(){
        var data = cursorState_SO.GetCursorStateData(currentCursorState);
        Cursor.SetCursor(data.texture, data.offset, CursorMode.Auto);
    }
    public void UpdateCursorState(CURSOR_STATE newState){
        if(currentCursorState != newState){
            currentCursorState = newState;
            var data = cursorState_SO.GetCursorStateData(currentCursorState);
            Cursor.SetCursor(data.texture, data.offset, CursorMode.Auto);
        }
    }
}
