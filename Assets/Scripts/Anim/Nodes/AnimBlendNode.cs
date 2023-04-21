using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Moyu.Anim
{
    // [CreateAssetMenu(fileName = "AnimBlendNode", menuName = "PlayableNode/AnimBlend", order = 3)]
    [Serializable]
    public class AnimBlendNode : PlayableNode
    {
        [SerializeReference]
        [PlayableInput]
        private PlayableNode input1;
    
        [SerializeReference]
        [PlayableInput]
        private PlayableNode input2;

        [SerializeField]
        private bool isBindParam;
    
        [SerializeField]
        private string paramName;
    
        [SerializeField]
        [Range(0, 1f)]
        private float alpha;

        private AnimationMixerPlayable animationMixerPlayable;
    
        public override Playable GetPlayable(PlayableGraph playableGraph, AnimController animController)
        {
            animationMixerPlayable = 
                AnimationMixerPlayable.Create(playableGraph, 2);
        
            Playable playable1 = input1.GetPlayable(playableGraph, animController);
            Playable playable2 = input2.GetPlayable(playableGraph, animController);
        
            playableGraph.Connect(playable1, 0, animationMixerPlayable, 0);
            playableGraph.Connect(playable2, 0, animationMixerPlayable, 1);

            UpdateWeight(animController);
        
            return animationMixerPlayable;
        }
    
        private void UpdateWeight(AnimController animController)
        {
            if (isBindParam)
            {
                float weight = animController.GetAnimParam<float>(paramName);
                weight = Mathf.Clamp01(weight);
                animationMixerPlayable.SetInputWeight(0, 1 - weight);
                animationMixerPlayable.SetInputWeight(1, weight);
            }
            else
            {
                animationMixerPlayable.SetInputWeight(0, 1 - alpha);
                animationMixerPlayable.SetInputWeight(1, alpha);
            }
        }

        public override void UpdatePlayable(float delta, PlayableGraph playableGraph, 
            AnimController animController)
        {
            input1.UpdatePlayable(delta, playableGraph, animController);
            input2.UpdatePlayable(delta, playableGraph, animController);
            // Debug.Log("AnimBlendNode Update " + alpha);

            UpdateWeight(animController);
        }
    }
}