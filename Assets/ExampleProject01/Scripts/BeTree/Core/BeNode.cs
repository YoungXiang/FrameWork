using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeNode
{
    #region Node link
    public BeNode left;
    public BeNode right;
    public BeNode parent;
    #endregion

    // if return true, then go to right node, else go to left node
    public virtual bool Execute()
    {
        return false;
    }
}
