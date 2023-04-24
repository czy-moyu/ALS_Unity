using System;
using System.Collections.Generic;
using Moyu.Anim;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class AnimGraphListView : GraphElement
{
    private readonly AnimGraphEditor _editor;
    private int _selectedIndex = 0;
    private readonly ListView listView;
    private event OnSelectedDelegate OnSelectedEvent;
    public delegate void OnSelectedDelegate(int newListIndex);

    public AnimGraphListView(AnimGraphEditor editor)
    {
        _editor = editor;
        listView = new()
        {
            reorderable = false,
            reorderMode = ListViewReorderMode.Animated,
            selectionType = SelectionType.Single,
            showAddRemoveFooter = false,
            itemsSource = _editor.GetGraphAsset().GetGraphs(),
            showBorder = true,
            makeItem = MakeItem,
            bindItem = BindItem,
        };
        Add(listView);
        this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
    }

    private void BuildContextualMenu(ContextualMenuPopulateEvent obj)
    {
        obj.menu.AppendAction("Add AnimGraph", action =>
        {
            AnimGraph animGraph = new();
            animGraph.Name = "AnimGraph" + _editor.GetGraphAsset().GetGraphs().Count;
            _editor.GetGraphAsset().GetGraphs().Add(animGraph);
            listView.RefreshItems();
        }, DropdownMenuAction.AlwaysEnabled);
    }
    
    public void AddOnIndexChangedEvent(OnSelectedDelegate onIndexChangedEvent)
    {
        OnSelectedEvent += onIndexChangedEvent;
    }
    
    public void RemoveOnIndexChangedEvent(OnSelectedDelegate onIndexChangedEvent)
    {
        OnSelectedEvent -= onIndexChangedEvent;
    }

    private void BindItem(VisualElement element, int index)
    {
        var textField = (TextField) element;
        BaseGraph animGraph = _editor.GetGraphAsset().GetGraphs()[index];
        Assert.IsNotNull(animGraph, $"index {index} graph is null");
        // if (animGraph == null)
        // {
        //     animGraph = _editor.GetGraphAsset().GetGraphs()[index] = new AnimGraph();
        //     animGraph.Name = "AnimGraph" + index;
        // }
        textField.SetValueWithoutNotify(animGraph.Name);
        textField.SetEnabled(false);
        
        element.parent.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
        element.style.width = Length.Percent(100);
        element.style.paddingTop = 0;
        if (_selectedIndex == index)
        {
            element.parent.style.backgroundColor = 
                new StyleColor(new Color(0.32f, 0.32f, 0.32f));
        }
        else
        {
            element.parent.style.backgroundColor = 
                new StyleColor(new Color(0.2745f, 0.2745f, 0.2745f));
        }
        textField.style.width = Length.Percent(30);

        void GraphNameMouseEvent(MouseDownEvent evt)
        {
            OnListItemClickTwice(evt, index);
        }

        textField.parent.RegisterCallback<MouseDownEvent>(GraphNameMouseEvent);

        void Callback(ContextualMenuPopulateEvent evt)
        {
            BuildGraphNameContextualMenu(evt, textField);
        }

        textField.parent.RegisterCallback<ContextualMenuPopulateEvent>(Callback);

        void EventCallback(FocusOutEvent evt)
        {
            textField.SetEnabled(false);
        }

        textField.RegisterCallback<FocusOutEvent>(EventCallback);

        void TextFieldValueChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == AnimInstance.ROOT_GRAPH_NAME)
            {
                Debug.LogError("AnimGraph name can't be " + AnimInstance.ROOT_GRAPH_NAME);
                return;
            }
            animGraph.Name = evt.newValue;
        }
        
        textField.RegisterValueChangedCallback(TextFieldValueChanged);
    }
    
    private static void BuildGraphNameContextualMenu(ContextualMenuPopulateEvent evt, TextField textField)
    {
        evt.StopPropagation();
        List<DropdownMenuAction> toRemove = new();
        for (int i = 0; i < evt.menu.MenuItems().Count; i++)
        {
            var item = evt.menu.MenuItems()[i];
            if (item is not DropdownMenuAction action) continue;
            switch (action.name)
            {
                case "Cut":
                case "Copy":
                    toRemove.Add(action);
                    break;
            }
        }
        foreach (var action in toRemove)
        {
            evt.menu.MenuItems().Remove(action);
        }

        if (textField.value != AnimInstance.ROOT_GRAPH_NAME)
        {
            evt.menu.AppendAction("Rename", _ =>
            {
                textField.SetEnabled(true);
                textField.Focus();
            }, DropdownMenuAction.AlwaysEnabled);
        }
    }

    private void OnListItemClickTwice(IMouseEvent evt, int itemIndex)
    {
        // 检查点击次数是否为2，以确定是否是双击
        if (evt.clickCount != 2 || evt.button != (int)PointerEventData.InputButton.Left) return;
        _selectedIndex = itemIndex;
        listView.RefreshItems();
        OnSelect(itemIndex);
    }

    private void OnSelect(int newListIndex)
    {
        OnSelectedEvent?.Invoke(newListIndex);
    }

    private static VisualElement MakeItem()
    {
        return new TextField();
    }
}
