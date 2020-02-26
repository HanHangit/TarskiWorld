﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : ASingleton<GameManager>
{
    [SerializeField]
    private SelectionManager _selectionManager = default;
    public SelectionManager GetSelectionManager() => _selectionManager;

    [SerializeField]
    private GUI_TextInputField _textInputField = default;
    public GUI_TextInputField GetTextInputField() => _textInputField;

    [SerializeField]
    private FloatVar _debugFloatVar = default;
    public FloatVar DebugFloatVar => _debugFloatVar;

    private List<IDebug> _debugList = new List<IDebug>();


    protected override void SingletonAwake()
    {
        _debugFloatVar.ValueChangedEvent.AddEventListener(DebugModeChangedListener);
        _debugFloatVar.ForceChangedEvent();
    }

    public bool IsDebugMode(int id)
    {
        return id == _debugFloatVar.CurrentValue;
    }

    public void AddObjToDebugList(IDebug component)
    {
        if (!_debugList.Contains(component))
            _debugList.Add(component);
    }

    private void DebugModeChangedListener(FloatVar.EventArgs arg0)
    {
        foreach (var item in _debugList)
        {
            if (item.GetDebugID() == arg0.NewValue)
            {
                item.DebugModeChanged(true);
            }
            else
            {
                item.DebugModeChanged(false);
            }
        }
    }
}
