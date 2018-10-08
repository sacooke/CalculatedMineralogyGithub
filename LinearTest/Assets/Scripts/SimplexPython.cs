using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SimplexPython : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public int identity(int numRows, int numCols, int val = 1, int rowStart = 0)
    {
        foreach(int i in Range.range(rowStart,numRows))
        {
            foreach(int j in Range.range(0,numCols))
            {
                if (i == j) return val;
            }
        }
        return 0;
    }

    public void standardForm(int[] cost, float[] greaterThans = null, float[] gtThreshold = null, float[] lessThans = null, float[] ltThreshold = null, float[] equalities = null, float[] eqThreshold = null, bool maximization = true)
    {
        int newVars = 0;
        int numRows = 0;
        if (gtThreshold != null)
        {
            newVars += gtThreshold.Length;
            numRows += gtThreshold.Length;
        }
        if (ltThreshold != null)
        {
            newVars += ltThreshold.Length;
            numRows += ltThreshold.Length;
        }
        if (eqThreshold != null)
        {
            newVars += eqThreshold.Length;
            numRows += eqThreshold.Length;
        }
        
        if(!maximization)
        {
            for (int i = 0; i <= cost.Length; i++)
                cost[i] *= -1;
        }

        if (newVars == 0)
            return; //return cost, equalities, eqThreshold

        int[] newCost = cost;
        cost[0] *= newVars;

        //newCost = List(cost) + [0] * newVars;

        List<float> constraints = new List<float>();
        List<float> threshold = new List<float>();

        System.Object[][] oldConstraints = new System.Object[][] { new System.Object[] { greaterThans, gtThreshold, -1 }, new System.Object[] {lessThans, ltThreshold, 1 }, new System.Object[] { equalities, eqThreshold, 0} };

        int offset = 0;

        //foreach()
    }

    /*public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(
    this IEnumerable<TFirst> first,
    IEnumerable<TSecond> second,
    Func<TFirst, TSecond, TResult> func)
    {
        using (var enumeratorA = first.GetEnumerator())
        using (var enumeratorB = second.GetEnumerator())
        {
            while (enumeratorA.MoveNext())
            {
                enumeratorB.MoveNext();
                yield return func(enumeratorA.Current, enumeratorB.Current);
            }
        }
    }*/
}
