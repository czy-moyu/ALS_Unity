using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimParams
{
    [SerializeReference]
    private List<IParamsPair> allParams = new ();
    
    private Dictionary<string, IParamsPair> _paramsCache = new();

    public void Init()
    {
        foreach (var param in allParams)
        {
            _paramsCache.Add(param.name, param);
        }
    }
    
    public List<IParamsPair> GetAllParams()
    {
        return allParams;
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
    [SerializeField]
    public T value;
}

[Serializable]
public class IParamsPair
{
    [SerializeField]
    public string name;
};