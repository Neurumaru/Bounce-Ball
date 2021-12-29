using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

public class BallAgent : Agent
{
    //==================================================
    //필요한 클래스 선언

    //생성되는 위치를 표현하는 Vector2 변수 두개
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
    //내부 변수 선언

    //관측을 위해 필요한 변수
    private Transform tr;
    private Rigidbody2D rb;
    private Vector2 firstPos;
    private Unity.MLAgents.Sensors.BufferSensorComponent bufferSensor;
    private GameObject[,] point;

    //디버깅을 위해 필요한 변수
    private int step;
    private static int debugStep = 0;
    private static int debugRecent = 0;
    private static int debugStepInterval = 100;

    //설정 값
    private float moveSpeed;

    //==================================================
    //인스펙터에서 가져오는 값들

    public bool trainning;
    public Vector2[] targetMove;
    private int targetMoveIndex;

    //생성되는 위치
    [Header("Spawn Position")]
    public SpawnPoint[] ballPositions;
    public SpawnPoint[] targetPositions;
    public bool useRandomTile;
    public UnityEngine.Tilemaps.TileBase tile;
    private List<Vector2> points;
    public int changeTileInterval;


    //관측되는 Target 오브젝트
    [Header("Observation")]
    public Transform targetTr;

    //PointSensor를 사용하는데 필요한 변수
    [Header("PointSensor2D")]
    public Vector2 cellScale;
    public Vector2 gridSize;
    public int stacked;
    public UnityEngine.Tilemaps.Tilemap tileMap;

    //PointSensor를 디버깅하는데 사용하는 변수
    [Header("PointSensor2D Debug")]
    public bool debug;
    public Sprite sprite;
    public float scale;
    public Color hitColor;
    public Color missColor;

    //==================================================
    //ML-Agents

    //초기화 작업을 위해 한번 호출되는 메소드
    public override void Initialize()
    {
        //변수들의 기본 값 설정
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = Ball.defaultMoveSpeed;
        bufferSensor = GetComponent<Unity.MLAgents.Sensors.BufferSensorComponent>();
        bufferSensor.MaxNumObservables = (int)(gridSize.x * gridSize.y * stacked);
        points = new List<Vector2>();
        step = 0;
        firstPos = rb.position;

        //디버깅에 사용할 오브젝트 생성
        if (debug)
        {
            point = new GameObject[(int)gridSize.x, (int)gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    //SpriteRenderer를 사용해 시각화
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

    //에피소드(학습단위)가 시작할때마다 호출
    public override void OnEpisodeBegin()
    {
        if (trainning)
        {
            //Ball의 velocity 초기화
            Ball.Stop(rb);

            //Ball의 위치 랜덤하게 생성
            int random = (int)Random.Range(0, ballPositions.GetLength(0));
            tr.localPosition = new Vector2(Random.Range(ballPositions[random].start.x, ballPositions[random].end.x),
                                           Random.Range(ballPositions[random].start.y, ballPositions[random].end.y));

            //Target의 위치 랜덤하게 생성
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

                //Target의 위치 랜덤하게 생성
                random = (int)Random.Range(0, points.Count);
                targetTr.localPosition = points[random];
            }


            //디버깅
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

    //환경 정보를 관측 및 수집해 정책 결정을 위해 브레인에 전달하는 메소드
    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        //sensor에 관측정보 전달 (6개)
        sensor.AddObservation(targetTr.localPosition.x);
        sensor.AddObservation(targetTr.localPosition.y);
        sensor.AddObservation(tr.localPosition.x);
        sensor.AddObservation(tr.localPosition.y);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);

        //BufferSensor에 Point들의 관측정보 전달 (PointSensor2D)
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            { 
                //각각의 위치에 있는 Tile 정보를 가져와서 관측
                float positionX = tr.localPosition.x + (x - (gridSize.x - 1.0f) / 2.0f) * cellScale.x;
                float positionY = tr.localPosition.y + (y - (gridSize.y - 1.0f) / 2.0f) * cellScale.y;
                Vector3Int position = new Vector3Int((int)Mathf.FloorToInt(positionX), (int)Mathf.FloorToInt(positionY), 0);
                float[] obs = { tileMap.GetTile(position) == null ? 0.0f : 1.0f };
                bufferSensor.AppendObservation(obs);

                //디버깅 사용시 Point들의 위치를 시각적으로 표현
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
    
    //브레인(정책)으로 부터 전달 받은 행동을 실행하는 메소드
    public override void OnActionReceived(ActionBuffers actions)
    {
        //연속적인 값들을 이산화 시킴
        float h = Mathf.Clamp(actions.ContinuousActions[0], -1.0f, 1.0f);
        if (h > 0.5) { Ball.Move(rb, moveSpeed); }
        else if (h < -0.5) { Ball.Move(rb, -moveSpeed); }
        else { Ball.Move(rb, 0); }

        //Max Step이 될경우 Dead_Zone 충돌과 같은 Reward를 받도록 설정 
        AddReward(-(1.0f / MaxStep));
    }
    
    //개발자(사용자)가 직접 명령을 내릴때 호출하는 메소드(주로 테스트용도 또는 모방학습에 사용)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
    }

    //==================================================
    //유니티 함수

    //Trigger된 오브젝트와 충돌시
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (trainning)
        {
            //Target과 충돌시
            if (other.CompareTag("Target"))
            {
                AddReward(+1.0f);
                debugRecent++;
                EndEpisode();
            }
            //Dead_Zone과 충돌시
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