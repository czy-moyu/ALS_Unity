using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimControllerParams
{
    [SerializeField]
    private List<AnimControllerParamsPair<float>> floatParams;
    
    private Dictionary<string, IParamsPair> _paramsCache = new();

    public void Init()
    {
        foreach (var param in floatParams)
        {
            _paramsCache.Add(param.name, param);
        }
    }
    
    public T GetParam<T>(string name)
    {
        if (_paramsCache.TryGetValue(name, out IParamsPair param))
        {
            if (param is AnimControllerParamsPair<T> pair)
            {
                return pair.value;
            }
        }
        Debug.LogError($"AnimControllerParams: Param {name} not found");
        return default;
    }
    
    public void SetParam<T>(string name, T value)
    {
        if (_paramsCache.TryGetValue(name, out IParamsPair param))
        {
            if (param is AnimControllerParamsPair<T> pair)
            {
                pair.value = value;
                return;
            }
        }
        Debug.LogError($"AnimControllerParams: Param {name} not found");
    }
}

[Serializable]
public class AnimControllerParamsPair<T> : IParamsPair
{
    public string name;
    public T value;
}

interface IParamsPair {};