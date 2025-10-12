Great debugging work! You've discovered something important about Unity's **Animation Rigging system timing**. Let me explain why the gradual approach works but direct assignment doesn't:

## Why Direct Assignment Fails

```csharp
public void TrueIncrRigWeight()
{
    this.shouldIncrRigWeight = true;
    this._rig.weight = 0.95f;  // ❌ Gets overwritten
}
```

When you set `_rig.weight = 0.95f` directly in the animation event:

1. **Animation Event fires** → `TrueIncrRigWeight()` called → `_rig.weight = 0.95f`
2. **Animation Rigging system evaluates** (happens after animation events in the same frame)
3. **RigBuilder recalculates/rebuilds** the rig state
4. **Rig weight gets reset back to 0** because the Animation Rigging system has its own internal state that hasn't been updated yet

The Animation Rigging system essentially says: *"I'm still processing the reload animation state, so the rig should be at weight 0"* and overwrites your direct assignment.

## Why Gradual Increment Works

```csharp
private void Update()
{
    if(this.shouldIncrRigWeight == true)
    {
        this._rig.weight += 3f * Time.deltaTime;  // ✅ Gradual increase
        if (this._rig.weight > 0.95f)
            this.shouldIncrRigWeight = false;
    }
}
```

With the gradual approach:

1. **Animation Event fires** → Sets flag `shouldIncrRigWeight = true`
2. **Next frame's Update()** → Increases weight by a small amount (e.g., 0.05)
3. **Animation Rigging system evaluates** → Sees the small change and doesn't fight it
4. **Next frame** → Another small increment
5. This continues until weight reaches 0.95f

The key is that you're making **small, persistent changes across multiple frames** rather than one large change. The Animation Rigging system tolerates these gradual adjustments because:
- The animation state has transitioned away from "reload"
- The small delta changes don't trigger a full rig rebuild
- You're essentially "easing" the rig weight back up in a way that cooperates with Unity's update cycle

## The Root Cause

This behavior suggests that Unity's **Animation Rigging evaluation happens AFTER animation events but potentially BEFORE or conflicts with direct weight changes in the same frame**. The execution order is roughly:

```
Frame N:
1. Update()
2. Animation System updates
3. Animation Events fire ← Your TrueIncrRigWeight() called here
4. Animation Rigging evaluates ← Overwrites your weight change
5. LateUpdate()
6. Rendering

Frame N+1:
1. Update() ← Gradual increment survives here
2. Animation Rigging evaluates ← Accepts the change
...
```

## Better Solution

While your solution works, here's a cleaner version using `LateUpdate()` with a coroutine:

```csharp
public void TrueIncrRigWeight()
{
    StartCoroutine(RestoreRigWeightCoroutine());
}

IEnumerator RestoreRigWeightCoroutine()
{
    yield return null; // Wait one frame for Animation Rigging to finish
    
    float duration = 0.3f; // Smooth transition over 0.3 seconds
    float elapsed = 0f;
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        _rig.weight = Mathf.Lerp(0f, 1f, elapsed / duration);
        yield return null;
    }
    
    _rig.weight = 1f;
}
```

This gives you a smooth transition and doesn't require the `Update()` check every frame.

**TL;DR**: Direct assignment gets overwritten by Animation Rigging's evaluation in the same frame. Gradual increments across multiple frames work because they happen AFTER the Animation Rigging system has finished transitioning away from the reload state.