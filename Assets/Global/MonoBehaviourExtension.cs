using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtension
{
    public static void CallNextFrame(this MonoBehaviour behaviour, Action function)
    {
        behaviour.StartCoroutine(InvokeNextFrame(function));
    }

    private static IEnumerator InvokeNextFrame(Action function)
    {
        yield return null;
        function.Invoke();
    }

    public static void CallNextFrame<Arg1>(this MonoBehaviour behaviour, Action<Arg1> function, Arg1 arg1)
    {
        behaviour.StartCoroutine(InvokeNextFrame(function, arg1));
    }

    private static IEnumerator InvokeNextFrame<Arg1>(Action<Arg1> function, Arg1 arg1)
    {
        yield return null;
        function.Invoke(arg1);
    }

    public static void CallNextFrame<Arg1, Arg2>(this MonoBehaviour behaviour, Action<Arg1, Arg2> function, Arg1 arg1, Arg2 arg2)
    {
        behaviour.StartCoroutine(InvokeNextFrame(function, arg1, arg2));
    }

    private static IEnumerator InvokeNextFrame<Arg1, Arg2>(Action<Arg1, Arg2> function, Arg1 arg1, Arg2 arg2)
    {
        yield return null;
        function.Invoke(arg1, arg2);
    }
}
