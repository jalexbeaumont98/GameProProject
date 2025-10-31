
using UnityEngine;


public class Shell_Indicator_Controller : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image shellImage;
    [SerializeField] private Sprite shellSprite;
    [SerializeField] private Sprite emptyShellSprite;

    public void SetFired()
    {
        shellImage.sprite = emptyShellSprite;
    }

    public void SetReloaded()
    {
        shellImage.sprite = shellSprite;
    }
    
    public void SetNewShellSprite(Sprite newSprite)
    {
        shellSprite = newSprite;
    }

}
