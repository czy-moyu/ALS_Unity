using System;
using System.Collections.Generic;
using Moyu.Anim;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseGraphView : GraphView, IDropTarget
{
    private ISelectable prevSelection;
    protected Action<VisualElement> OnNodeViewSelected;
    
    public virtual bool CanAcceptDrop(List<ISelectable> selection)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool DragUpdated(DragUpdatedEvent evt, IEnumerable<ISelectable> selection, IDropTarget dropTarget, ISelection dragSource)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool DragPerform(DragPerformEvent evt, IEnumerable<ISelectable> selection, IDropTarget dropTarget, ISelection dragSource)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool DragEnter(DragEnterEvent evt, IEnumerable<ISelectable> selection, IDropTarget enteredTarget, ISelection dragSource)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool DragLeave(DragLeaveEvent evt, IEnumerable<ISelectable> selection, IDropTarget leftTarget, ISelection dragSource)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool DragExited()
    {
        throw new System.NotImplementedException();
    }

    protected abstract void OnSelectionChange();

    public virtual void Update()
    {
        if (selection.Count == 1 && prevSelection != selection[0])
        {
            OnSelectionChange();
        }
        if (selection.Count == 1)
            prevSelection = selection[0];
    }

    public abstract void SaveChanges();

    public abstract void AddOnNodeViewSelected(Action<VisualElement> action);
    
    public abstract BaseGraph GetGraph();
    
    protected void CreateBackground()
    {
        var gridStyleSheet = Resources.Load<StyleSheet>("GridBackground");
        styleSheets.Add(gridStyleSheet);
        // 创建背景
        GridBackground gridBackground = new();
        Insert(0, gridBackground);
    }
}
