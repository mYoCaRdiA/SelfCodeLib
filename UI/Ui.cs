using UnityEngine;

public abstract class Ui : MonoBehaviour
{
    public virtual void ShowUi()
    {
        gameObject.SetActive(true);
    }

    public virtual void CloseUi()
    {
        gameObject.SetActive(false);
    }
}
public abstract class RuntimeUi : Ui
{
    public override void CloseUi()
    {
        GameObjectPoolTool.PutInPool(gameObject);
    }
}

public abstract class StaticUi : Ui
{
   
}

