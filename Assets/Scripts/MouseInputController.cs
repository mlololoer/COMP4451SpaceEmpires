using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Handle stuff like dragging + clicking
public class MouseInputController : MonoBehaviour
{
	
    //used to detect when cursor is above UI elements
    EventSystem eventSys;
    void Awake()
    {
        eventSys = GameObject.Find("EventSystem").GetComponent<EventSystem>();        
    }

    /*
    SCENARIOS

    ____________________
    |                  |
    |                  |
    |      _____       |
    |      | A |       |
    |      |___|  B    |
    |                  |
    |__________________|

    1) NO dragging
        a. LMB down: nothing
        b. LMB up: select unit (if mouse is not on UI)
        c. RMB down: If a SHIP is selected, we SHOW the movement overlay
        d. RMB up: The movement overlay disappears. Movement is committed depending on whether the cursor was above the movement overlay
    2) WITH dragging (mouse held, then moved more than some value epsilon)
        a. LMB down: 
            A: Handled by UI mouse controller
            B: Assign mouse movement to map movement? (start dragging map) if not on UI. 
                If RMB is already down: ONLY allow map movement, NO selection.
        b. LMB up: 
            A->A: UI mouse handler should take care of it (still don't do anything)
            A->B: NOTHING happens on the map
            B->A: Unassign mouse movement to map movement, and nothing else
            B->B: Same as above
        c. RMB down:
            A: Handled by UI mouse controller
            B: Show a movement selector on the screen, that follows the cursor
        d. RMB up:
            A->A: Nothing
            A->B: Nothing
            B->A: The movement overlay disappears, and NO movement is committed
            B->B: Unassign mouse movement from movement selector and commit movement (if mouse was above movement overlay)
    */
    bool leftDragging = false;
    bool leftDown = false;
    bool leftWasOnUI = false;
    bool rightDragging = false;
    bool rightDown = false;
    bool rightWasOnUI = false;
    Vector3 leftMouseDownPos;
    Vector3 rightMouseDownPos;
    Vector3 lastCameraPosBeforeDrag;
    float dragEpsilon = 0.3f;
    //3.55, 2.0
    public float zoomXMultiplier;
    public float zoomYMultiplier;
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            leftDown = false;
            leftDragging = false;
            rightDown = false;
            rightDragging = false;
        }*/

        bool onUI = eventSys.IsPointerOverGameObject();
        //LMB DOWN
        if (Input.GetMouseButtonDown(0))
        {
            leftDown = true;
            leftDragging = false;
            leftMouseDownPos = Input.mousePosition;
            lastCameraPosBeforeDrag = this.transform.position;
            leftWasOnUI = onUI;
        }
        else if (leftDragging || (leftDown && Input.GetMouseButton(0) && Vector3.Distance(Input.mousePosition, leftMouseDownPos) > dragEpsilon))
        {
            //now we won't stop dragging if we go back inside the dragepsilon zone
            leftDragging = true;
            if (!leftWasOnUI) {
                //handle camera dragging
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - leftMouseDownPos);
                Vector3 diff = new Vector3(-pos.x * Camera.main.orthographicSize * zoomXMultiplier, -pos.y * Camera.main.orthographicSize * zoomYMultiplier, 0);
                this.transform.position = lastCameraPosBeforeDrag + diff;
            }
        }
        if (leftDown && Input.GetMouseButtonUp(0))
        {
            leftDown = false;
            if (!leftDragging && !leftWasOnUI && !rightDown) {
                //LMB up (no drag)
                SelectionManager.SM.Select(GetGameObjectsAtCursor(Input.mousePosition));
            }
            leftDragging = false;
            if (!leftWasOnUI) {
                //scenario 2b B->B
            }
        }

        //RMB DOWN
        if (Input.GetMouseButtonDown(1))
        {
            rightDown = true;
            rightDragging = false;
            rightMouseDownPos = Input.mousePosition;
            //since the camera might move during movement selection, we can't set this, we need to rely on this.transform.position instead
            //lastCameraPosBeforeDrag = this.transform.position;
            rightWasOnUI = onUI;
            if (!onUI) {
                SelectionManager.SM.MoveOverlayOn();
            }
        }
        else if (rightDragging || (rightDown && Input.GetMouseButton(1) && Vector3.Distance(Input.mousePosition, rightMouseDownPos) > dragEpsilon))
        {
            //now we won't stop dragging if we go back inside the dragepsilon zone
            rightDragging = true;
            if (!rightWasOnUI) {
                //handle movement selection
                /*Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - rightMouseDownPos);
                Vector3 diff = new Vector3(-pos.x * Camera.main.orthographicSize * zoomXMultiplier, -pos.y * Camera.main.orthographicSize * zoomYMultiplier, 0);
                this.transform.position = lastCameraPosBeforeDrag + diff;*/
            }
        }
        if (rightDown && Input.GetMouseButtonUp(1))
        {
            rightDown = false;
            if (!rightWasOnUI) {
                //RMB up (no drag)
                SelectionManager.SM.MoveOverlayOff(GetGameObjectsAtCursor(Input.mousePosition));
            }
            rightDragging = false;
        }
    }


    List<GameObject> GetGameObjectsAtCursor(Vector3 mousePos) {
        mousePos.z = -this.transform.position.z;
        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll (Camera.main.ScreenPointToRay(mousePos), Mathf.Infinity);
        List<GameObject> hitObjects = new List<GameObject>();
        //Debug.Log("START LIST OF CLICKED");
        foreach (var hit in hits) {
            if (hit.collider != null) {
                //Debug.Log(hit.collider.gameObject.transform.parent.gameObject);
                if (hit.collider.gameObject.transform.parent.gameObject.layer != 5) {
                    hitObjects.Add(hit.collider.gameObject.transform.parent.gameObject);
                }
            }
        }
        //Debug.Log("END LIST OF CLICKED");
        return hitObjects;
    }
    /* OLD MOUSE SELECTION CODE
        //2. Handle clicking map (not when clicking UI elements)
        if (!eventSys.IsPointerOverGameObject()) {
            //LEFT click: SELECTION
            if (Input.GetMouseButtonUp(0)) {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = -this.transform.position.z;
                RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll (Camera.main.ScreenPointToRay(mousePos), Mathf.Infinity);
                List<GameObject> hitObjects = new List<GameObject>();
                Debug.Log("START LIST OF CLICKED");
                foreach (var hit in hits) {
                    if (hit.collider != null) {
                        Debug.Log(hit.collider.gameObject.transform.parent.gameObject);
                        if (hit.collider.gameObject.transform.parent.gameObject.layer != 5) {
                            hitObjects.Add(hit.collider.gameObject.transform.parent.gameObject);
                        }
                    }
                }
                Debug.Log("END LIST OF CLICKED");

                SelectionManager.SM.Select(hitObjects);
            }
            //RIGHT click DOWN: If we selected a ship, then SHOW the MOVEMENT OVERLAY.
            if (Input.GetMouseButtonDown(1)) {

            }
            //RIGHT click UP: Commit the movement
            if (Input.GetMouseButtonUp(1)) {

            }
        }
        */
}
