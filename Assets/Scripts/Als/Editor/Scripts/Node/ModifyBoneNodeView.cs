using System.Collections.Generic;
using System.Reflection;
using Moyu.Anim;

[BindAnimNode(typeof(ModifyBoneNode))]
public class ModifyBoneNodeView : NodeView<ModifyBoneNode>
{
    public ModifyBoneNodeView(ModifyBoneNode node, List<FieldInfo> nodeInputFieldInfos, NodeGraphView graphView) 
        : base(node, nodeInputFieldInfos, graphView)
    {
        AddOutputPort();
    }
}
