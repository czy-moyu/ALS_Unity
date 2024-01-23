using System;
using Moyu.Anim;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Plugins.Als.Editor.Scripts.Inspector
{
    public class StateNodeInspector<T> : GraphElement where T : PlayableNode
    {
        private StateNodeView<T> _nodeView;
        
        protected Length FieldLabelWidth { get; set; } = Length.Percent(30);
        
        public StateNodeInspector(StateNodeView<T> nodeView)
        {
            _nodeView = nodeView;
            
            AddToClassList("state-node-inspector");
            switch (_nodeView)
            {
                case StateNodeView<StateNode> stateNodeView:
                    AddTextField("Name", stateNodeView.GetNode().NodeName, true, OnNameChange);
                    break;
                case StateNodeView<EntryStateNode> entryStateNodeView:
                    AddTextField("Name", entryStateNodeView.GetNode().NodeName, false,null);
                    break;
            }
        }

        private void OnNameChange(string newName)
        {
            _nodeView.SetName(newName);
        }
        
        private void AddObjectField(string label, Object value, Action<object> SetValueFunc)
        {
            ObjectField clipField = new ObjectField(label);
            clipField.objectType = value.GetType();
            clipField.labelElement.style.minWidth = StyleKeyword.Auto;
            clipField.labelElement.style.maxWidth = StyleKeyword.Auto;
            clipField.labelElement.style.width = FieldLabelWidth;
            clipField.value = value;
            if (SetValueFunc != null)
                clipField.RegisterValueChangedCallback(evt => SetValueFunc(evt.newValue));
            Add(clipField);
            AddSeparator(5);
        }
        
        private void AddTextField(string label, string value, bool enable, Action<string> SetValueFunc)
        {
            TextField textField = new TextField(label);
            textField.labelElement.style.minWidth = StyleKeyword.Auto;
            textField.labelElement.style.maxWidth = StyleKeyword.Auto;
            textField.labelElement.style.width = FieldLabelWidth;
            textField.value = value;
            if (SetValueFunc != null)
                textField.RegisterValueChangedCallback(evt => SetValueFunc(evt.newValue));
            textField.SetEnabled(enable);
            Add(textField);
            AddSeparator(5);
        }
        
        private void AddSeparator(int height)
        {
            var separator = new HorizontalSeparatorVisualElement(height);
            Add(separator);
        }
    }
}