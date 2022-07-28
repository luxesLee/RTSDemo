using UnityEngine;

public class SelectableCharacter : MonoBehaviour {

    public SpriteRenderer selectImage;
    public MonoBase thisMono;

    private void Awake() {
        selectImage.enabled = false;
        thisMono = GetComponentInParent<MonoBase>();
    }

    //Turns off the sprite renderer
    public void TurnOffSelector()
    {
        selectImage.enabled = false;
        thisMono.TurnOffOperateByPlayer();
    }

    //Turns on the sprite renderer
    public void TurnOnSelector()
    {
        selectImage.enabled = true;
        thisMono.TurnOnOperateByPlayer();
    }

}
