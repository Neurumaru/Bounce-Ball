using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

public class BallAgent : Agent
{
    //==================================================
    //�ʿ��� Ŭ���� ����

    //�����Ǵ� ��ġ�� ǥ���ϴ� Vector2 ���� �ΰ�
    [System.Serializable]
    public class SpawnPoint
    {
        public Vector2 start, end;

        public SpawnPoint(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }
    }

    //==================================================
    //���� ���� ����

    //������ ���� �ʿ��� ����
    private Transform tr;
    private Rigidbody2D rb;
    private Vector2 firstPos;
    private Unity.MLAgents.Sensors.BufferSensorComponent bufferSensor;
    private GameObject[,] point;

    //������� ���� �ʿ��� ����
    private int step;
    private static int debugStep = 0;
    private static int debugRecent = 0;
    private static int debugStepInterval = 100;

    //���� ��
    private float moveSpeed;

    //==================================================
    //�ν����Ϳ��� �������� ����

    public bool trainning;
    public Vector2[] targetMove;
    private int targetMoveIndex;

    //�����Ǵ� ��ġ
    [Header("Spawn Position")]
    public SpawnPoint[] ballPositions;
    public SpawnPoint[] targetPositions;
    public bool useRandomTile;
    public UnityEngine.Tilemaps.TileBase tile;
    private List<Vector2> points;
    public int changeTileInterval;


    //�����Ǵ� Target ������Ʈ
    [Header("Observation")]
    public Transform targetTr;

    //PointSensor�� ����ϴµ� �ʿ��� ����
    [Header("PointSensor2D")]
    public Vector2 cellScale;
    public Vector2 gridSize;
    public int stacked;
    public UnityEngine.Tilemaps.Tilemap tileMap;

    //PointSensor�� ������ϴµ� ����ϴ� ����
    [Header("PointSensor2D Debug")]
    public bool debug;
    public Sprite sprite;
    public float scale;
    public Color hitColor;
    public Color missColor;

    //==================================================
    //ML-Agents

    //�ʱ�ȭ �۾��� ���� �ѹ� ȣ��Ǵ� �޼ҵ�
    public override void Initialize()
    {
        //�������� �⺻ �� ����
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = Ball.defaultMoveSpeed;
        bufferSensor = GetComponent<Unity.MLAgents.Sensors.BufferSensorComponent>();
        bufferSensor.MaxNumObservables = (int)(gridSize.x * gridSize.y * stacked);
        points = new List<Vector2>();
        step = 0;
        firstPos = rb.position;

        //����뿡 ����� ������Ʈ ����
        if (debug)
        {
            point = new GameObject[(int)gridSize.x, (int)gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    //SpriteRenderer�� ����� �ð�ȭ
                    point[x, y] = new GameObject("");
                    Transform pointTr = point[x, y].GetComponent<Transform>();
                    SpriteRenderer pointSr = point[x, y].AddComponent<SpriteRenderer>();
                    pointSr.sprite = sprite;
                    pointSr.color = missColor;
                    pointTr.localScale = new Vector3(scale, scale, 1);
                    pointTr.parent = tr.parent.transform;
                }
            }
        }
    }

    //���Ǽҵ�(�н�����)�� �����Ҷ����� ȣ��
    public override void OnEpisodeBegin()
    {
        if (trainning)
        {
            //Ball�� velocity �ʱ�ȭ
            Ball.Stop(rb);

            //Ball�� ��ġ �����ϰ� ����
            int random = (int)Random.Range(0, ballPositions.GetLength(0));
            tr.localPosition = new Vector2(Random.Range(ballPositions[random].start.x, ballPositions[random].end.x),
                                           Random.Range(ballPositions[random].start.y, ballPositions[random].end.y));

            //Target�� ��ġ �����ϰ� ����
            random = (int)Random.Range(0, targetPositions.GetLength(0));
            targetTr.localPosition = new Vector2(Random.Range(targetPositions[random].start.x, targetPositions[random].end.x),
                                                Random.Range(targetPositions[random].start.y, targetPositions[random].end.y));

            if (useRandomTile && step++ % changeTileInterval == 0)
            {
                tileMap.ClearAllTiles();
                points.Clear();

                int randomX = (int)Random.Range(-10, 9);
                int randomY = (int)Random.Range(-6, 3);
                while (randomX >= -10 && randomX <= 9 && randomY >= -6 && randomY <= 3)
                {
                    points.Add(new Vector2(randomX + 0.5f, randomY + 1.5f));
                    tileMap.SetTile(new Vector3Int(randomX, randomY, 0), tile);

                    int postRandomX = randomX;
                    int postRandomY = randomY;

                    randomX += (int)Random.Range(-4, 4);
                    randomY = (int)Random.Range(randomY + 1, -6);

                    if (tileMap.GetTile(new Vector3Int(randomX, randomY - 1, 0)) != null ||
                        tileMap.GetTile(new Vector3Int(randomX, randomY + 1, 0)) != null)
                    {
                        randomX = postRandomX;
                        randomY = postRandomY;
                    }
                }
            }
            if (useRandomTile)
            {
                tr.localPosition = points[0];

                //Target�� ��ġ �����ϰ� ����
                random = (int)Random.Range(0, points.Count);
                targetTr.localPosition = points[random];
            }


            //�����
            if (debugStepInterval != 0 && (++debugStep % debugStepInterval == 0))
            {
                Debug.Log($"[Debug] Step : {debugStep}. Mean Result : {(float)debugRecent / (float)debugStepInterval}.");
                debugRecent = 0;
            }
        } 
        else
        {
            targetMoveIndex = 0;
            if (targetMoveIndex < targetMove.Length)
                targetTr.localPosition = targetMove[targetMoveIndex++];
        }
    }

    //ȯ�� ������ ���� �� ������ ��å ������ ���� �극�ο� �����ϴ� �޼ҵ�
    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        //sensor�� �������� ���� (6��)
        sensor.AddObservation(targetTr.localPosition.x);
        sensor.AddObservation(targetTr.localPosition.y);
        sensor.AddObservation(tr.localPosition.x);
        sensor.AddObservation(tr.localPosition.y);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);

        //BufferSensor�� Point���� �������� ���� (PointSensor2D)
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            { 
                //������ ��ġ�� �ִ� Tile ������ �����ͼ� ����
                float positionX = tr.localPosition.x + (x - (gridSize.x - 1.0f) / 2.0f) * cellScale.x;
                float positionY = tr.localPosition.y + (y - (gridSize.y - 1.0f) / 2.0f) * cellScale.y;
                Vector3Int position = new Vector3Int((int)Mathf.FloorToInt(positionX), (int)Mathf.FloorToInt(positionY), 0);
                float[] obs = { tileMap.GetTile(position) == null ? 0.0f : 1.0f };
                bufferSensor.AppendObservation(obs);

                //����� ���� Point���� ��ġ�� �ð������� ǥ��
                if (debug)
                {
                    Transform pointTr = point[x, y].GetComponent<Transform>();
                    SpriteRenderer pointSr = point[x, y].GetComponent<SpriteRenderer>();
                    pointTr.localPosition = new Vector3(positionX, positionY, 2);
                    pointSr.color = obs[0] == 1.0f ? hitColor : missColor;
                }
            }
        }
    }
    
    //�극��(��å)���� ���� ���� ���� �ൿ�� �����ϴ� �޼ҵ�
    public override void OnActionReceived(ActionBuffers actions)
    {
        //�������� ������ �̻�ȭ ��Ŵ
        float h = Mathf.Clamp(actions.ContinuousActions[0], -1.0f, 1.0f);
        if (h > 0.5) { Ball.Move(rb, moveSpeed); }
        else if (h < -0.5) { Ball.Move(rb, -moveSpeed); }
        else { Ball.Move(rb, 0); }

        //Max Step�� �ɰ�� Dead_Zone �浹�� ���� Reward�� �޵��� ���� 
        AddReward(-(1.0f / MaxStep));
    }
    
    //������(�����)�� ���� ����� ������ ȣ���ϴ� �޼ҵ�(�ַ� �׽�Ʈ�뵵 �Ǵ� ����н��� ���)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
    }

    //==================================================
    //����Ƽ �Լ�

    //Trigger�� ������Ʈ�� �浹��
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (trainning)
        {
            //Target�� �浹��
            if (other.CompareTag("Target"))
            {
                AddReward(+1.0f);
                debugRecent++;
                EndEpisode();
            }
            //Dead_Zone�� �浹��
            if (other.CompareTag("Dead_Zone"))
            {
                SetReward(-1.0f);
                EndEpisode();
            }
        }
        else
        {
            if (other.CompareTag("Target"))
            {
                if (targetMoveIndex < targetMove.Length)
                    targetTr.localPosition = targetMove[targetMoveIndex++];
            }
            if (other.CompareTag("Dead_Zone"))
            {
                Ball.Die(rb, firstPos);
                EndEpisode();
            }
        }
    }
}