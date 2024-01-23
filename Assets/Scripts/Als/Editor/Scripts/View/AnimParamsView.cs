using Moyu.Anim;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimParamsView : GraphElement
{
    private AnimInstance _animInstance;
    private readonly ListView listView;
    
    public AnimParamsView(AnimInstance animInstance)
    {
        _animInstance = animInstance;
        
        style.paddingLeft = 2;
        style.paddingRight = 2;
        style.paddingTop = 10;
        style.paddingBottom = 10;

        listView = new ListView
        {
            reorderable = true,
            reorderMode = ListViewReorderMode.Animated,
            selectionType = SelectionType.Single,
            showAddRemoveFooter = false,
            itemsSource = _animInstance.GetAnimParams().GetAllParams(),
            showBorder = true,
            makeItem = MakeItem,
            bindItem = BindItem,
        };
        Add(listView);
        this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
    }

    private void BuildContextualMenu(ContextualMenuPopulateEvent obj)
    {
        obj.menu.AppendAction("Add Bool Param", action =>
        {
            _animInstance.GetAnimParams().GetAllParams().Add(new AnimControllerParamsPair<bool>()
            {
                name = "bool",
                value = false,
            });
            listView.Rebuild();
        });
        
        obj.menu.AppendAction("Add Float Param", action =>
        {
            _animInstance.GetAnimParams().GetAllParams().Add(new AnimControllerParamsPair<float>()
            {
                name = "float",
                value = 0,
            });
            listView.Rebuild();
        });

        obj.menu.AppendAction("Add Vector2 Param", action =>
        {
            _animInstance.GetAnimParams().GetAllParams().Add(new AnimControllerParamsPair<Vector2>()
            {
                name = "vector2",
                value = Vector2.zero,
            });
            listView.Rebuild();
        });
        
        obj.menu.AppendAction("Add Vector3 Param", action =>
        {
            _animInstance.GetAnimParams().GetAllParams().Add(new AnimControllerParamsPair<Vector3>()
            {
                name = "vector3",
                value = Vector3.zero,
            });
            listView.Rebuild();
        });

        obj.menu.AppendAction("Add Object Param", action =>
        {
            _animInstance.GetAnimParams().GetAllParams().Add(new AnimControllerParamsPair<Object>()
            {
                name = "object",
                value = null,
            });
            listView.Rebuild();
        });
    }

    private VisualElement MakeItem()
    {
        return new VisualElement();
    }
    
    private void BindItem(VisualElement element, int index)
    {
        element.Clear();
        element.style.flexDirection = FlexDirection.Row;
        
        TextField nameLabel = new TextField();
        var param = _animInstance.GetAnimParams().GetAllParams()[index];
        nameLabel.SetValueWithoutNotify(param.name);
        element.Add(nameLabel);
        nameLabel.parent.parent.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
        nameLabel.style.width = Length.Percent(30);

        void TextFieldValueChanged(ChangeEvent<string> evt)
        {
            param.name = evt.newValue;
        }
        nameLabel.UnregisterValueChangedCallback(TextFieldValueChanged);
        nameLabel.RegisterValueChangedCallback(TextFieldValueChanged);

        if (param is AnimControllerParamsPair<bool> boolParam)
        {
            Label label = new Label("bool");
            element.Add(label);
            label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
            
            Toggle toggle = new Toggle();
            toggle.SetValueWithoutNotify(boolParam.value);
            element.Add(toggle);
            toggle.style.marginTop = 3;
            void ToggleValueChanged(ChangeEvent<bool> evt)
            {
                boolParam.value = evt.newValue;
            }
            toggle.UnregisterValueChangedCallback(ToggleValueChanged);
            toggle.RegisterValueChangedCallback(ToggleValueChanged);
            
        }

        if (param is AnimControllerParamsPair<float> floatParam)
        {
            Label label = new Label("float");
            element.Add(label);
            label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
            
            FloatField floatField = new FloatField();
            floatField.SetValueWithoutNotify(floatParam.value);
            element.Add(floatField);
            floatField.style.marginTop = 3;
            void FloatFieldValueChanged(ChangeEvent<float> evt)
            {
                floatParam.value = evt.newValue;
            }
            floatField.UnregisterValueChangedCallback(FloatFieldValueChanged);
            floatField.RegisterValueChangedCallback(FloatFieldValueChanged);
        }
        
        if (param is AnimControllerParamsPair<Vector2> vector2Param)
        {
            Label label = new Label("vector2");
            element.Add(label);
            label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
            
            Vector2Field vector2Field = new Vector2Field();
            vector2Field.SetValueWithoutNotify(vector2Param.value);
            element.Add(vector2Field);
            vector2Field.style.marginTop = 3;
            void Vector2FieldValueChanged(ChangeEvent<Vector2> evt)
            {
                vector2Param.value = evt.newValue;
            }
            vector2Field.UnregisterValueChangedCallback(Vector2FieldValueChanged);
            vector2Field.RegisterValueChangedCallback(Vector2FieldValueChanged);
        }
        
        if (param is AnimControllerParamsPair<Vector3> vector3Param)
        {
            Label label = new Label("vector3");
            element.Add(label);
            label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
            
            Vector3Field vector3Field = new Vector3Field();
            vector3Field.SetValueWithoutNotify(vector3Param.value);
            element.Add(vector3Field);
            vector3Field.style.marginTop = 3;
            void Vector3FieldValueChanged(ChangeEvent<Vector3> evt)
            {
                vector3Param.value = evt.newValue;
            }
            vector3Field.UnregisterValueChangedCallback(Vector3FieldValueChanged);
            vector3Field.RegisterValueChangedCallback(Vector3FieldValueChanged);
        }
        
        if (param is AnimControllerParamsPair<Object> objectParam)
        {
            Label label = new Label("object");
            element.Add(label);
            label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
            
            ObjectField objectField = new ObjectField();
            objectField.SetValueWithoutNotify(objectParam.value);
            element.Add(objectField);
            objectField.style.marginTop = 3;
            void ObjectFieldValueChanged(ChangeEvent<Object> evt)
            {
                objectParam.value = evt.newValue;
            }
            objectField.UnregisterValueChangedCallback(ObjectFieldValueChanged);
            objectField.RegisterValueChangedCallback(ObjectFieldValueChanged);
        }
    }
}
