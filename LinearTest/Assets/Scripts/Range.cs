using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Code sampled from Andrius Naruševičius
//https://stackoverflow.com/questions/5343006/is-there-a-c-sharp-type-for-representing-an-integer-range

public class Range : MonoBehaviour {


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public static List<int> range(int a, int b)
    {
        List<int> result = new List<int>();

        for (int i = a; i <= b; i++)
        {
            result.Add(i);
        }

        return result;
    }

    public static int[] Understand(string input)
    {
        return understand(input).ToArray();
    }

    public static List<int> understand(string input)
    {
        List<int> result = new List<int>();
        string[] lines = input.Split(new char[] { ';', ',' });

        foreach (string line in lines)
        {
            try
            {
                int temp = int.Parse(line);
                result.Add(temp);
            }
            catch
            {
                string[] temp = line.Split(new char[] { '-' });
                int a = int.Parse(temp[0]);
                int b = int.Parse(temp[1]);
                result.AddRange(range(a, b));
            }
        }

        return result;
    }


}
