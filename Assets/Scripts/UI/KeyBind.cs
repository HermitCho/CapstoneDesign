using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;


public enum ActionKey
{
    //�÷��̾� ������
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,

    //�÷��̾� ��ȣ�ۿ�
    Shoot,
    Reload,
    Sprint,
    Crouch,
    Skill1,
    Skill2,
}

[Serializable]
public class KeyBind : MonoBehaviour
{
    public Dictionary<ActionKey, KeyCode> Bindings => _bindingDict;
    private Dictionary<ActionKey, KeyCode> _bindingDict;


    //������
    public KeyBind(SerializableKeyBind serialKey)
    {
        _bindingDict =  new Dictionary<ActionKey, KeyCode>();

        foreach (var pair in serialKey.bindPairs)
        {
            _bindingDict[pair.key] = pair.value;
        }
    }

    //���ο� ���ε� Ű ����
    public void ApplyKeyBind(SerializableKeyBind newKeyBind)
    {
        _bindingDict.Clear();

        foreach (var pair in newKeyBind.bindPairs)
        {
            _bindingDict[pair.key] = pair.value;
        }
    }

    //���� ���ε� ���� �޼��� (allowOverLap: �ֺ� ��� ���� ����)
    public void Bind(in ActionKey actionKey, in KeyCode keyCode, bool allowOverLap = false)
    {
        if (!allowOverLap && _bindingDict.ContainsValue(keyCode))
        {
            var dictCopy = new Dictionary<ActionKey, KeyCode>(_bindingDict);

            foreach (var equal in dictCopy)
            {
                if (equal.Value.Equals(keyCode))
                {
                    _bindingDict[equal.Key] = KeyCode.None;
                }
            }
        }

        _bindingDict[actionKey] = keyCode;
    }


    // �ʱ� ���ε��� ���� �޼ҵ�
    public void ResetAll()
    {
        Bind(ActionKey.Shoot, KeyCode.Mouse0);

        Bind(ActionKey.MoveForward, KeyCode.W);
        Bind(ActionKey.MoveBackward, KeyCode.S);
        Bind(ActionKey.MoveLeft, KeyCode.A);
        Bind(ActionKey.MoveRight, KeyCode.D);

        Bind(ActionKey.Sprint, KeyCode.LeftShift);
        Bind(ActionKey.Skill1, KeyCode.Q);
        Bind(ActionKey.Skill2, KeyCode.E);
    }



    public void SaveFile()//���� �ʿ�
    {
        SerializableKeyBind serialKey = new SerializableKeyBind(this);
        string jsonStr = JsonUtility.ToJson(serialKey);

    }
}

[Serializable]
public class SerializableKeyBind
{
    public BindPair[] bindPairs;

    public SerializableKeyBind(KeyBind binding)
    {
        int len = binding.Bindings.Count;
        int index = 0;

        bindPairs = new BindPair[len];

        foreach(var pair in binding.Bindings)
        {
            bindPairs[index++] = new BindPair(pair.Key, pair.Value);
        }
    }
}



[Serializable]
public class BindPair
{
    public ActionKey key;
    public KeyCode value;

    public BindPair(ActionKey key, KeyCode value)
    {
        this.key = key;
        this.value = value;
    }
}





