using UnityEngine;

static public class Utility
{
    static public GameObject GetAncestor(this GameObject rhs, System.Func<GameObject, bool> func)
    {
        var p = rhs.transform.parent;
        do
        {

            if (p == null)
            {
                return null;
            }
            var pobj = p.gameObject;

            if (pobj == null)
            {
                return null;
            }

            if (func(pobj))
            {
                return pobj;
            }


        } while (p = p.transform.parent);

        return null;
    }

    static public T GetAncestorComponent<T>(this GameObject rhs) where T : class
    {
        var a = rhs.GetAncestor(n => n.GetComponent<T>() != null);
        if (a != null)
        {
            return a.GetComponent<T>();
        }
        else
        {
            return null;
        }

    }
}
