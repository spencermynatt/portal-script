using UnityEngine;
using Invector.vShooter;

[RequireComponent(typeof(vShooterWeapon))] //"any GameObject with this script must also have a vShooterWeapon component, which is invektors
public class PortalGun : MonoBehaviour
{
    public float portalWidth = 1.5f;
    public float portalHeight = 2.5f;
    public Color blueColor = Color.blue;
    public Color pinkColor = Color.magenta;

    public float maxRange = 100f; //how far the gun can shoot

    private bool _nextIsBlue = true;
    private GameObject _bluePortal;
    private GameObject _pinkPortal;
    private Camera _cam; //caches reference to the main cam
    
    private bool _triggerHeld = false; //Tracks whether a controller trigger is currently being held down, to prevent firing repeatedly every frame while it stays pressed.

    void Start() => _cam = Camera.main; //Camera.main finds the scene's main camera and stores it in _cam. The => is shorthand for a one-line method body.

    void Update()
    {
        float trigger = Input.GetAxis("RT"); // reads input of rt on the controller, 0 and 1

        if (Input.GetMouseButtonDown(0) || (trigger > 0.1f && !_triggerHeld)) //checks if mouse button pressed down OR the trigger is bigger than a threshold of 0.1 and triggerheld is not not true
        {
            _triggerHeld = true;
            TryPlacePortal(); //run this function
        }
        if (trigger <= 0.1f) _triggerHeld = false; //for controller
    }

    void TryPlacePortal()
    {
        if (_cam == null) _cam = Camera.main; //if its assigned to nothing just assing it

        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        //Builds a Ray (a starting point + direction) from the camera, passing through the exact center of the screen (0.5, 0.5 in viewport coordinates) — the crosshair.
        //Casts that ray forward, up to maxRange units, checking for anything solid in its path. If it hits something, details go into hit.
        if (Physics.Raycast(ray, out RaycastHit hit, maxRange))
        {
            if (hit.collider.CompareTag("PortalSurface"))
            {
                CreatePortal(hit.point, hit.normal, _nextIsBlue); //Calls CreatePortal, passing in: where the ray hit (hit.point), which way that surface faces (hit.normal), and which color to make this portal.
                _nextIsBlue = !_nextIsBlue;
            }
        }
    }
    void CreatePortal(Vector3 pos, Vector3 normal, bool isBlue)
    {
        if (isBlue && _bluePortal != null) Destroy(_bluePortal); //before creating destroy
        if (!isBlue && _pinkPortal != null) Destroy(_pinkPortal);

        GameObject portal = new GameObject(isBlue ? "BluePortal" : "PinkPortal");
        portal.transform.position = pos + (normal * 0.05f);
        portal.transform.rotation = Quaternion.LookRotation(normal, Vector3.up);

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Quad);
        visual.transform.SetParent(portal.transform, false);
        visual.transform.localScale = new Vector3(portalWidth, portalHeight, 1f);
        Destroy(visual.GetComponent<Collider>());

        Renderer rend = visual.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        rend.material.color = isBlue ? blueColor : pinkColor;
        rend.material.SetFloat("_Cull", 0);

        BoxCollider bc = portal.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(portalWidth, portalHeight, 1.0f);
        bc.center = Vector3.zero;

        Portal pScript = portal.AddComponent<Portal>();
        pScript.isBluePortal = isBlue;

        if (isBlue) _bluePortal = portal; else _pinkPortal = portal;
    }
}