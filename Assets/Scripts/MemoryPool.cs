using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MemoryPool : MonoBehaviour
{
    private Beat poolObject;
    private List<Beat> list;

    public void Init(Beat newObj)
    {
        poolObject = newObj;
        list = new List<Beat>();
    }

    public Beat Get()
    {
        List<Beat> inactiveList = list.Where(o => o.gameObject.activeSelf == false).ToList();

        if (inactiveList.Count == 0)
        {
            Beat newObj = Instantiate(poolObject);
            list.Add(newObj);
            return newObj;
        }
        else
        {
            inactiveList[0].gameObject.SetActive(true);
            return inactiveList[0];
        }
    }
}
