using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITest : UIBase
{

    private Button button;
    protected override List<string> mFindNames
    {
        get {return new List<string>{"Button"}; }
    }

    protected override void OnInit()
    {
        GetComponentInChildsByName<Button>("Button",out button);
        BindButtonEvent(button,delegate {Debug.Log("点击按钮");});
    }

    protected override void OnShow(params object[] objs)
    {
        Debug.Log("显示");
    }

    protected override void OnHide(params object[] objs)
    {
        Debug.Log("隐藏");
    }
}
