using System.Collections.Generic;
using UnityEngine;
using uMVVM.Sources.Infrastructure;

[RequireComponent(typeof(RectTransform))]
public class SelectManager : MonoBehaviour
{
    [Tooltip("The camera used for highlighting")]
    public Camera selectCam;
    [Tooltip("The rectangle to modify for selection")]
    public RectTransform SelectingBoxRect;
    
    private Rect SelectingRect;
    private Vector3 SelectingStart;

    [Tooltip("Changes the minimum square before selecting characters. Needed for single click select")]
    public float minBoxSizeBeforeSelect = 10f;

    public bool selecting = false;

    public Player player;

    public List<MonoBase> selectMono = new List<MonoBase>();

    private List<SelectableCharacter> selectedArmy = new List<SelectableCharacter>();
    public List<SelectableCharacter> holdonCharacter {
        get {
            return selectedArmy;
        }
    }

    private void Awake() {
        if (!SelectingBoxRect) {
            SelectingBoxRect = GetComponent<RectTransform>();
        }

    }

    void Update() {

        //The input for triggering selecting. This can be changed
        if (Input.GetMouseButtonDown(0)) {
            if(!Input.GetKey(KeyCode.LeftShift))
                ReSelect();

            //Sets up the screen box
            SelectingStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            SelectingBoxRect.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        selecting = Input.GetMouseButton(0);

        if (selecting) {
            SelectingArmy();

            CheckIfUnderMouse();
        } else
            SelectingBoxRect.sizeDelta = new Vector2(0, 0);

    }

    //Resets what is currently being selected
    void ReSelect() {
        foreach (var Army in selectedArmy) {
            Army.TurnOffSelector();
        }
        selectMono.Clear();
        selectedArmy.Clear();
    }

    //Does the calculation for mouse dragging on screen
    //Moves the UI pivot based on the direction the mouse is going relative to where it started
    //Update: Made this a bit more legible
    void SelectingArmy() {
        Vector2 _pivot = Vector2.zero;
        Vector3 _sizeDelta = Vector3.zero;
        Rect _rect = Rect.zero;

        //Controls x's of the pivot, sizeDelta, and rect
        if (-(SelectingStart.x - Input.mousePosition.x) > 0) 
        {
            _sizeDelta.x = -(SelectingStart.x - Input.mousePosition.x);
            _rect.x = SelectingStart.x;
        } 
        else 
        {
            _pivot.x = 1;
            _sizeDelta.x = (SelectingStart.x - Input.mousePosition.x);
            _rect.x = SelectingStart.x - SelectingBoxRect.sizeDelta.x;
        }

        //Controls y's of the pivot, sizeDelta, and rect
        if (SelectingStart.y - Input.mousePosition.y > 0) 
        {
            _pivot.y = 1;
            _sizeDelta.y = SelectingStart.y - Input.mousePosition.y;
            _rect.y = SelectingStart.y - SelectingBoxRect.sizeDelta.y;
        } 
        else 
        {
            _sizeDelta.y = -(SelectingStart.y - Input.mousePosition.y);
            _rect.y = SelectingStart.y;
        }

        //Sets pivot if of UI element
        if (SelectingBoxRect.pivot != _pivot)
            SelectingBoxRect.pivot = _pivot;

        //Sets the size
        SelectingBoxRect.sizeDelta = _sizeDelta;

        //Finished the Rect set up then set rect
        _rect.height = SelectingBoxRect.sizeDelta.x;
        _rect.width = SelectingBoxRect.sizeDelta.y;
        SelectingRect = _rect;

        //Only does a select check if the box is bigger than the minimum size.
        //While checking it messes with single click
        if (_rect.height > minBoxSizeBeforeSelect && _rect.width > minBoxSizeBeforeSelect) {
            CheckForSelectedCharacters();
        }
    }

    //Checks if the correct characters can be selected and then "selects" them
    void CheckForSelectedCharacters() {
        foreach (MonoBase soldier in player.monoOfPlayer) {
            Vector2 screenPos = selectCam.WorldToScreenPoint(soldier.transform.position);
            if (SelectingRect.Contains(screenPos)) {
                if (!selectMono.Contains(soldier)) {
                    selectMono.Add(soldier);
                    selectedArmy.Add(soldier.selectableCharacter);
                }
                soldier.selectableCharacter.TurnOnSelector();
            } else {
                soldier.selectableCharacter.TurnOffSelector();

                if (selectMono.Contains(soldier)) {
                    selectMono.Remove(soldier);
                    selectedArmy.Remove(soldier.selectableCharacter);
                }
                    
            }
        }
    }

    //Checks if there is a character under the mouse that is on the Selectable list
    void CheckIfUnderMouse() {
        RaycastHit hit;
        Ray ray = selectCam.ScreenPointToRay(Input.mousePosition);
        //Raycast from mouse and select character if its hit!
        if (Physics.Raycast(ray, out hit, 100f, ~(1 << 6))) {
            if (hit.transform != null) {
                MonoBase selectmono = hit.transform.gameObject.GetComponent<MonoBase>();
                if (selectmono != null && player.monoOfPlayer.Contains(selectmono) && !selectMono.Contains(selectmono)) {
                    selectMono.Add(selectmono);
                    selectedArmy.Add(selectmono.selectableCharacter);
                    selectmono.selectableCharacter.TurnOnSelector();
                }
            }
        }
    }
}
