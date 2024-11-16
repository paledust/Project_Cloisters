using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cloisters/CursorState_SO")]
public class CursorState_SO : ScriptableObject
{
    [SerializeField] private List<CursorStateData> cursorStateList;
    public CursorStateData GetCursorStateData(CURSOR_STATE cursorState){
        var data = cursorStateList.Find(x=>x.cursorState == cursorState);
        if(data == null) return CursorStateData.Default;
        return cursorStateList.Find(x=>x.cursorState == cursorState);
    }
}

[System.Serializable]
public class CursorStateData{
    public CURSOR_STATE cursorState = CURSOR_STATE.DEFAULT;
    public Texture2D texture;
    public Vector2 offset = Vector2.zero;
    public static CursorStateData Default = new CursorStateData();
}
