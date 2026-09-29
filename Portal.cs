using UnityEngine; //Imports Unity's core API (MonoBehaviour, Transform, Vector3, etc.).

public class Portal : MonoBehaviour
{
    public bool isBluePortal; //boolean checking which color the portal is
    private static float _globalCooldown; // A static field — meaning one value shared across every Portal instance in the scene, not per-portal. This stores a timestamp (Time.time value) before which no teleport is allowed.
    /// only the portal in this class can access this
    ///statoc means only one existance, for both blue and pink portals
    /// float just means it will include a decimal 
    ///how its used
    // ------------------
    //it just determines how long before you can use the portal again
    //_globalCooldown = Time.time + 4f;
    //if (other.CompareTag("Player") && Time.time > _globalCooldown)
    //


    private void OnTriggerStay(Collider other) //method that checks collision
                                               ///method for Trigger Zone Intersections (3D)
                                               /// Called every frame the object stays inside the zone.
                                               /// This is a Unity "message" method — you never call it yourself.Unity calls it automatically, every physics update,
                                               /// for every frame that some other collider is currently overlapping this portal's trigger collider.
                                               // other is whatever object is currently touching/inside the portal.

        //So in plain English: "Hey Portal, something is standing inside you right now — here's what it is." This fires repeatedly(many times per second) the whole time the player stands in the portal, not just once.
    {
        // Must be the player and the 0.5s cooldown must be over the time it takes
        if (other.CompareTag("Player") && Time.time > _globalCooldown) //other.CompareTag("Player") — checks whether the GameObject currently overlapping this trigger has the tag "Player"
            //BECAUSE _GLOBALCOOLDOWN IS NOT ASSIGNED TO Time.time +4f YET this is autmatically true
        {
            //Time.time — a built-in Unity property that returns the number of seconds since the game started running (as a decimal, like 47.283). It constantly increases as the game runs — it's essentially Unity's stopwatch.

            // _globalCooldown — the shared timestamp field we talked about earlier, which stores "the point in time when teleporting becomes allowed again." It's not a duration (like "4 seconds")
            
            /// — it's a specific moment on that same stopwatch(like "at the 51.283 second mark").
            /// > — the "greater than" comparison operator. The whole expression evaluates to either true or false.

           ///So Time.time > _globalCooldown literally asks: "Is the current time later than the time we're allowed to teleport again?"
            ///t's really checking Time.time > 0. in the if statement above ^
            ///we're declaring that if time.time is bigger than that float, we can collide into the portal and since its already 0 thats fine,
            ///but then later in the if statement we add 4 seconds to the current time, it makes the if statement above false.
            Portal destination = FindDestination();
            if (destination != null) //if the destination is not equal to nothing
            {
                // Set the cooldown immediately to prevent the bounce-back when we leave.
                _globalCooldown = Time.time + 4f;
                //of course Time.time will eventually surpass this so we can check this if statement forever
                // it's really checking Time.time > 0. in the if statement above ^
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
        ///setting telling Unity not to bother sorting the results(e.g., by instance ID).Sorting costs a little extra performance,
        ///and since order doesn't matter here (there are only ever two portals, blue and pink), None skips that unnecessary work.
        foreach (var p in portals)  //This loop will run twice — once with p = Object A, once with p = Object B.
        {
            if (p.isBluePortal != this.isBluePortal) return p; //(a bool field)
                                                               //not equal to blue portal, checking if its not spawning both blues or both pinks

        }
        return null; 
    }
  ///  ////Say the scene has exactly two Portal objects that exist right now:

   // Object A: the blue portal, isBluePortal = true
//    Object B: the pink portal, isBluePortal = false

///The player is standing inside Object A(the blue one), so OnTriggerStay is running on Object A, meaning this = Object A for this whole call.
    private void TeleportPlayer(GameObject player, Transform exit)
    {
        //cc is the player's movement component
        CharacterController cc = player.GetComponent<CharacterController>(); ///This is the parameter passed into TeleportPlayer(GameObject player, Transform exit) —
                                                                             ///a reference to the specific GameObject that needs to be teleported (in practice, this is
        
        if (cc != null)
            cc.enabled = false;   //Turn off the character movement  component (so it doesn't fight the move)
        ///
        //T
        /// Move the player to just in front of the exit portal
        /// Turn the player to face the same way the portal faces
        ///The reason the code needs a reference to the CharacterController in the first place is because that component actively manages the player's
        ///position/movement every frame — if you try to directly set player.transform.position (as this method does further down) while the CharacterController is still enabled, it can fight against that change,

        // SPAWN OFFSET: 1.5 units ensures you clear the 1.0 unit trigger box (so you don't teleport back)
        
        player.transform.position = exit.position + (exit.forward * 1.5f);
        //Moves the player to the exit portal's spot, then pushes them 1.5 units further out
        //exit.forward is a direction (length 1) pointing out from the portal

        /// Face the direction of the exit
        //saves exit.forward in a new vector3. Basically exit.forward 
        Vector3 exitForward = exit.forward; //forward coorresponds to the z axis.
                                            ///A Vector3 is a Unity type that stores three numbers together — X, Y, and Z — used to represent either a position or a direction in 3D space.


        
        exitForward.y = 0;


        ///Vector3 that gives you the squared length of the vector, meaning its length multiplied by itself.

        /// What "length" means for a Vector3

        ///If the exit portal's direction still has a real, usable direction after flattening it, turn the player to face that way. Otherwise, leave their rotation alone

        /// A Vector3 like(3, 0, 4) is an arrow in 3D space, and that arrow has a length.You get it using the Pythagorean theorem:

        //.sqrMagnitude

        // A property built into Vector3 that gives you the squared length of the vector — its length multiplied by itself.For a vector(x, y, z),
        //this is calculated as x² +y² +z² (no square root taken, which is what makes it faster to compute than the real length
        //exit forward is like an arrow
        // now if the portal is spawned on the wall, the arrow is pointing sideways (left,right,forward back)
        // now if the portal is on the wall, the arrow points up or down
        //sqr.Maginitude is just asking how long the arrow is
        if (exitForward.sqrMagnitude > 0.1f)
        {
            player.transform.rotation = Quaternion.LookRotation(exitForward, Vector3.up); //A Quaternion is Unity's type for storing a rotation: which way an object is oriented in 3D space. This is the single line responsible for physically turning the player
        }
        // converts it into a proper Quaternion rotation that faces the way of exitforward.
        //it — the Transform parameter passed into TeleportPlayer, representing the exit portal's position/rotation.
        //.forward — a built-in property every Transform has, giving a Vector3(a direction, length 1) pointing in whatever way that object is currently facing.
        // Vector3.up is a built-in shorthand Unity provides for the world direction straight up — specifically, the Vector3 value (0, 1, 0).
        //But a full 3D orientation needs two independent pieces of information to be fully determined: which way is forward, and which way is up relative to that forward.
        //Without the second piece, there are still infinitely many valid rotations that all share the same forward direction (you can spin around your own forward axis endlessly and still be "facing" the same way).
        //Here's the core problem it solves: imagine you're told "face north." That's one piece of information — a direction.
        ////But you could satisfy "face north" in infinitely many ways: standing upright and facing north, lying on your back and facing north, lying on your side and facing north,
        /////doing a handstand while facing north. All of these technically point "north," but they're wildly different orientations.

        if (cc != null) cc.enabled = true;


        //Portal on a wall, facing straight out: exit.forward might be (0, 0, 1)
        //After y = 0, it's still (0, 0, 1). sqrMagnitude = 1. That's greater than 0.1, so the rotation runs.
        //Portal on a floor, facing straight up: exit.forward is (0, 1, 0). After y = 0, it becomes (0, 0, 0). sqrMagnitude = 0. That's not greater than 0.1, so the rotation is skipped.
    }
}