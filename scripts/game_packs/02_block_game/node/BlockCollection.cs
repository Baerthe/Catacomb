namespace BlockGame;

using Common;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages a collection of blocks in the BlockGame.
/// </summary>
[GlobalClass]
public sealed partial class BlockCollection : Node2D
{
    public event Action OnAllBlocksDestroyed;
    [Export] public PackedScene BlockScene { get; private set; }
    public Vector2 BlockSize { get; private set; } = new Vector2(24, 18);
    public List<(Block , byte, byte)> BlockList { get; private set; }
    public Vector2 Spacing { get; private set; } = new Vector2(2, 2);
    private LevelData _currentLevel;
    private bool _debugMode = false;
    public override void _Process(double delta)
    {
        if (_debugMode)
            DebugDraw();
    }
    /// <summary>
    /// Cleans up all blocks from the current level.
    /// </summary>
    public void ClearLevel()
    {
        foreach (var block in GetTree().GetNodesInGroup("block"))
        {
            if (block is Block b)
                b.QueueFree();
        }
        BlockList = new List<(Block, byte, byte)>();
        _currentLevel = null;
    }
    /// <summary>
    /// Generates a level based on the provided LevelData. It adds them within the BlockCollection node space.
    /// Leave level as null to generate a random level.
    /// </summary>
    /// <param name="level"></param>
    public void GenerateLevel(LevelData level = null)
    {
        _currentLevel = level;
        var lines = null as string[];
        var width = 24;
        byte[] weightedPoints = { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 3, 3, 4 };
        if (level == null)
        {
            byte randLines = (byte)GD.RandRange(5, 15);
            lines = new string[randLines];
            for (byte i = 0; i < randLines; i++)
            {
                string line = "";
                for (byte j = 0; j < width; j++)
                {
                    byte points = weightedPoints[(byte)GD.RandRange(0, weightedPoints.Length - 1)];
                    line += points.ToString();
                }
                lines[i] = line;
            }
        }
        else
            lines = level.LevelGrid.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        var height = lines.Length;
        BlockList = new List<(Block, byte, byte)>();
        for (int y = 0; y < height; y++)
        {
            var line = lines[y];
            for (int x = 0; x < width; x++)
            {
                if (x < line.Length)
                {
                    var charValue = line[x];
                    if (charValue >= '1' && charValue <= '9')
                    {
                        var hitPoints = (byte)(charValue - '0');
                        var blockInstance = this.InstanceScene(BlockScene) as Block;
                        blockInstance.Position = new Vector2(x * (BlockSize.X + Spacing.X), y * (BlockSize.Y + Spacing.Y));
                        blockInstance.XPOS = (byte)x;
                        blockInstance.YPOS = (byte)y;
                        blockInstance.BlockDestroyed += OnBlockDestroyed;
                        blockInstance.SetHitPoints(hitPoints);
                        BlockList.Add((blockInstance, (byte)x, (byte)y));
                    }
                }
            }
        }
    }
    /// <summary>
    /// Resets the current level by clearing and regenerating it.
    /// </summary>
    public void ResetLevel()
    {
        if (_currentLevel != null)
        {
            ClearLevel();
            GenerateLevel(_currentLevel);
        }
    }
    /// <summary>
    /// Debug function to randomly hit blocks and regenerate level if all blocks are destroyed. Used to make sure both this and blocks logic worked.
    /// </summary>
    private void DebugDraw()
    {
        var rand = GD.RandRange(0, 1);
        if (rand > 0.8f && BlockList != null && BlockList.Count > 0)
        {
            byte item = (byte)GD.RandRange(0, BlockList.Count - 1);
            Block block = BlockList[item].Item1 ?? null;
            if (block != null)
                block?.Call("OnBlockHit");
        }
        if (GetTree().GetNodesInGroup("block").Count == 0)
            GenerateLevel();
    }
    /// <summary>
    /// Handles the block destroyed event to remove it from the BlockList.
    /// </summary>
    private void OnBlockDestroyed(Block block)
    {
        block.BlockDestroyed -= OnBlockDestroyed;
        BlockList.RemoveAll(item => item.Item1 == block);
        if (BlockList.Count == 0)
            OnAllBlocksDestroyed?.Invoke();
    }
}