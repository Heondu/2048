using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public Vector2 Pos => transform.position;

    public int Value;
    public Node Node;

    public Block MergingBlock;
    public bool Merging;

    public void Init(BlockType blockType)
    {
        Value = blockType.value;
        text.text = blockType.value.ToString();
        spriteRenderer.sprite = blockType.sprite;
    }

    public void SetBlock(Node node)
    {
        if (Node != null) Node.OccupiedBlock = null;
        Node = node;
        Node.OccupiedBlock = this;
    }

    public void MergeBlock(Block blockToMergeWith)
    {
        MergingBlock = blockToMergeWith;

        Node.OccupiedBlock = null;

        blockToMergeWith.Merging = true;
    }

    public bool CanMerge(int value) => value == Value && !Merging && MergingBlock == null;
}