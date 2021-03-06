using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Size")]
    [SerializeField] private int size = 4;

    [Header("Prefab")]
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private Block blockPrefab;

    [Header("Value")]
    [SerializeField] private float moveTime = 0.2f;

    [Header("Screen UI")]
    [SerializeField] private GameObject gameOverScreen;

    [Header("Component")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private BeatManager beatManager;

    [Header("Block Type")]
    [SerializeField] private List<BlockType> types;

    private List<Node> nodes;
    private List<Block> blocks;

    private GameState state;
    public GameState State => state;
    private int round = 0;
    private int highestValue = 0;

    private Vector2 beginPos;

    private BlockType GetBlockTypeByValue(int value) => types.First(t => t.value == value);

    private void Start()
    {
        ChangeState(GameState.Generate);
    }

    public void ChangeState(GameState newState)
    {
        state = newState;

        switch (state)
        {
            case GameState.Generate:
                Generate();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.GameOver:
                gameOverScreen.SetActive(true);
                scoreManager.Show(highestValue);
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (state != GameState.WaitingInput) return;
        if (Time.timeScale == 0) return;

#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveTo(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveTo(Vector2.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveTo(Vector2.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveTo(Vector2.down);

        if (Input.GetMouseButtonDown(0)) beginPos = Input.mousePosition;
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 dir = ((Vector2)Input.mousePosition - beginPos).normalized;

            if (dir.x < 0 && dir.y < 0.5f && dir.y > -0.5f) MoveTo(Vector2.left);
            if (dir.x > 0 && dir.y < 0.5f && dir.y > -0.5f) MoveTo(Vector2.right);
            if (dir.y > 0 && dir.x < 0.5f && dir.x > -0.5f) MoveTo(Vector2.up);
            if (dir.y < 0 && dir.x < 0.5f && dir.x > -0.5f) MoveTo(Vector2.down);
        }
#elif UNITY_ANDROID
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            beginPos = touch.position;
        }
        if (touch.phase == TouchPhase.Ended)
        {
            Vector2 dir = (touch.position - beginPos).normalized;

            if (dir.x < 0 && dir.y < 0.5f && dir.y > -0.5f) MoveTo(Vector2.left);
            if (dir.x > 0 && dir.y < 0.5f && dir.y > -0.5f) MoveTo(Vector2.right);
            if (dir.y > 0 && dir.x < 0.5f && dir.x > -0.5f) MoveTo(Vector2.up);
            if (dir.y < 0 && dir.x < 0.5f && dir.x > -0.5f) MoveTo(Vector2.down);
        }
#endif
    }

    private void Generate()
    {
        nodes = new List<Node>();
        blocks = new List<Block>();

        Vector2 center = new Vector2((float)size / 2 - 0.5f, (float)size / 2 - 0.5f);

        Instantiate(boardPrefab, Vector2.zero, Quaternion.identity).size = new Vector2(size, size);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Node node = Instantiate(nodePrefab, new Vector2(x, y) - center, Quaternion.identity);
                nodes.Add(node);
            }
        }

        scoreManager.Init();

        ChangeState(GameState.SpawningBlocks);
    }

    public void SpawnBlocks(int amount)
    {
        List<Node> freeNodes = nodes.Where(c => c.OccupiedBlock == null).OrderBy(b => Random.value).ToList();

        foreach (Node node in freeNodes.Take(amount))
        {
            int rand = Random.Range(0, 101);
            if (highestValue <= 16) SpawnBlock(node, rand > 100 ? 4 : 2);
            else if (highestValue == 32) SpawnBlock(node, rand > 90 ? 4 : 2);
            else if (highestValue == 64) SpawnBlock(node, rand > 80 ? 4 : 2);
            else if (highestValue == 128) SpawnBlock(node, rand > 70 ? 4 : 2);
            else if (highestValue == 256) SpawnBlock(node, rand > 60 ? 4 : 2);
            else if (highestValue == 512) SpawnBlock(node, rand > 55 ? 4 : 2);
            else if (highestValue == 1024) SpawnBlock(node, rand > 50 ? 4 : 2);
            else if (highestValue >= 2048) SpawnBlock(node, rand > 45 ? 4 : 2);
        }

        if (freeNodes.Count == 1)
        {
            if (CanMovingBlock() == false)
            {
                ChangeState(GameState.GameOver);
                return;
            }
        }

        ChangeState(GameState.WaitingInput);
    }

    private void SpawnBlock(Node node, int value)
    {
        Block block = Instantiate(blockPrefab, node.Pos, Quaternion.identity);
        BlockType blockType = GetBlockTypeByValue(Mathf.Min(4096, value));
        blockType.value = value;
        block.Init(blockType);
        block.SetBlock(node);
        blocks.Add(block);
    }

    private void MoveTo(Vector2 dir)
    {
        ChangeState(GameState.Moving);

        List<Block> orderedBlocks = blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

        bool isMove = false;
        foreach (Block block in orderedBlocks)
        {
            Node next = block.Node;
            do
            {
                block.SetBlock(next);

                Node possibleNode = GetNodeAtPosition(next.Pos + dir);

                if (possibleNode != null)
                {
                    if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.Value))
                    {
                        block.MergeBlock(possibleNode.OccupiedBlock);
                        isMove = true;
                    }
                    else if (possibleNode.OccupiedBlock == null)
                    {
                        next = possibleNode;
                        isMove = true;
                    }
                }

            } while (next != block.Node);
        }

        if (isMove == false || beatManager.InactivateBeat() == false)
        {
            beatManager.TakeDamage();
            scoreManager.ResetCombo();
        }
        else
        {
            scoreManager.IncreaseCombo();
        }

        Sequence sequence = DOTween.Sequence();

        foreach (Block block in orderedBlocks)
        {
            Vector2 movePoint = block.MergingBlock != null ? block.MergingBlock.Node.Pos : block.Node.Pos;
            sequence.Insert(0, block.transform.DOMove(movePoint, moveTime));
        }

        sequence.OnComplete(() =>
        {
            int score = scoreManager.Score;
            foreach (Block block in orderedBlocks.Where(b => b.MergingBlock != null))
            {
                if (block.Value * 2 > highestValue)
                {
                    highestValue = block.Value * 2;
                }
                scoreManager.IncreaseScore();
                MergeBlocks(block.MergingBlock, block);
            }
            if (score != scoreManager.Score)
            {
                scoreManager.DisplayScore();
            }

            if (state != GameState.GameOver)
            {
                ChangeState(isMove ? GameState.SpawningBlocks : GameState.WaitingInput);
            }
        });
    }

    private void MergeBlocks(Block baseBlock, Block mergingBlock)
    {
        SpawnBlock(baseBlock.Node, baseBlock.Value * 2);

        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    private void RemoveBlock(Block block)
    {
        blocks.Remove(block);
        Destroy(block.gameObject);
    }

    private bool CanMovingBlock()
    {
        int count = 0;
        do
        {
            Vector2 dir = Vector2.left;

            switch (count)
            {
                case 0: dir = Vector2.left; break;
                case 1: dir = Vector2.right; break;
                case 2: dir = Vector2.up; break;
                case 3: dir = Vector2.down; break;
            }

            List<Block> orderedBlocks = blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
            if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

            foreach (Block block in orderedBlocks)
            {
                Node next = block.Node;
                Node current;
                do
                {
                    current = next;

                    Node possibleNode = GetNodeAtPosition(next.Pos + dir);

                    if (possibleNode != null)
                    {
                        if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.Value)) return true;
                        else if (possibleNode.OccupiedBlock == null) return true;
                    }

                } while (next != current);
            }

            count++;

        } while (count < 4);

        return false;
    }

    private Node GetNodeAtPosition(Vector2 pos)
    {
        return nodes.FirstOrDefault(n => n.Pos == pos);
    }

    public void Restart()
    {
        SceneManager.LoadScene("Title");
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }
}

[System.Serializable]
public struct BlockType
{
    public int value;
    public Sprite sprite;
}

public enum GameState
{
    Generate,
    SpawningBlocks,
    WaitingInput,
    Moving,
    GameOver
}