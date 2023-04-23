using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class AnimGraphListView : GraphElement
{
    private List<AnimGraph> _graphs;
    private int _selectedIndex = 0;
    private ListView listView;
    private event OnIndexChangedDelegate OnIndexChangedEvent;
    public delegate void OnIndexChangedDelegate(int newListIndex);

    public AnimGraphListView(List<AnimGraph> graphs)
    {
        _graphs = graphs;
        listView = new()
        {
            reorderable = false,
            reorderMode = ListViewReorderMode.Animated,
            selectionType = SelectionType.Single,
            showAddRemoveFooter = true,
            itemsSource = _graphs,
            showBorder = true,
            makeItem = MakeItem,
            bindItem = BindItem,
        };
        Add(listView);
    }
    
    public void AddOnIndexChangedEvent(OnIndexChangedDelegate onIndexChangedEvent)
    {
        OnIndexChangedEvent += onIndexChangedEvent;
    }
    
    public void RemoveOnIndexChangedEvent(OnIndexChangedDelegate onIndexChangedEvent)
    {
        OnIndexChangedEvent -= onIndexChangedEvent;
    }

    private void BindItem(VisualElement element, int index)
    {
        var textField = (TextField) element;
        AnimGraph animGraph = _graphs[index];
        if (animGraph == null)
        {
            animGraph = _graphs[index] = new AnimGraph();
            animGraph.Name = "AnimGraph" + index;
        }
        textField.value = animGraph.Name;
        textField.SetEnabled(false);
        
        element.parent.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
        element.style.width = Length.Percent(100);
        element.style.paddingTop = 0;
        if (_selectedIndex == index)
        {
            element.parent.style.backgroundColor = new StyleColor(new Color(0.32f, 0.32f, 0.32f));
        }
        else
        {
            element.parent.style.backgroundColor = new StyleColor(new Color(0.2745f, 0.2745f, 0.2745f));
        }
        textField.style.width = Length.Percent(30);
        textField.parent.RegisterCallback<MouseDownEvent>(evt =>
        {
            OnListItemClickTwice(evt, index);
        });
    }

    private void OnListItemClickTwice(MouseDownEvent evt, int itemIndex)
    {
        // 检查点击次数是否为2，以确定是否是双击
        if (evt.clickCount != 2 || evt.button != (int)PointerEventData.InputButton.Left) return;
        _selectedIndex = itemIndex;
        listView.Rebuild();
        OnIndexChanged(itemIndex);
    }

    private void OnIndexChanged(int newListIndex)
    {
        OnIndexChangedEvent?.Invoke(newListIndex);
    }

    private static VisualElement MakeItem()
    {
        return new TextField();
    }
}
