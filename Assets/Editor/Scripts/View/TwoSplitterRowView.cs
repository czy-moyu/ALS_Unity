using GBG.AnimationGraph.Editor.ViewElement;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TwoSplitterRowView : VisualElement
{
    public VisualElement TopElement;

    public VisualElement BottomElement;
    
    public TwoSplitterRowView(float topPaneMinHeight, float splitterWidth = 2f) {
        style.flexGrow = 1;
        style.flexDirection = FlexDirection.Column;
        
        var splitterColor = new Color(35 / 255f, 35 / 255f, 35 / 255f, 1.0f);
        
        // Left pane
        TopElement = new VisualElement
        {
            name = "top-pane",
            style =
            {
                minHeight = topPaneMinHeight,
                width = Length.Percent(100),
                height = topPaneMinHeight,
                paddingLeft = 2,
                paddingRight = 2,
                paddingTop = 2,
                paddingBottom = 2,
            }
        };
        Add(TopElement);
        
        // Left splitter
        var splitter = new VisualElement
        {
            name = "splitter",
            style =
            {
                width = Length.Percent(100),
                height = splitterWidth,
                backgroundColor = splitterColor,
                cursor = TripleSplitterRowView.LoadCursor(MouseCursor.SplitResizeUpDown),
            }
        };
        Add(splitter);
        // Left dragger
        var topDragger = new PaneDraggerManipulator(TopElement, FlexDirection.Column, 1);
        splitter.AddManipulator(topDragger);
        
        // Middle pane
        BottomElement = new VisualElement()
        {
            name = "bottom-pane",
            style =
            {
                flexGrow = 1,
                paddingLeft = 2,
                paddingRight = 2,
                paddingTop = 2,
                paddingBottom = 2,
            }
        };
        Add(BottomElement);
    }
}
