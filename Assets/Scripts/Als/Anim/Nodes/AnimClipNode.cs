using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Moyu.Anim
{
    // [CreateAssetMenu(fileName = "AnimClipNode", menuName = "PlayableNode/AnimClip", order = 2)]
    [Serializable]
    public class AnimClipNode : PlayableNode
    {
        [SerializeField]
        private AnimationClip _clip;
    
        [Range(0, 10f)]
        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private bool isSingleFrame;
        
        [SerializeField]
        private float singleFrameTime;
        
        private AnimationClipPlayable animationClipPlayable;

        public override Playable GetPlayable(PlayableGraph playableGraph, 
            AnimController animController)
        {
            animationClipPlayable = AnimationClipPlayable.Create(playableGraph, _clip);
            animationClipPlayable.SetSpeed(speed);
            return animationClipPlayable;
        }
        
        // public AnimationClip GetClip()
        // {
        //     return _clip;
        // }

        public override void UpdatePlayable(float delta, PlayableGraph playableGraph, 
            AnimController animController)
        {
            animationClipPlayable.SetSpeed(speed);
            if (isSingleFrame)
                animationClipPlayable.SetTime(singleFrameTime);
        }

        // public void SetClip(AnimationClip newClip)
        // {
        //     _clip = newClip;
        // }
    }
}