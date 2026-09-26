using UnityEngine; //Imports Unity's core API (MonoBehaviour, Transform, Vector3, etc.).

public class Portal : MonoBehaviour
{
    public bool isBluePortal; //boolean checking which color the portal is
    private static float _globalCooldown; // A static field — meaning one value shared across every Portal instance in the scene, not per-portal. This stores a timestamp (Time.time value) before which no teleport is allowed.
    // only the portal in this class can access this
    //statoc means only one existance, for both blue and pink portals
    // float just means it will include a decimal 
    //how its used
    // ------------------
    //it just determines how long before you can use the portal again
    //_globalCooldown = Time.time + 4f;
    //if (other.CompareTag("Player") && Time.time > _globalCooldown)
    //


    private void OnTriggerStay(Collider other) //method that checks collision
                                               //method for Trigger Zone Intersections (3D)
                                               // Called every frame the object stays inside the zone.
                                               // This is a Unity "message" method — you never call it yourself.Unity calls it automatically, every physics update,
                                               // for every frame that some other collider is currently overlapping this portal's trigger collider.
                                               // other is whatever object is currently touching/inside the portal.

    //So in plain English: "Hey Portal, something is standing inside you right now — here's what it is." This fires repeatedly(many times per second) the whole time the player stands in the portal, not just once.
    {
        // Must be the player and the 0.5s cooldown must be over
        if (other.CompareTag("Player") && Time.time > _globalCooldown) //other.CompareTag("Player") — checks whether the GameObject currently overlapping this trigger has the tag "Player"
        {
            //Time.time — a built-in Unity property that returns the number of seconds since the game started running (as a decimal, like 47.283). It constantly increases as the game runs — it's essentially Unity's stopwatch.
            // _globalCooldown — the shared timestamp field we talked about earlier, which stores "the point in time when teleporting becomes allowed again." It's not a duration (like "4 seconds")
            // — it's a specific moment on that same stopwatch(like "at the 51.283 second mark").
            // > — the "greater than" comparison operator. The whole expression evaluates to either true or false.

            //So Time.time > _globalCooldown literally asks: "Is the current time later than the time we're allowed to teleport again?"
            //t's really checking Time.time > 0. in the if statement above ^
            //we're declaring that if time.time is bigger than that float, we can collide into the portal and since its already 0 thats fine,
            ///but then later in the if statement we add 4 seconds to the current time, it makes the if statement above false.
            Portal destination = FindDestination();
            if (destination != null) //if the destination is not equal to nothing
            {
                // Set the cooldown immediately to prevent the bounce-back
                _globalCooldown = Time.time + 4f;
                //of course Time.time will eventually surpass this so we can check this if statement forever
                // //t's really checking Time.time > 0. in the if statement above ^
                //we're declaring that if time.time is bigger than that float, we can collide into the portal and since its already 0 thats fine,
                ///but then later in the if statement we add 4 seconds to the current time, it makes the if statement above false.
                TeleportPlayer(other.gameObject, destination.transform);
            }
        }
    }

    private Portal FindDestination() //this function is totally useless it only checks the color not really anything else, find destination just refers to the other portal being a different color. 
        // IT HAS to be the color pink in the for loop and that pink will determine the destination.transform
        //and its called in teleport player
    {
        Portal[] portals = Object.FindObjectsByType<Portal>(FindObjectsSortMode.None);
        //This returns an array containing both objects — something like [Object A, Object B] (order isn't guaranteed, but let's say this order for this example).
        //Declares an array of portals, and stores all the objects named portal.
        //Object.FindObjectsByType<Portal>(...) — a Unity method that searches the entire loaded scene for every active component of type Portal and returns them all as an array.
        //setting telling Unity not to bother sorting the results(e.g., by instance ID).Sorting costs a little extra performance,
        //and since order doesn't matter here (there are only ever two portals, blue and pink), None skips that unnecessary work.
        foreach (var p in portals)  //This loop will run twice — once with p = Object A, once with p = Object B.
        {
            if (p.isBluePortal != this.isBluePortal) return p; //(a bool field)
                                                               //not equal to blue portal, checking if its not spawning both blues or both pinks

        }
        return null; 
    }
    //Say the scene has exactly two Portal objects that exist right now:

   // Object A: the blue portal, isBluePortal = true
//    Object B: the pink portal, isBluePortal = false

//The player is standing inside Object A(the blue one), so OnTriggerStay is running on Object A, meaning this = Object A for this whole call.
    private void TeleportPlayer(GameObject player, Transform exit)
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // SPAWN OFFSET: 1.5 units ensures you clear the 1.0 unit trigger box
        player.transform.position = exit.position + (exit.forward * 1.5f);

        // Face the direction of the exit
        Vector3 exitForward = exit.forward;
        exitForward.y = 0;
        if (exitForward.sqrMagnitude > 0.1f)
        {
            player.transform.rotation = Quaternion.LookRotation(exitForward, Vector3.up);
        }

        if (cc != null) cc.enabled = true;
    }
}