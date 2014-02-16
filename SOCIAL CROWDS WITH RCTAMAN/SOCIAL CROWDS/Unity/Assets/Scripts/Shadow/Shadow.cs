using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// In the future, this class will probably hold more interesting things
public class Shadow
{
    protected GameObject _rootObject = null;
    protected ShadowController controller = null;

    // A dictionary that caches transforms in the shadow for quick remapping
    protected Dictionary<string, Transform> quickFind;

    public Animation animation { get { return this._rootObject.animation; } }
    public Transform transform { get { return this._rootObject.transform; } }
    public GameObject gameObject { get { return this._rootObject; } }

    // Used for inheritance
    protected Shadow()
    {
        this._rootObject = null;
        this.controller = null;
    }

    /// <summary>
    /// Makes a shadow, which is a hierarchy of GameObjects
    /// that matches the hierarchy given by root. It only
    /// contains the transform hierarchy and an animation 
    /// component, nothing else! 
    /// </summary>
    /// <param name="originalRoot">Root of the hierarchy to be shadowed</param>
    /// <param name="controller">The controller being assigned this shadow</param>
    /// <returns></returns>
    public Shadow(
        Transform originalRoot, 
        ShadowController controller)
    {
        if (originalRoot.animation == null)
            throw new System.ArgumentException(
                "Component to be shadowed must have animation component", 
                "originalRoot");

        this.controller = controller;
        this.quickFind = new Dictionary<string, Transform>();
        this._rootObject = CloneHierarchy(
            originalRoot,
            new HashSet<string>(controller.ignoreTransforms),
            quickFind);

        // Add the ShadowGizmo
        ShadowGizmo sg = this._rootObject.AddComponent<ShadowGizmo>();
        sg.parentController = controller;

        // Add animations in original GameObject to the shadow.
        // TODO: Allow shadow controllers to be more choosy about what 
        //       animations get copied over, and make the animation
        //       component itself optional. - AS
        this._rootObject.AddComponent<Animation>();
        foreach (AnimationState aState in originalRoot.animation)
            this._rootObject.animation.AddClip(aState.clip, aState.name);
        this._rootObject.animation.playAutomatically = false;

        // The shadow should always animate, despite not being rendered
        // TODO: Adjust this as per LOD. - AS
        this._rootObject.animation.cullingType =
            AnimationCullingType.AlwaysAnimate;
        
        // Set the name based on what's being cloned
        string name = controller.GetType().ToString();
        this._rootObject.name += " (Shadow: " + name + ")";
    }

    /// <summary>
    /// Finds the shadowed version of a transform from the original model or
    /// another shadow of the same model.
    /// </summary>
    /// <param name="t">The original transform of which we're trying to find
    /// the shadow.</param>
    /// <returns>The version of trans contained in this shadow</returns>
    public Transform FindInShadow(Transform trans)
    {
        Transform result = null;
        quickFind.TryGetValue(trans.name, out result);
        return result;
    }

    /// <summary>
    /// Gets a bone by name
    /// </summary>
    public Transform GetBone(string name)
    {
        return this.quickFind[name];
    }

    // Performs a traversal of the body hierarchy and creates empty
    // GameObject clones with the same position and orientation.
    protected static GameObject CloneHierarchy(
        Transform root,
        HashSet<string> ignore,
        Dictionary<string, Transform> quickFind)
    {
        GameObject clone = new GameObject(root.name);
        quickFind[root.name] = clone.transform;

        foreach (Transform child in root)
        {
            if (ignore.Contains(child.name) == false)
            {
                GameObject newChild = 
                    CloneHierarchy(child, ignore, quickFind);
                newChild.transform.parent = clone.transform;
            }
        }

        // We set the clone transform properties last, so Unity propagates the
        // changes to its children after they're assigned
        clone.transform.localPosition = root.localPosition;
        clone.transform.localRotation = root.localRotation;
        clone.transform.localScale = root.localScale;

        return clone;
    }

    #region Encoding and Decoding
    /// <summary>
    /// Creates an encoded skeleton array from this shadow hierarchy
    /// </summary>
    /// <returns>A potentially sparse array describing this shadow</returns>
    public ShadowTransform[] Encode(ShadowTransform[] buffer)
    {
        ShadowCoordinator coordinator = this.controller.Coordinator;
        Shadow.WriteShadowData(
            buffer, 
            this._rootObject.transform.GetChild(0),
            coordinator);
        return buffer;
    }

    /// <summary>
    /// <summary>
    /// Creates an encoded skeleton array from this shadow hierarchy
    /// </summary>
    /// <param name="listType">Whitelist or blacklist</param>
    /// <param name="list">The list of transforms to prune or include</param>
    /// <returns>A potentially sparse array describing this shadow</returns>
    public ShadowTransform[] Encode(
        ShadowTransform[] buffer,
        FilterList<string> nameFilter)
    {
        ShadowCoordinator coordinator = this.controller.Coordinator;
        Shadow.WriteShadowData(
            buffer,
            this._rootObject.transform.GetChild(0),
            coordinator,
            nameFilter);
        return buffer;
    }

    /// <summary>
    /// Applies an encoded skeleton (or part of one) to this shadow
    /// </summary>
    /// <param name="data">The encoded shadow skeleton</param>
    public void Decode(ShadowTransform[] data)
    {
        Shadow.ReadShadowData(
            data, 
            this._rootObject.transform.GetChild(0),
            this.controller.Coordinator);
    }


    /// <summary>
    /// Applies an encoded skeleton (or part of one) to this shadow
    /// </summary>
    /// <param name="data">The encoded shadow skeleton</param>
    /// <param name="listType">Whitelist or blacklist</param>
    /// <param name="list">The list of transforms to prune or include</param>
    public void Decode(
        ShadowTransform[] data,
        FilterList<string> nameFilter)
    {
        Shadow.ReadShadowData(
            data,
            this._rootObject.transform.GetChild(0),
            this.controller.Coordinator,
            nameFilter);
    }

    /// <summary>
    /// Recursively populates a transform data array from this shadow
    /// </summary>
    /// <param name="data">The array to populate</param>
    /// <param name="t">The current subtree root</param>
    /// <param name="coordinator">The coordinator</param>
    public static void WriteShadowData(
        ShadowTransform[] buffer,
        Transform t,
        ShadowCoordinator coordinator)
    {
        int key = coordinator.GetBoneKey(t.name);
        buffer[key].ReadFrom(t);
        foreach (Transform child in t)
            WriteShadowData(buffer, child, coordinator);
    }

    /// <summary>
    /// Recursively populates a transform data array from this shadow, with
    /// a potential whitelist or blacklist
    /// </summary>
    /// <param name="data">The array to populate</param>
    /// <param name="t">The current subtree root</param>
    /// <param name="coordinator">The coordinator</param>
    /// <param name="nameFilter">The filter for names</param>
    /// <param name="bypass">Used for whitelists to take children of 
    /// whitelisted transforms</param>
    public static void WriteShadowData(
        ShadowTransform[] data,
        Transform t,
        ShadowCoordinator coordinator,
        FilterList<string> nameFilter,
        bool bypass = false)
    {
        bool allowed = (bypass == true || nameFilter.Allowed(t.name) == true);
        bool whitelist = (nameFilter.IsWhitelist() == true);
        bypass = (allowed == true && whitelist == true);

        // If we're permitting this bone through the filter
        if (allowed == true)
            data[coordinator.GetBoneKey(t.name)].ReadFrom(t);

        // See if we need to keep searching
        if (whitelist == true || allowed == true)
            foreach (Transform child in t)
                WriteShadowData(data, child, coordinator, nameFilter, bypass);
    }

    /// <summary>
    /// Recursively applies an encoded skeleton, or part of it, to the shadow
    /// </summary>
    /// <param name="data">The encoded skeleton data</param>
    /// <param name="t">The current transform to apply to</param>
    public static void ReadShadowData(
        ShadowTransform[] data, 
        Transform t,
        ShadowCoordinator coordinator)
    {
        int key = coordinator.GetBoneKey(t.name);
        if (ShadowTransform.IsValid(data[key]) == true)
            data[key].WriteTo(t);
        foreach (Transform child in t)
            ReadShadowData(data, child, coordinator);
    }

    /// <summary>
    /// Recursively applies an encoded skeleton, or part of it, to the shadow
    /// </summary>
    /// <param name="data">The encoded skeleton data</param>
    /// <param name="t">The current transform to apply to</param>
    /// <param name="nameFilter">The filter for names</param>
    /// <param name="bypass">Used for whitelists to take children of 
    /// whitelisted transforms</param>
    public static void ReadShadowData(
        ShadowTransform[] data,
        Transform t,
        ShadowCoordinator coordinator,
        FilterList<string> nameFilter,
        bool bypass = false)
    {
        bool allowed = (bypass == true || nameFilter.Allowed(t.name) == true);
        bool whitelist = (nameFilter.IsWhitelist() == true);
        bypass = (allowed == true && whitelist == true);

        // If this is a valid bone, and is allowed, read it to the skeleton
        int key = coordinator.GetBoneKey(t.name);
        if (allowed == true && ShadowTransform.IsValid(data[key]) == true)
            data[key].WriteTo(t);

        // See if we need to keep searching
        if (whitelist == true || allowed == true)
            foreach (Transform child in t)
                ReadShadowData(data, child, coordinator, nameFilter, bypass);
    }
    #endregion Encoding and Decoding

}
