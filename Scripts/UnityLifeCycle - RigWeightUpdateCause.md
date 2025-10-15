# Unity Execution Order Reference

A comprehensive guide to Unity's frame update cycle and script lifecycle execution order with visual diagrams.

---

## Complete Execution Order Diagram

```cs
═══════════════════════════════════════════════════════════════════════════════
                         UNITY EXECUTION ORDER - COMPLETE FLOW
═══════════════════════════════════════════════════════════════════════════════
 
 set the [DefaultExecutionOrder(-1000)] // if required for a certain monobehaviour script to run as first MonoBehaviour script to run, after UnityEngine Initialization
┌─────────────────────────────────────────────────────────────────────────────┐
│                      INITIALIZATION (Once Per Script)                       │ 
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
                              ┌───────────────┐
                              │   Awake()     │ ◄─── First function called
                              └───────┬───────┘      Called even if disabled
                                      │              Initialize references
                                      ▼
                              ┌───────────────┐
                              │  OnEnable()   │ ◄─── When object is enabled
                              └───────┬───────┘      Called after Awake()
                                      │              Can be called multiple times
                                      ▼
                              ┌───────────────┐
                              │   Start()     │ ◄─── Before first Update()
                              └───────┬───────┘      Only if enabled
                                      │              Initialize dependencies
                                      │
┌─────────────────────────────────────┴───────────────────────────────────────┐
│                                                                             │
│                         MAIN GAME LOOP (Every Frame)                        │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
                         ┌────────────────────────┐
                         │   INPUT PROCESSING     │
                         │  Unity reads all input │
                         └──────────┬─────────────┘
                                    │
                                    ▼
        ╔═══════════════════════════════════════════════════════╗
        ║          PHYSICS LOOP (0, 1, or Multiple Times)       ║
        ║              Based on Time.fixedDeltaTime             ║
        ╚═══════════════════════════════════════════════════════╝
                                    │
                    ┌───────────────┼───────────────┐
                    ▼               ▼               ▼
            ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
            │ FixedUpdate()│ │ FixedUpdate()│ │ FixedUpdate()│
            └──────┬───────┘ └──────┬───────┘ └──────┬───────┘
                   │                │                │
                   └────────────────┬────────────────┘
                   ▼                ▼                ▼
            ┌──────────────────────────────────────────────┐
            │  Physics Simulation Step                     │
            │  - Apply forces                              │
            │  - Collision detection                       │
            │  - Rigidbody calculations                    │
            └──────────────────┬───────────────────────────┘
                               │
                ┌──────────────┼──────────────┐
                ▼              ▼              ▼
        ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
        │OnTriggerEnter│ │OnCollisionXXX│ │OnTriggerStay │
        │OnTriggerExit │ │  Enter/Stay  │ │              │
        │              │ │     /Exit    │ │              │
        └──────────────┘ └──────────────┘ └──────────────┘
                               │
                               │
        ╔══════════════════════╧════════════════════════╗
        ║           END OF PHYSICS LOOP                 ║
        ╚═══════════════════════════════════════════════╝
                               │
                               ▼
                      ┌─────────────────┐
                      │    Update()     │ ◄─── Main game logic
                      │                 │      Called once per frame
                      └────────┬────────┘      Frame-rate dependent
                               │
                               ▼
                      ┌─────────────────┐
                      │   yield return  │ ◄─── Coroutines evaluate
                      │  Coroutines     │      yield instructions
                      └────────┬────────┘
                               │
                               ▼
        ╔══════════════════════════════════════════════════╗
        ║              ANIMATION SYSTEM                    ║
        ╚══════════════════════════════════════════════════╝
                               │
                      ┌────────┴────────┐
                      ▼                 ▼
            ┌──────────────────┐  ┌──────────────────┐
            │ Animator Update  │  │  State Machine   │
            │                  │  │   Evaluation     │
            └────────┬─────────┘  └────────┬─────────┘
                     │                     │
                     └──────────┬──────────┘
                                ▼
                    ┌───────────────────────┐
                    │ Animation Sampling    │
                    │ & Blending            │
                    └──────────┬────────────┘
                               │
                               ▼
                    ┌───────────────────────┐
                    │ ⚠️ ANIMATION EVENTS   │ ◄─── CRITICAL!
                    │    FIRE HERE          │      Events fire DURING
                    └──────────┬────────────┘      animation processing
                               │
                               ▼
                    ┌───────────────────────┐
                    │ Apply Animation to    │
                    │    Transforms         │
                    └──────────┬────────────┘
                               │
        ╔══════════════════════╧════════════════════════╗
        ║           END OF ANIMATION SYSTEM             ║
        ╚═══════════════════════════════════════════════╝
                               │
                               ▼
                      ┌─────────────────┐
                      │  LateUpdate()   │ ◄─── After all Updates
                      │                 │      Camera follow
                      └────────┬────────┘      Post-processing
                               │
                               ▼
                      ┌─────────────────┐
                      │ OnAnimatorIK()  │ ◄─── If IK Pass enabled
                      │  (per layer)    │      Humanoid IK
                      └────────┬────────┘
                               │
                               ▼
        ╔══════════════════════════════════════════════════╗
        ║         ANIMATION RIGGING & IK SYSTEM            ║
        ╚══════════════════════════════════════════════════╝
                               │
                      ┌────────┴────────┐
                      ▼                 ▼
            ┌──────────────────┐  ┌──────────────────┐
            │  Rig Evaluation  │  │ IK Constraints   │
            │                  │  │   Processing     │
            └────────┬─────────┘  └────────┬─────────┘
                     │                     │
                     └──────────┬──────────┘
                                ▼
                    ┌───────────────────────┐
                    │   Rig.weight          │ ◄─── Weight applied HERE
                    │   Applied             │      After animation
                    └──────────┬────────────┘
                               │
        ╔══════════════════════╧════════════════════════╗
        ║           END OF IK SYSTEM                    ║
        ╚═══════════════════════════════════════════════╝
                               │
                               ▼
        ╔══════════════════════════════════════════════════╗
        ║              RENDERING PREPARATION               ║
        ╚══════════════════════════════════════════════════╝
                               │
                      ┌────────┴────────┐
                      ▼                 ▼
            ┌──────────────────┐  ┌──────────────────┐
            │OnWillRenderObject│  │  OnBecameVisible │
            │                  │  │ OnBecameInvisible│
            └────────┬─────────┘  └────────┬─────────┘
                     │                     │
                     └──────────┬──────────┘
                                ▼
                      ┌──────────────────┐
                      │   OnPreCull()    │
                      └────────┬─────────┘
                               ▼
                      ┌──────────────────┐
                      │  OnPreRender()   │
                      └────────┬─────────┘
                               │
                               ▼
        ╔══════════════════════════════════════════════════╗
        ║                   RENDERING                      ║
        ╚══════════════════════════════════════════════════╝
                               │
                      ┌────────┴────────┐
                      ▼                 ▼
            ┌──────────────────┐  ┌──────────────────┐
            │  Scene Rendering │  │ Camera Culling   │
            │                  │  │   Calculations   │
            └────────┬─────────┘  └────────┬─────────┘
                     │                     │
                     └──────────┬──────────┘
                                ▼
                      ┌──────────────────┐
                      │ OnRenderObject() │
                      └────────┬─────────┘
                               ▼
                      ┌──────────────────┐
                      │ OnPostRender()   │
                      └────────┬─────────┘
                               ▼
                      ┌──────────────────┐
                      │ OnRenderImage()  │ ◄─── Post-processing
                      └────────┬─────────┘
                               │
        ╔══════════════════════╧════════════════════════╗
        ║              END OF RENDERING                 ║
        ╚═══════════════════════════════════════════════╝
                               │
                               ▼
        ╔══════════════════════════════════════════════════╗
        ║               GIZMOS & GUI                       ║
        ╚══════════════════════════════════════════════════╝
                               │
                      ┌────────┴────────┐
                      ▼                 ▼
            ┌──────────────────┐  ┌──────────────────┐
            │  OnDrawGizmos()  │  │OnDrawGizmosSelect│
            │                  │  │                  │
            └──────────────────┘  └──────────────────┘
                               │
                               ▼
                      ┌──────────────────┐
                      │    OnGUI()       │ ◄─── Legacy UI
                      │ (Multiple times) │      Layout + Repaint
                      └────────┬─────────┘
                               │
                               ▼
        ╔══════════════════════════════════════════════════╗
        ║               END OF FRAME                       ║
        ╚══════════════════════════════════════════════════╝
                               │
                               ▼
                      ┌──────────────────┐
                      │ WaitForEndOfFrame│ ◄─── Coroutines resume
                      │  yield resumes   │      Screenshots, etc.
                      └────────┬─────────┘
                               │
                               │
                      ┌────────┴────────┐
                      │                 │
                      │  Frame Complete │
                      │   Loop Repeats  │
                      │                 │
                      └────────┬────────┘
                               │
                               │ (Next Frame)
                               └──────────┐
                                          │
                                          ▼
                              ┌───────────────────┐
                              │  INPUT PROCESSING │
                              │   (Next Frame)    │
                              └───────────────────┘


┌─────────────────────────────────────────────────────────────────────────────┐
│                    DEINITIALIZATION (When Destroyed)                        │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
                              ┌───────────────┐
                              │  OnDisable()  │ ◄─── When disabled
                              └───────┬───────┘      Or before destroy
                                      │
                                      ▼
                              ┌───────────────┐
                              │  OnDestroy()  │ ◄─── When destroyed
                              └───────────────┘      Final cleanup

═══════════════════════════════════════════════════════════════════════════════
```

---

## Mouse Events Flow Diagram

```
════════════════════════════════════════════════════════════════════════
                        MOUSE INTERACTION FLOW
════════════════════════════════════════════════════════════════════════

                    Mouse moves toward GameObject
                                  │
                                  ▼
                        ┌──────────────────┐
                        │ OnMouseEnter()   │ ◄─── Cursor enters collider
                        └────────┬─────────┘
                                 │
                                 ▼
                        ┌──────────────────┐
                        │  OnMouseOver()   │ ◄─── Every frame while over
                        └────────┬─────────┘      (repeats)
                                 │
                        ┌────────┴────────┐
                        │                 │
            Mouse button pressed?         Mouse moves away?
                        │                 │
                 ┌──────┴──────┐          │
                 │ YES         │ NO       │
                 ▼             │          ▼
        ┌──────────────┐       │  ┌──────────────┐
        │OnMouseDown() │       │  │OnMouseExit() │ ◄─── Cursor leaves
        └──────┬───────┘       │  └──────────────┘
               │               │
               ▼               │
     Button held + moving?     │
               │               │
        ┌──────┴──────┐        │
        │ YES         │ NO     │
        ▼             │        │
  ┌──────────────┐    │        │
  │ OnMouseDrag()│    │        │ ◄─── Every frame while dragging
  │  (repeats)   │    │        │
  └──────┬───────┘    │        │
         │            │        │
         ▼            ▼        │
        Button released?       │
                │              │
                ▼              │
       ┌──────────────────┐    │
       │   OnMouseUp()    │    │ ◄─── Button released
       └────────┬─────────┘    │
                │              │
      Still over same collider?│
                │              │
         ┌──────┴──────┐       │
         │ YES         │ NO    │
         ▼             │       │
  ┌──────────────────┐ │       │
  │OnMouseUpAsButton │ │       │ ◄─── Released over same object
  └──────────────────┘ │       │
                       │       │
                       └───────┴───────► (Continue or Exit)

════════════════════════════════════════════════════════════════════════
```

---

## Physics Events Flow Diagram

```
════════════════════════════════════════════════════════════════════════
                    PHYSICS COLLISION & TRIGGER FLOW
════════════════════════════════════════════════════════════════════════

                        FixedUpdate() called
                                │
                                ▼
                    ┌───────────────────────┐
                    │  Physics Simulation   │
                    │        Step           │
                    └──────────┬────────────┘
                               │
                ┌──────────────┴──────────────┐
                │                             │
        Collision Detected?          Trigger Overlap?
                │                             │
         ┌──────┴──────┐              ┌──────┴──────┐
         │ YES         │ NO           │ YES         │ NO
         ▼             │              ▼             │
    ┌────────────┐     │          ┌────────────┐    │
    │  COLLISION │     │          │  TRIGGER   │    │
    │   EVENTS   │     │          │   EVENTS   │    │
    └─────┬──────┘     │          └─────┬──────┘    │
          │            │                │           │
          │            │                │           │
    First frame?       │          First frame?      │
          │            │                │           │
    ┌─────┴─────┐      │          ┌─────┴─────┐     │
    │ YES       │ NO   │          │ YES       │ NO  │
    ▼           │      │          ▼           │     │
┌──────────────┐│      │      ┌──────────────┐│     │
│OnCollision   ││      │      │  OnTrigger   ││     │
│  Enter()     ││      │      │   Enter()    ││     │
└──────┬───────┘│      │      └──────┬───────┘│     │
       │        │      │             │        │     │
       └────────┼──────┘             └────────┼─────┘
                │                             │
                ▼                             ▼
    Still in contact?             Still overlapping?
                │                             │
         ┌──────┴──────┐              ┌──────┴──────┐
         │ YES         │ NO           │ YES         │ NO
         ▼             │              ▼             │
┌──────────────┐       │       ┌──────────────┐     │
│OnCollision   │       │       │  OnTrigger   │     │
│  Stay()      │       │       │   Stay()     │     │
│  (repeats)   │       │       │  (repeats)   │     │
└──────┬───────┘       │       └──────┬───────┘     │
       │               │              │             │
       └───────────────┘              └─────────────┘
                │                             │
                ▼                             ▼
       Separation detected?         Separation detected?
                │                             │
                ▼                             ▼
       ┌──────────────┐              ┌──────────────┐
       │OnCollision   │              │  OnTrigger   │
       │   Exit()     │              │   Exit()     │
       └──────────────┘              └──────────────┘

════════════════════════════════════════════════════════════════════════
```

---

## Animation Rigging Problem - Visual Timeline

```
════════════════════════════════════════════════════════════════════════
              WHY ANIMATION EVENTS NEED COROUTINE DELAY
════════════════════════════════════════════════════════════════════════

✓ WORKS: Input in Update()
─────────────────────────────
Frame N:
  │
  ├─► 1. INPUT PROCESSING
  │
  ├─► 2. Update()
  │      │
  │      └─► if (Input.GetKeyDown(KeyCode.I))
  │              _rig.weight = 1f;  ✓ Value set
  │
  ├─► 3. Animation System
  │      (doesn't change _rig.weight)
  │
  ├─► 4. LateUpdate()
  │
  ├─► 5. IK/Rigging System
  │      │
  │      └─► Reads _rig.weight = 1f  ✓ Correct!
  │
  └─► 6. Render Frame ✓


✗ FAILS: Animation Event Direct Call
──────────────────────────────────────
Frame N:
  │
  ├─► 1. INPUT PROCESSING
  │
  ├─► 2. Update()
  │
  ├─► 3. Animation System
  │      │
  │      ├─► Animation samples frame
  │      │
  │      ├─► ⚠️ ANIMATION EVENT FIRES
  │      │      │
  │      │      └─► AnimationEvent_ResetRig()
  │      │              _rig.weight = 1f;  ✓ Value set
  │      │
  │      └─► Animation continues processing
  │              _rig.weight = 0f;  ✗ OVERWRITTEN!
  │
  ├─► 4. LateUpdate()
  │
  ├─► 5. IK/Rigging System
  │      │
  │      └─► Reads _rig.weight = 0f  ✗ Wrong!
  │
  └─► 6. Render Frame ✗ Incorrect


Great question! Let me clarify - the "overwritten" scenario I described is actually a **potential issue** depending on your specific animation setup, not a guaranteed behavior in all cases.

Let me explain what **actually happens** and when the overwrite occurs:

## What Actually Happens With Animation Rigging

The real issue isn't that the animation system directly overwrites `_rig.weight = 0f`. The problem is more subtle:

### The Real Problem: **Timing of Constraint Evaluation**

```csharp
Frame N:
  Update()
  
  Animation System Updates:
    ├─► Animation samples
    ├─► Animation Event Fires
    │     └─► _rig.weight = 1f;  ✓ You set it to 1
    └─► Animation applies to transforms
  
  LateUpdate()
  
  Animation Rigging System Evaluates:
    └─► Reads _rig.weight to determine constraint strength
        BUT: The rig system may not properly register 
        changes made DURING animation processing
```

### Why Does It Appear "Overwritten"?

There are several potential scenarios where your change gets lost:

#### Scenario 1: **Rig System Caches Values Before Animation Events**
```csharp
// Inside Unity's Animation Rigging system (conceptual):

PreAnimationUpdate()
{
    cachedRigWeight = _rig.weight;  // Caches BEFORE animation events
}

AnimationUpdate()
{
    // Your animation event fires here
    _rig.weight = 1f;  // You change it
}

RigEvaluation()
{
    float actualWeight = cachedRigWeight;  // Uses OLD cached value!
    // Your change is ignored
}
```

#### Scenario 2: **Animation Curves Controlling Rig Weight**
```csharp
// If you have an animation curve controlling the rig:

Animation Event Fires:
    _rig.weight = 1f;  ✓ Set to 1

Animation Continues Processing:
    Animation Curve for _rig.weight at current keyframe = 0f
    _rig.weight = 0f;  ✗ Animation curve overwrites your value
```

#### Scenario 3: **Constraint Graph Update Order**

Unity's Animation Rigging uses a constraint graph that evaluates in a specific order. Changes made during animation events might not be "seen" by the constraint system because:

```csharp
// Simplified constraint evaluation:

BuildConstraintGraph()  // Before animation
{
    // Graph built with current rig.weight
}

Animation Updates → Events Fire
{
    _rig.weight = 1f;  // You change it
}

EvaluateConstraintGraph()  // After animation
{
    // Graph was already built with old weight
    // Change doesn't affect this frame's evaluation
}
```

## The Actual Behavior in Your Code

Looking at your code more carefully:

```csharp
void ReloadWeapon()
{
    _rig.weight = 0f; // disable aim, IK constraint
    this._animator.SetTrigger("reload");
}
```

The reload animation probably:
1. Sets `_rig.weight = 0f` at the start
2. Has an Animation Event at the end calling `ResetRigWeightForIK()`
3. Should restore `_rig.weight = 1f`

**The issue is:** When the Animation Event fires and you try to set `_rig.weight = 1f`, the Animation Rigging system has already prepared its constraint evaluation for this frame using the old `0f` value.

## Why KeyDown Works But Animation Events Don't

```csharp
✓ KeyDown in Update():
  Update()
    _rig.weight = 1f;  // Set BEFORE animation system runs
  
  Animation System
    // Reads the new value
  
  Rigging System
    // Uses the value that animation system saw


✗ Animation Event:
  Animation System
    (Rigging system prepares with old value)
    Animation Event → _rig.weight = 1f;  // Too late!
    (Rigging already computed for this frame)
  
  Rigging System
    // Uses pre-computed values, ignores your change
```

## The Real Solution: Frame Delay

By waiting one frame, you ensure the change happens **before** any systems process it:

```csharp
IEnumerator RestoreRigWeightCoroutine()
{
    yield return null; // Wait for next frame
    
    // Now we're at the START of the next frame
    // BEFORE animation and rigging systems run
    _rig.weight = 1f;  ✓ Will be seen by all systems
}
```

## Summary

The "overwrite" isn't a literal `_rig.weight = 0f` line of code in Unity. Instead:

1. **Timing Issue**: Changes during animation events happen too late in the frame
2. **Cached Values**: Rigging system may cache weights before events fire
3. **Graph Pre-computation**: Constraint graphs built before your change
4. **Animation Curves**: If present, they can literally overwrite your value

**The core issue:** Animation Events fire in the **middle** of a complex update chain, and systems that need to read `_rig.weight` have already prepared their data for the current frame.

Does this clarify the actual mechanism? The "overwrite" is more about timing and system update order than a specific line of code setting it back to 0.



# Solution:

## The Answer: **Option 2** - Call With Coroutine Delay Inside AnimationEvent Only for Animation Rigging

**You only need the coroutine delay when modifying Animation Rigging components** (like `Rig.weight`, constraint properties, etc.)

## The Rule:

```csharp
❌ Need Delay:
- Rig.weight
- Constraint properties (TwoBoneIKConstraint, MultiAimConstraint, etc.)
- Anything in UnityEngine.Animations.Rigging namespace

✓ No Delay Needed:
- Animator.SetTrigger()
- Animator.SetBool()
- Animator.SetFloat()
- Regular GameObject manipulation
- Playing sounds
- Spawning particles
- Most other logic
```

## Examples:

### ✓ No Delay Needed (Works Fine):

```csharp
// PlayerAnimationEventsManager.cs

public void PlayFootstepSound()
{
    audioSource.PlayOneShot(footstepClip);  // ✓ Works immediately
}

public void SpawnDustParticle()
{
    Instantiate(dustPrefab, transform.position, Quaternion.identity);  // ✓ Works
}

public void SetWeaponActive()
{
    weapon.SetActive(true);  // ✓ Works
}

public void TriggerNextAnimation()
{
    animator.SetTrigger("nextAnim");  // ✓ Works
}
```

### ❌ Needs Delay (Animation Rigging):

```csharp
// PlayerAnimationEventsManager.cs

public void ResetRigWeight()
{
    // ❌ Won't work properly
    // _weaponVisualController._rig.weight = 1f;
    
    // ✓ Correct way
    StartCoroutine(ResetRigWeightCoroutine());
}

IEnumerator ResetRigWeightCoroutine()
{
    yield return null;
    _weaponVisualController._rig.weight = 1f;
}

public void EnableIKConstraint()
{
    // ❌ Won't work properly
    // leftHandIKConstraint.weight = 1f;
    
    // ✓ Correct way
    StartCoroutine(EnableIKCoroutine());
}

IEnumerator EnableIKCoroutine()
{
    yield return null;
    leftHandIKConstraint.weight = 1f;
}
```

## Your Specific Code:

```csharp
// PlayerAnimationEventsManager.cs
public class PlayerAnimationEventsManager : MonoBehaviour
{
    [SerializeField] WeaponVisualController _weaponVisualController;

    // This deals with Rig.weight → Needs delay ❌
    public void ResetRigWeightForIK()
    {
        StartCoroutine(ResetRigCoroutine());
    }
    
    IEnumerator ResetRigCoroutine()
    {
        yield return null;
        this._weaponVisualController._rig.weight = 1f;
    }

    // These would work fine without delay ✓
    public void PlayReloadSound()
    {
        audioSource.Play();  // No delay needed
    }
    
    public void EjectShellCasing()
    {
        Instantiate(shellPrefab, ejectionPoint.position, ejectionPoint.rotation);  // No delay
    }
}
```

## Quick Decision Tree:

```
Is the Animation Event modifying...
│
├─► Rig.weight?                    → YES → Use coroutine delay
├─► IK Constraint properties?      → YES → Use coroutine delay
├─► Anything in Rigging namespace? → YES → Use coroutine delay
│
└─► Everything else?               → NO  → Call directly, no delay needed
```

## **In a Nutshell:**

**Only use coroutine delay for Animation Rigging components. Everything else works fine directly in Animation Events.**

The vast majority of your Animation Event callbacks can be called directly. It's only the IK/Rigging system that has this specific timing quirk.




```cs
✓ SOLUTION: Coroutine Delay
────────────────────────────
Frame N:
  │
  ├─► 1. INPUT PROCESSING
  │
  ├─► 2. Update()
  │
  ├─► 3. Animation System
  │      │
  │      ├─► Animation samples frame
  │      │
  │      └─► ⚠️ ANIMATION EVENT FIRES
  │              │
  │              └─► AnimationEvent_ResetRig()
  │                      StartCoroutine(RestoreRigWeight());
  │                      (Coroutine registered, waits)
  │
  ├─► 4. LateUpdate()
  │
  ├─► 5. IK/Rigging System
  │      (Still using old weight)
  │
  └─► 6. Render Frame

Frame N+1:
  │
  ├─► 1. INPUT PROCESSING
  │
  ├─► 2. Update()
  │      │
  │      └─► yield return null completed
  │              _rig.weight = 1f;  ✓ Value set safely
  │
  ├─► 3. Animation System
  │      (Animation completed, no interference)
  │
  ├─► 4. LateUpdate()
  │
  ├─► 5. IK/Rigging System
  │      │
  │      └─► Reads _rig.weight = 1f  ✓ Correct!
  │
  └─► 6. Render Frame ✓


✓ ALTERNATIVE: LateUpdate Flag
───────────────────────────────
Frame N:
  │
  ├─► 1. INPUT PROCESSING
  │
  ├─► 2. Update()
  │
  ├─► 3. Animation System
  │      │
  │      └─► ⚠️ ANIMATION EVENT FIRES
  │              │
  │              └─► AnimationEvent_ResetRig()
  │                      shouldRestoreRig = true;  ✓ Flag set
  │
  ├─► 4. LateUpdate()
  │      │
  │      └─► if (shouldRestoreRig)
  │              _rig.weight = 1f;  ✓ Set after animation
  │              shouldRestoreRig = false;
  │
  ├─► 5. IK/Rigging System
  │      │
  │      └─► Reads _rig.weight = 1f  ✓ Correct!
  │
  └─► 6. Render Frame ✓

════════════════════════════════════════════════════════════════════════
```

---

## Detailed Function Reference

### Initialization (One-Time, In Order)

```
Awake() 
  → Called when script instance is loaded
  → Runs even if GameObject is disabled
  → Use for self-initialization, setting up references
  → Guaranteed to run before any Start()
  → All Awake() calls complete before any OnEnable()

OnEnable()
  → Called when GameObject/Component becomes active
  → Runs after Awake()
  → Called every time object is re-enabled
  → Use for registering event listeners

Start()
  → Called before first frame Update()
  → Only runs if Component is enabled
  → Runs after all Awake() and OnEnable() calls
  → Use for initialization that depends on other objects
```

### Per-Frame Update Cycle

```
INPUT PROCESSING
  → Unity captures all input states
  → Keyboard, mouse, touch, gamepad

FixedUpdate() [0 to N times per frame]
  → Fixed timestep (default 0.02s = 50fps)
  → Independent of frame rate
  → Use for physics calculations
  → Can run multiple times if frame rate is low
  → May skip if frame rate is very high

Physics Simulation
  → Apply forces to Rigidbodies
  → Collision detection
  → Constraint solving

OnTriggerXXX() / OnCollisionXXX()
  → Fired during physics simulation
  → Order: Enter → Stay (repeating) → Exit
  → Requires Collider + Rigidbody

Update()
  → Main game logic
  → Called once per frame
  → Frame-rate dependent
  → Use Time.deltaTime for frame-independent behavior

Internal Animation Update
  → Animator evaluates state machine
  → Animation blending calculations

Animation Sampling
  → Current frame of animation calculated
  → Blend trees evaluated

Animation Events Fire ⚠️
  → Events execute during animation sampling
  → Synchronous execution
  → Can cause issues with IK/Rigging if not careful

Animation Applied to Transforms
  → Bone rotations/positions updated
  → Transform hierarchy modified

LateUpdate()
  → Runs after all Update() calls
  → Use for camera following (avoids jitter)
  → Good for reading final positions after Update()

OnAnimatorIK(int layerIndex)
  → Called after animation, before IK
  → Only if "IK Pass" enabled on animator layer
  → Use for procedural IK adjustments

Animation Rigging System
  → Rig constraints evaluate
  → IK chains solve
  → Rig.weight applied here ⚠️

Rendering Preparation
  → Culling calculations
  → Determine what's visible

OnWillRenderObject()
  → Called for each camera rendering this object
  
OnBecameVisible() / OnBecameInvisible()
  → When object enters/exits ANY camera view

OnPreCull() → OnPreRender()
  → Camera-specific callbacks

Rendering
  → Scene rendered to camera

OnRenderObject()
  → After scene rendering
  → Use GL class for custom rendering

OnPostRender()
  → After camera completes rendering

OnRenderImage(RenderTexture src, RenderTexture dest)
  → Post-processing effects on camera

OnDrawGizmos() / OnDrawGizmosSelected()
  → Debug visualization in editor

OnGUI()
  → Legacy UI rendering
  → Called multiple times (Layout + Repaint)

WaitForEndOfFrame
  → Coroutines with this yield resume here
  → After all rendering complete
  → Use for screenshots
```

### Deinitialization

```
OnDisable()
  → Called when GameObject/Component disabled
  → Called before OnDestroy()
  → Use for cleanup, unregistering events
  → Called when scene unloads

OnApplicationPause(bool pauseStatus)
  → Called when app loses/gains focus
  → Mobile: when minimized/restored

OnApplicationQuit()
  → Before application quits
  → Not called in Editor when stopping play

OnDestroy()
  → When GameObject is destroyed
  → Final cleanup
  → Called after OnDisable()
```

---

## Quick Reference Table

| Phase | Functions | Frequency | Use Case |
|-------|-----------|-----------|----------|
| **Initialization** | `Awake()` | Once | Self-initialization, references |
| | `OnEnable()` | On enable | Event registration |
| | `Start()` | Once (if enabled) | Dependencies initialization |
| **Physics** | `FixedUpdate()` | Fixed timestep | Physics calculations |
| | `OnCollisionXXX()` | Per collision | Collision response |
| | `OnTriggerXXX()` | Per trigger | Trigger detection |
| **Update** | `Update()` | Per frame | Main game logic |
| **Animation** | `Animation Events` | Per marker | Animation callbacks ⚠️ |
| **Late Update** | `LateUpdate()` | Per frame | Camera follow, post-process |
| | `OnAnimatorIK()` | Per frame (if enabled) | IK adjustments |
| **Rendering** | `OnWillRenderObject()` | Per camera | Pre-render logic |
| | `OnBecameVisible()` | On visible | Optimization |
| | `OnPreRender()` | Per camera | Camera setup |
| | `OnRenderObject()` | Per frame | Custom rendering |
| | `OnPostRender()` | Per camera | Post-camera effects |
| **Input** | `OnMouseXXX()` | Per event | Mouse interaction |
| **Cleanup** | `OnDisable()` | On disable | Cleanup |
| | `OnDestroy()` | On destroy | Final cleanup |

---

## Common Pitfalls & Best Practices

### ❌ Common Mistakes

1. **Physics in Update()** - Use `FixedUpdate()` instead
2. **Camera follow in Update()** - Use `LateUpdate()` to prevent jitter
3. **Direct Rig.weight in Animation Event** - Use coroutine delay or `LateUpdate()`
4. **Forgetting OnDisable()** - Always pair `OnEnable()`/`OnDisable()`
5. **Initialization dependencies in Awake()** - Use `Start()` for cross-script dependencies

### ✓ Best Practices

```csharp
// ✓ Self-initialization in Awake
void Awake()
{
    rigidbody = GetComponent<Rigidbody>();
    animator = GetComponent<Animator>();
}

// ✓ Dependencies in Start
void Start()
{
    gameManager = GameManager.Instance;
    playerController = FindObjectOfType<PlayerController>();
}

// ✓ Physics in FixedUpdate
void FixedUpdate()
{
    rb.AddForce(Vector3.forward * speed);
}

// ✓ Camera follow in LateUpdate
void LateUpdate()
{
    transform.position = target.position + offset;
}

// ✓ Animation Events with delay
public void AnimEvent_ResetRig()
{
    StartCoroutine(RestoreRigWeightCoroutine());
}

IEnumerator RestoreRigWeightCoroutine()
{
    yield return null; // Wait one frame
    _rig.weight = 1f;
}

// ✓ Event cleanup
void OnEnable()
{
    EventManager.OnGameOver += HandleGameOver;
}

void OnDisable()
{
    EventManager.OnGameOver -= HandleGameOver;
}
```

---

## Execution Order Examples

### Example 1: Simple GameObject Lifecycle

```csharp
// GameObject instantiated with this script

Awake()           // "Initializing references"
OnEnable()        // "Registering events"
Start()           // "Starting game logic"

// --- Game Loop Begins ---
Update()          // Frame 1
LateUpdate()      // Frame 1

Update()          // Frame 2
LateUpdate()      // Frame 2
// ... continues ...

// --- GameObject Disabled ---
OnDisable()       // "Unregistering events"

// --- GameObject Re-enabled ---
OnEnable()        // "Registering events again"
                  // (Note: Awake and Start don't run again)

// --- GameObject Destroyed ---
OnDisable()       // "Cleanup before destroy"
OnDestroy()       // "Final cleanup"
```

### Example 2: Physics Collision Flow

```csharp
// Two GameObjects with Colliders and Rigidbodies collide

FixedUpdate()
  Physics Simulation
    OnCollisionEnter(Collision col)  // "Hit detected!"
    
FixedUpdate()
  Physics Simulation
    OnCollisionStay(Collision col)   // "Still touching"
    
FixedUpdate()
  Physics Simulation
    OnCollisionStay(Collision col)   // "Still touching"

// Objects separate
FixedUpdate()
  Physics Simulation
    OnCollisionExit(Collision col)   // "Separated!"
```

### Example 3: Mouse Interaction Flow

```csharp
// User interacts with GameObject that has a Collider

OnMouseEnter()      // "Mouse entered"
OnMouseOver()       // "Mouse over" (repeats each frame)
OnMouseOver()       // "Mouse over"
OnMouseDown()       // "Button pressed!"
OnMouseDrag()       // "Dragging" (repeats while holding + moving)
OnMouseDrag()       // "Dragging"
OnMouseUp()         // "Button released"
OnMouseUpAsButton() // "Clicked!" (if released over same object)
OnMouseExit()       // "Mouse left"
```

### Example 4: Animation Event Problem

```csharp
// ❌ WRONG WAY - Direct call in Animation Event

Update()
Animation System
  Animation Event Fires
    → ResetRigWeight()
        _rig.weight = 1f;        // Set to 1
    → Animation continues
        _rig.weight = 0f;        // OVERWRITTEN to 0!
LateUpdate()
IK System
  → Reads _rig.weight = 0f       // ❌ Wrong value!


// ✓ CORRECT WAY - Using Coroutine

Frame N:
  Update()
  Animation System
    Animation Event Fires
      → ResetRigWeight()
          StartCoroutine(RestoreRig());  // Schedule for next frame
  LateUpdate()
  IK System
    → Reads old _rig.weight      // Still using old value

Frame N+1:
  Update()
    Coroutine Resumes (yield return null completed)
      _rig.weight = 1f;          // ✓ Set safely
  Animation System
    (No interference)
  LateUpdate()
  IK System
    → Reads _rig.weight = 1f     // ✓ Correct value!
```

---

## Advanced Timing Scenarios

### Scenario 1: Multiple FixedUpdates in One Frame

```
Low framerate (e.g., 20fps) with default physics (50fps):

Frame Start (0.05s elapsed since last frame)
  FixedUpdate() #1 (0.00s - 0.02s)
  FixedUpdate() #2 (0.02s - 0.04s)
  FixedUpdate() #3 (0.04s - 0.05s, partial)
  Update()
  LateUpdate()
  Render
Frame End
```

### Scenario 2: Coroutine Timing

```csharp
void Start()
{
    StartCoroutine(ExampleCoroutine());
}

IEnumerator ExampleCoroutine()
{
    Debug.Log("Start");           // Executes immediately in Start()
    
    yield return null;            // Waits until next frame's Update
    Debug.Log("After 1 frame");   // Next frame's Update phase
    
    yield return new WaitForSeconds(2f);  // Waits 2 seconds
    Debug.Log("After 2 seconds"); // 2 seconds later in Update phase
    
    yield return new WaitForFixedUpdate();
    Debug.Log("After FixedUpdate"); // After next FixedUpdate
    
    yield return new WaitForEndOfFrame();
    Debug.Log("After rendering"); // After frame fully rendered
}
```

**Timeline:**
```
Frame 0:
  Start()
    → "Start" logged
  Update()
    → yield return null completes
    → "After 1 frame" logged
    → yield return WaitForSeconds(2f) starts
  
Frame 1-120: (assuming 60fps = ~2 seconds)
  Update()
    → Waiting...

Frame 121:
  Update()
    → 2 seconds elapsed
    → "After 2 seconds" logged
    → yield return WaitForFixedUpdate() starts
  FixedUpdate()
    → yield completes
  Update() (continues after FixedUpdate)
    → "After FixedUpdate" logged
    → yield return WaitForEndOfFrame() starts
  Rendering...
  End of Frame
    → "After rendering" logged
```

---

## Physics Timing Details

### Collision Event Order

```csharp
// Two objects collide with different collision types

Object A: Collider (non-trigger)
Object B: Collider (trigger)

FixedUpdate() [Frame N]
  Physics Simulation
    → No collision yet
    
FixedUpdate() [Frame N+1]
  Physics Simulation
    → Collision detected!
    → OnCollisionEnter(Collision col)  // A hits B (non-trigger collision)
    
FixedUpdate() [Frame N+2]
  Physics Simulation
    → Still colliding
    → OnCollisionStay(Collision col)   // Continuous contact
    
FixedUpdate() [Frame N+3]
  Physics Simulation
    → Separated
    → OnCollisionExit(Collision col)   // No longer touching
```

### Trigger vs Collision

```
Trigger (isTrigger = true):
  - No physical collision response
  - Objects pass through each other
  - OnTriggerEnter/Stay/Exit called
  - Useful for: Zones, pickups, detection areas

Collision (isTrigger = false):
  - Physical collision response
  - Objects bounce/stop
  - OnCollisionEnter/Stay/Exit called
  - Provides Collision data (contact points, impulse)
  - Useful for: Physical interactions
```

---

## Unity 2D vs 3D Functions

| 3D Function | 2D Equivalent | Notes |
|-------------|---------------|-------|
| `OnCollisionEnter(Collision)` | `OnCollisionEnter2D(Collision2D)` | 2D uses Collision2D |
| `OnCollisionStay(Collision)` | `OnCollisionStay2D(Collision2D)` | |
| `OnCollisionExit(Collision)` | `OnCollisionExit2D(Collision2D)` | |
| `OnTriggerEnter(Collider)` | `OnTriggerEnter2D(Collider2D)` | 2D uses Collider2D |
| `OnTriggerStay(Collider)` | `OnTriggerStay2D(Collider2D)` | |
| `OnTriggerExit(Collider)` | `OnTriggerExit2D(Collider2D)` | |
| `Rigidbody` | `Rigidbody2D` | Different component |
| `Collider` | `Collider2D` | Different component |

---

## Performance Considerations

### What Runs Every Frame?

```
HIGH FREQUENCY (Every Frame):
✓ Update()
✓ LateUpdate()
✓ OnGUI() (multiple times!)
✓ OnMouseOver() (if mouse is over)
✓ OnCollisionStay() / OnTriggerStay()

VARIABLE FREQUENCY:
~ FixedUpdate() (0 to N times per frame)

LOW FREQUENCY (On Event):
○ Awake() (once)
○ Start() (once)
○ OnEnable() / OnDisable() (on state change)
○ OnCollisionEnter() / OnTriggerEnter() (on event)
○ OnMouseEnter() / OnMouseExit() (on event)
```

### Optimization Tips

```csharp
// ❌ BAD - GetComponent every frame
void Update()
{
    GetComponent<Rigidbody>().AddForce(Vector3.up);
}

// ✓ GOOD - Cache in Awake
Rigidbody rb;
void Awake()
{
    rb = GetComponent<Rigidbody>();
}
void Update()
{
    rb.AddForce(Vector3.up);
}

// ❌ BAD - Find every frame
void Update()
{
    GameObject player = GameObject.Find("Player");
}

// ✓ GOOD - Find once in Start
GameObject player;
void Start()
{
    player = GameObject.Find("Player");
}

// ❌ BAD - Allocating in Update
void Update()
{
    Vector3 temp = new Vector3(1, 2, 3);  // Garbage allocation!
}

// ✓ GOOD - Reuse or use struct
Vector3 temp = Vector3.zero;
void Update()
{
    temp.Set(1, 2, 3);  // No allocation
}
```

---

## Special Cases & Edge Cases

### DontDestroyOnLoad

```csharp
void Awake()
{
    DontDestroyOnLoad(gameObject);
}

// Execution with scene changes:
Scene 1: Awake() → OnEnable() → Start() → Update() loop...
Scene 2 Loads
  → OnDisable() is NOT called
  → OnDestroy() is NOT called
  → Update() continues in new scene
```

### Instantiate During Runtime

```csharp
GameObject obj = Instantiate(prefab);

// Execution order:
// 1. Awake() on new object
// 2. OnEnable() on new object
// 3. (Current frame continues)
// 4. Start() on new object (next frame, before Update)
// 5. Update() on new object (same frame as Start)
```

### SetActive State Changes

```csharp
gameObject.SetActive(false);
// → OnDisable() called immediately

gameObject.SetActive(true);
// → OnEnable() called immediately
// → If first time active: Start() on next frame
// → If previously active: Start() NOT called again
```

---

## Debug Timing Template

Use this script to observe execution order in your project:

```csharp
using UnityEngine;

public class ExecutionOrderDebug : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log($"[{Time.frameCount}] Awake");
    }

    private void OnEnable()
    {
        Debug.Log($"[{Time.frameCount}] OnEnable");
    }

    private void Start()
    {
        Debug.Log($"[{Time.frameCount}] Start");
    }

    private void FixedUpdate()
    {
        Debug.Log($"[{Time.frameCount}] FixedUpdate (Time: {Time.fixedTime})");
    }

    private void Update()
    {
        Debug.Log($"[{Time.frameCount}] Update");
    }

    private void LateUpdate()
    {
        Debug.Log($"[{Time.frameCount}] LateUpdate");
    }

    private void OnAnimatorIK(int layerIndex)
    {
        Debug.Log($"[{Time.frameCount}] OnAnimatorIK (Layer: {layerIndex})");
    }

    private void OnDisable()
    {
        Debug.Log($"[{Time.frameCount}] OnDisable");
    }

    private void OnDestroy()
    {
        Debug.Log($"[{Time.frameCount}] OnDestroy");
    }

    // Physics Events
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[{Time.frameCount}] OnCollisionEnter: {collision.gameObject.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[{Time.frameCount}] OnTriggerEnter: {other.gameObject.name}");
    }

    // Mouse Events
    private void OnMouseEnter()
    {
        Debug.Log($"[{Time.frameCount}] OnMouseEnter");
    }

    private void OnMouseExit()
    {
        Debug.Log($"[{Time.frameCount}] OnMouseExit");
    }

    // Rendering Events
    private void OnBecameVisible()
    {
        Debug.Log($"[{Time.frameCount}] OnBecameVisible");
    }

    private void OnBecameInvisible()
    {
        Debug.Log($"[{Time.frameCount}] OnBecameInvisible");
    }
}
```

---

## Summary Cheat Sheet

```
INITIALIZATION (Once):
  Awake → OnEnable → Start

EVERY FRAME:
  Input Processing
    ↓
  FixedUpdate (0-N times)
    ↓ Physics
    ↓ OnCollision/OnTrigger
    ↓
  Update
    ↓ Coroutines
    ↓
  Animation System
    ↓ Animation Events ⚠️
    ↓
  LateUpdate
    ↓
  OnAnimatorIK
    ↓
  Animation Rigging/IK
    ↓
  Rendering Prep
    ↓ OnWillRenderObject
    ↓ OnBecameVisible/Invisible
    ↓
  Rendering
    ↓ OnRenderObject
    ↓
  GUI
    ↓ OnGUI
    ↓
  End of Frame
    ↓ WaitForEndOfFrame

ON EVENT:
  OnMouseXXX (mouse interaction)
  OnCollisionXXX (physics collision)
  OnTriggerXXX (physics trigger)

DEINITIALIZATION:
  OnDisable → OnDestroy
```

---

**Key Takeaway for Animation Rigging:**

```
Animation Events fire DURING animation processing,
but IK/Rigging runs AFTER animation processing.

Solution: Delay Rig.weight changes by one frame using:
  1. Coroutine with yield return null
  2. LateUpdate() with a flag
```

---

**Last Updated:** October 2025  
**Unity Version:** 2020.3+ (Compatible with most modern versions)  
**Document Version:** 2.0