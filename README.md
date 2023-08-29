<h1 align="center">Unity Top-Down Click To Follow (Touch Supported)</h1>

<br>
<p align="center">
    <img src="Screenshots/example_gif.gif?raw=true">
</p>
<br/>

<hr>

## Table of Contents

- [NavMesh Setup](#navmesh-setup)
- [Player Setup](#player-setup)
    - [Movement Animations](#movement-animations)
    - [Nav Mesh Agent](#navmesh-agent)
    - [Player Controller](#player-controller)
- [Camera Setup to Follow Player](#camera-setup)

<a name="navmesh-setup"></a>
## NavMesh Setup
So the player can move to the desired destination, we will used Unity's built in navigation mesh (NavMesh).

Setting it up is pretty simple:

1. Install "AI Navigation" package from Package Manager (if you don't already have it installed). You can do this by opening _Window > Package Manager_. Choose "Unity Registry" and search for "AI Navigation".
2. In scene, create a Plane, which will be ground (_Right click on Hiearchy > 3D Object > Plane_). Make sure that Plane has a collider, so our player won't fall through later.
3. Right click on your Plane and choose _AI > NavMesh Surface_. This will create a child object with _NavMeshSurface_ component.
4. Bake the NavMesh by clicking "Bake" in the "NavMeshSurface" component.

<br>
<p align="center">
    <img src="Screenshots/navmesh_surface_component.png?raw=true">
</p>
<br/>

<br>
Generated NavMesh:
<br>
<br>
<p align="center">
    <img src="Screenshots/generated_navmesh.png?raw=true">
</p>
<br/>

<hr>

<a name="player-setup"></a>
## Player Setup
In this example I used an asset from Synty Studios called "POLYGON - Explorer Kit". You can purchase this asset [here](https://syntystore.com/products/polygon-explorer-kit). But you can use any other model you have available (it has to be properly rigged for animations to work).

<br>
<p align="center">
    <img src="Screenshots/player_model.png?raw=true">
</p>
<br/>

<hr>

<a name="movement-animations"></a>
### Movement Animations
We want 3 types of animations for our player: **idle, walking and runnning**. You can get these animation from [Mixamo](https://www.mixamo.com/) or you can use an asset from Unity Asset Store. I used [Basic Motions FREE](https://assetstore.unity.com/packages/3d/animations/basic-motions-free-154271).

1. Create a new Animator Controller in _Project > Create > Animator Controller_ and name it "PlayerAnimatorController".
2. Open the Animator window by double clicking on the Animator Controller.
3. Create a new Blend Tree by right clicking in the Animator window and choosing _Create State > From New Blend Tree_. Name it "Locomotion".

<br>
<p align="center">
    <img src="Screenshots/player_animator_base_layer.png?raw=true">
</p>
<br>

4. In the Blend Tree, create 3 motion fields and name them "Idle", "Walk" and "Run".
5. Drag and drop the animations you want to use for each motion field. For "Idle" I used "Idle" animation, for "Walk" I used "Walk" animation and for "Run" I used "Run" animation.

<br>
<p align="center">
    <img src="Screenshots/blend_tree.png?raw=true">
</p>
<br>

6. Set the "Threshold" values for each motion field. For "Idle" set it to 0, for "Walk" set it to 3 and for "Run" set it to 5. These tresholds will be used to determine which animation to play based on the player's speed.

<br>
<p align="center">
    <img src="Screenshots/tresholds.png?raw=true">
</p>
<br>

7. Rename the "Blend" parameter to "Speed" and set its default value to 0.
8. Make sure you assign this Animator Controller to your player's Animator component.

<hr>

<a name="navmesh-agent"></a>
### NavMesh Agent
We already generated a NavMesh. But for our player to be able to move on it, we need to add a NavMesh Agent component to it. This component will allow us to move the player to a desired destination on the NavMesh. You just have to add the _NavMesh Agent_ component to your player's game object.

<br>
<p align="center">
    <img src="Screenshots/player_gameobject.png?raw=true">
</p>
<br>

<hr>

<a name="player-controller"></a>
### Player Controller
Now we need to create a script that will control our player. Create a new C# script called "PlayerController" and attach it to your player's game object. 

In this script we will check for mouse/touch clicks and move the player to the clicked position. We will also update the "Speed" parameter of the Animator component, so the correct animation is played. We will take the value from current speed of the NavMesh Agent.

Open the script and add the following code:

```csharp
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Update the speed parameter of the animator for the blend tree
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);

        if (Input.mousePresent)
        {
            HandleMouseClick();
        }
        else if (Input.touchSupported)
        {
            HandleTouch();
        }
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MovePlayerToPosition(Input.mousePosition);
        }
    }

    private void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                MovePlayerToPosition(touch.position);
            }
        }
    }

    private void MovePlayerToPosition(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the touched point is on the ground
            // Add a new Tag called "Ground" and assign it to your ground object
            if (hit.collider.CompareTag("Ground"))
            {
                navMeshAgent.SetDestination(hit.point);
            }
        }
    }

}
```

<br>
<hr>

<a name="camera-setup"></a>
### Camera Setup to Follow Player
Now we need to setup the camera to follow the player. We will use the [Cinemachine](https://unity.com/unity/features/editor/art-and-design/cinemachine) package for this. You can install it from the Package Manager (_Window > Package Manager_).

1. Choose your "MainCamera" and add the _Cinemachine Brain_ component to it.
2. Create a new _Cinemachine Virtual Camera_ by right clicking in the Hierarchy and choosing _Cinemachine > Virtual Camera_. Name it "PlayerFollowCamera".
3. Assign player's game object to the "Follow" and "Look At" field of the _Cinemachine Virtual Camera_ component.
4. Set "Body" to "Transposer" and "Binding Mode" to "World Space".
5. Play with "Follow Offset". In my case, the best was to set it to (-0.4, 12, -13).

<br>
<p align="center">
    <img src="Screenshots/virtual_camera.png?raw=true">
</p>
<br/>
