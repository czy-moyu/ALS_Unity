using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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
            showAddRemoveFooter = true,
            itemsSource = _editor.GetGraphAsset().GetAnimGraphs(),
            showBorder = true,
            makeItem = MakeItem,
            bindItem = BindItem,
        };
        Add(listView);
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
        AnimGraph animGraph = _editor.GetGraphAsset().GetAnimGraphs()[index];
        if (animGraph == null)
        {
            animGraph = _editor.GetGraphAsset().GetAnimGraphs()[index] = new AnimGraph();
            animGraph.Name = "AnimGraph" + index;
        }
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

        void Callback1(ChangeEvent<string> evt)
        {
            animGraph.Name = evt.newValue;
        }

        textField.RegisterValueChangedCallback(Callback1);
    }
    
    private static void BuildGraphNameContextualMenu(ContextualMenuPopulateEvent evt, TextField textField)
    {
        // evt.StopPropagation();
        for (int i = 0; i < evt.menu.MenuItems().Count; i++)
        {
            evt.menu.RemoveItemAt(0);
        }
        evt.menu.AppendAction("Rename", action =>
        {
            textField.SetEnabled(true);
            textField.Focus();
        }, DropdownMenuAction.AlwaysEnabled);
    }

    private void OnListItemClickTwice(MouseDownEvent evt, int itemIndex)
    {
        // 检查点击次数是否为2，以确定是否是双击
        if (evt.clickCount != 2 || evt.button != (int)PointerEventData.InputButton.Left) return;
        _selectedIndex = itemIndex;
        listView.Rebuild();
        OnSelect(itemIndex);
        // Debug.Log("OnListItemClickTwice: ");
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
