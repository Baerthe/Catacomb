namespace Common;

using System;
using Godot;
/// <summary>
/// Utility extension methods for Godot Nodes.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Adds a new node of type T as a child to the specified parent node.
    /// This allows for easy instantiation and addition of nodes in the scene tree.
    /// </summary>
    /// <typeparam name="T">The type of node to add. Must be a subclass of Node and have a parameterless constructor.</typeparam>
    /// <param name="parent">The parent node to which the new node will be added as a child.</param>
    /// <param name="name">The name to assign to the new node.</param>
    /// <returns>The newly created node of type T.</returns>
    public static T AddNode<T>(this Node parent, string name = "") where T : Node, new()
    {
        var child = new T() ?? throw new InvalidOperationException($"Failed to create instance of type {typeof(T).Name}");
        GD.Print($"Adding node of type {typeof(T).Name} to parent {parent.Name}");
        if (!string.IsNullOrEmpty(name))
            child.Name = name;
        parent.AddChild(child);
        return child;
    }
    /// <summary>
    /// Instantiates a PackedScene and adds it as a child to the specified parent node.
    /// </summary>
    /// <param name="parent">The parent node to which the instantiated scene will be added as a child.</param>
    /// <param name="scene">The PackedScene to instantiate.</param>
    /// <returns>The root node of the instantiated scene.</returns>
    public static Node InstanceScene(this Node parent, PackedScene scene)
    {
        var instance = scene.Instantiate() ?? throw new InvalidOperationException($"Failed to instantiate scene of type {scene.ResourceName}");
        GD.Print($"Instancing scene of type {instance.GetType().Name} to parent {parent.Name}");
        parent.AddChild(instance);
        return instance;
    }
    /// <summary>
    /// Turncates a string to a specified maximum length determined by float.
    /// </summary>
    public static string Truncate(this string value, float maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        int trim = (int)(value.Length * maxLength);
        return value.Length <= maxLength ? value : value[..trim];
    }
}