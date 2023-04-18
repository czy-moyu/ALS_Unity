using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "BlendSpace2D", menuName = "PlayableNode/BlendSpace2D", order = 3)]
public class BlendSpace2DNode : PlayableNode
{
    [SerializeField]
    private BlendSpacePoint blendSpacePointLeftBottom = new(new Vector2(0, 0));
    
    [SerializeField]
    private BlendSpacePoint blendSpacePointRightBottom = new(new Vector2(1, 0));
    
    [SerializeField]
    private BlendSpacePoint blendSpacePointLeftTop = new(new Vector2(0, 1));
    
    [SerializeField]
    private BlendSpacePoint blendSpacePointRightTop = new(new Vector2(1, 1));
    
    [SerializeField]
    private Vector2 currentPos;

    private AnimationMixerPlayable animationMixerPlayable;
    
    public override Playable GetPlayable(PlayableGraph playableGraph, AnimControllerParams animControllerParams)
    {
        animationMixerPlayable = AnimationMixerPlayable.Create(playableGraph, 4);
        AnimationClipPlayable leftBottomClip = AnimationClipPlayable.Create(
            playableGraph, blendSpacePointLeftBottom.clip);
        AnimationClipPlayable rightBottomClip = AnimationClipPlayable.Create(
            playableGraph, blendSpacePointRightBottom.clip);
        AnimationClipPlayable leftTopClip = AnimationClipPlayable.Create(
            playableGraph, blendSpacePointLeftTop.clip);
        AnimationClipPlayable rightTopClip = AnimationClipPlayable.Create(
            playableGraph, blendSpacePointRightTop.clip);
        
        playableGraph.Connect(leftBottomClip, 0, animationMixerPlayable, 0);
        playableGraph.Connect(rightBottomClip, 0, animationMixerPlayable, 1);
        playableGraph.Connect(leftTopClip, 0, animationMixerPlayable, 2);
        playableGraph.Connect(rightTopClip, 0, animationMixerPlayable, 3);
        
        UpdateWeight(animControllerParams);

        return animationMixerPlayable;
    }

    private void UpdateWeight(AnimControllerParams animControllerParams)
    {
        //假设输入参数（x，y）位于0到1之间
        float x = Mathf.Clamp01(currentPos.x);
        float y = Mathf.Clamp01(currentPos.y);

        float weightLeftBottom = (1f - x) * (1f - y);
        float weightRightBottom = x * (1f - y);
        float weightLeftTop = (1f - x) * y;
        float weightRightTop = x * y;
        
        animationMixerPlayable.SetInputWeight(0, weightLeftBottom);
        animationMixerPlayable.SetInputWeight(1, weightRightBottom);
        animationMixerPlayable.SetInputWeight(2, weightLeftTop);
        animationMixerPlayable.SetInputWeight(3, weightRightTop);
    }
    
    public override void UpdatePlayable(PlayableGraph playableGraph, AnimControllerParams animControllerParams)
    {
        UpdateWeight(animControllerParams);
    }
}

[Serializable]
public class BlendSpacePoint
{
    [ReadOnly]
    public Vector2 position;
    public AnimationClip clip;

    public BlendSpacePoint(Vector2 pos)
    {
        position = pos;
    }
}