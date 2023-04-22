using Moyu.Anim;

[BindAnimNode(typeof(ModifyBoneNode))]
public class ModifyBoneNodeView : NodeView<ModifyBoneNode>
{
    public ModifyBoneNodeView(ModifyBoneNode node, int inputPortNum, NodeGraphView graphView) 
        : base(node, inputPortNum, graphView)
    {
        AddOutputPort();
    }
}
