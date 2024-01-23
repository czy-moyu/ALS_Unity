namespace Moyu.Anim
{
    //  状态机结点对应的AnimGraph
    public class StateAnimGraph : AnimGraph
    {
        private StateNode node;
        
        public StateAnimGraph(StateNode node)
        {
            this.node = node;
        }
        
        public StateNode GetNode()
        {
            return node;
        }
    }
}