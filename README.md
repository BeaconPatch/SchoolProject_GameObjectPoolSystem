# GameObjectPool system
_A system made in Unity for my video game developer program's final project._

## In short
A implementation of an object pool system for Unity's GameObject. I'll recognize this implementation as naive, but I wanted to experiment and I've learn a few interesting things along the way.

This implementation of a GameOjbectPool rest on the idea that we could use a GameObject activation's state as flags to determine if it was still in the pool or actively being used. This means that the object, once create during the GameObjectPool's initialization, never moves around in the scene. This lack of ability to reparent objects has occasionally caused some headache, but I believe that this decision was a good once despite its implementation to be rectified.

It starts with a GameObjectPoolManager, a singleton-like MonoBehaviour that we would be saved as a prefab to put in the scene once. It contains a list of GameObjects (ideally prefabs) with a given quantity. For each of those entry in the list, it would create a child GameObjectPool dedicated to a single entry in the list, which in turn will create the instances for its assigned model in the given quantity.

From there, the GameObjectPoolManager act as an entry gate to any pool request. While we could also get a GameObjectPool directly, passing by the GameObject's reference, the manager allowed to draw from any pools with ease and without potentially saving references to multiple pool. For example, an EnemyManager might draw enemies from multiple GameObjectPools. Passing by the manager allows said EnemyManager to not save multiple references for specific pools.

## Few changes for this repository
For the purpose of this repository, I've...  
... replaced the project namespace with my own when I made the file.
... removed a few work in progress notes and left overs TODOs.  
The rest remains as is, albeit outside of its full context.

## What would I change in the future
For a start, the how it was managed. GameObjectPools where assuming that the objects their where instantiating wouldn't be moving the scene's hierarchy. As such, the implementation made it that they shouldn't, but they aren't stopped from it, which would break everything. It also relied on the fact that GameOjbect would be considered available when deactivated and in use and activated. This way of keeping track of their availability made it simple to track, but harder to handle when you wanted to "extract" a GameObject from a pool without activating it right away (such as waiting to find where to spawn it).

The overall experimentation I made here was interesting and instructive to me. However, I would do it again very differently. The biggest problem to address in my opinion is how the activation state (in/out of the pool) is being tracked.
