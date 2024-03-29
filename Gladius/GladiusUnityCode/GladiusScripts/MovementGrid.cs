﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using UnityEngine;

public class MovementGrid : MonoBehaviour
{
    public int GridSize
    {
        get;
        set;
    }

    public Arena Arena
    {
        get
        {
            return GladiusGlobals.GameStateManager.ArenaStateCommon.Arena;
        }
    }

    public void ResetGrid()
    {
        for (int i = 0; i < GridSize; ++i)
        {
            for (int j = 0; j < GridSize; ++j)
            {
                m_arenaSquares[i, j] = GridSquareType.None;
                m_skillActiveSquares[i, j] = false;
            }
        }
    }


    public void Start()
    {
        GladiusGlobals.GameStateManager.ArenaStateCommon.MovementGrid = this;
        GridSize = 32;
        m_arenaSquares = new GridSquareType[GridSize, GridSize];
        m_skillActiveSquares = new bool[GridSize, GridSize];

        // always want to know about actor changes. unlike actionevents.
        EventManager.BaseActorChanged += new EventManager.BaseActorSelectionChanged(EventManager_BaseActorChanged);
        BuildDictionary();
        BuildMesh();
        RebuildMesh = true;
        Vector3 pos = gameObject.transform.position;
        pos.y += 0.2f;
        gameObject.transform.position = pos;
        m_cursorObject = new GameObject();
    }

    private void BuildDictionary()
    {
        m_dictionary[GridSquareType.BL] = CTTurnBL;
        m_dictionary[GridSquareType.BR] = CTTurnBR;
        m_dictionary[GridSquareType.TL] = CTTurnTL;
        m_dictionary[GridSquareType.TR] = CTTurnTR;
        m_dictionary[GridSquareType.ZB] = CTEndMoveB;
        m_dictionary[GridSquareType.ZT] = CTEndMoveT;
        m_dictionary[GridSquareType.ZL] = CTEndMoveL;
        m_dictionary[GridSquareType.ZR] = CTEndMoveR;
        m_dictionary[GridSquareType.FH] = CTForwardH;
        m_dictionary[GridSquareType.FV] = CTForwardV;

        m_dictionary[GridSquareType.BLD] = CTTurnBLDisabled;
        m_dictionary[GridSquareType.BRD] = CTTurnBRDisabled;
        m_dictionary[GridSquareType.TLD] = CTTurnTLDisabled;
        m_dictionary[GridSquareType.TRD] = CTTurnTRDisabled;
        m_dictionary[GridSquareType.ZBD] = CTEndMoveBDisabled;
        m_dictionary[GridSquareType.ZTD] = CTEndMoveTDisabled;
        m_dictionary[GridSquareType.ZLD] = CTEndMoveLDisabled;
        m_dictionary[GridSquareType.ZRD] = CTEndMoveRDisabled;
        m_dictionary[GridSquareType.FHD] = CTForwardHDisabled;
        m_dictionary[GridSquareType.FVD] = CTForwardVDisabled;

        m_dictionary[GridSquareType.Target] = CTTarget;
        m_dictionary[GridSquareType.Select] = CTSelect;
        m_dictionary[GridSquareType.TargetSelect] = CTTargetSelect;
        m_dictionary[GridSquareType.Occupied] = CTCross;
        m_dictionary[GridSquareType.SM] = CTCross;
        m_dictionary[GridSquareType.None] = CTZero;
        m_dictionary[GridSquareType.Blank] = CTBlank;
        m_dictionary[GridSquareType.Blocked] = CTBlocked;
    }


    public bool RebuildMesh
    {
        get;
        set;
    }

    public void BuildMaskForGrid(BaseActor actor, Point centerPoint, AttackSkill skill)
    {
        int distance = GladiusGlobals.PathDistance(actor.ArenaPoint, centerPoint);
        DrawSkill(skill.SkillRangeName, skill.SkillRange, actor.ArenaPoint, centerPoint, true);
        DrawSkill(skill.SkillExcludeRangeName, skill.SkillExcludeRange, actor.ArenaPoint, centerPoint, false);
    }



    public void DrawSkill(String name, int range, Point start, Point end, bool val)
    {
        int distance = GladiusGlobals.PathDistance(start, end);
        if (distance > range)
        {
            return;
        }

        switch (name)
        {
            case "Self":
                SetSkillActivePoint(start, val);
                break;

            case "Square":
            case "Square2x2":
                break;

            case "Plus":
                BuildCross(end, 1, val);
                break;
            case "Plus2x2":
                BuildCross(end, 2, val);
                break;
            case "Plus3x3":
                BuildCross(end, 3, val);
                break;
            case "Linear":
                break;
            case "Star":
                BuildStar(start, range, val);
                break;
            case "Diamond":
                BuildDiamond(start, range, val);
                break;
            case "Cone":
                BuildCone(start, end, range, val);
                break;
        }
    }

    public void BuildCross(Point start, int armLength, bool val)
    {
        for (int i = 0; i < armLength; ++i)
        {
            SetSkillActivePoint(Point.Add(start, new Point(i, 0)), val);
            SetSkillActivePoint(GladiusGlobals.Add(start, new Point(-i, 0)), val);
            SetSkillActivePoint(GladiusGlobals.Add(start, new Point(0, i)), val);
            SetSkillActivePoint(GladiusGlobals.Add(start, new Point(0, -i)), val);
        }
    }

    private int[] m_coneSlots = new int[] { 1, 3, 3, 5, 5, 5, 7, 7, 7, 7 };

    public void BuildCone(Point startPoint, Point endPoint, int length, bool val)
    {
        Point offset1 = GladiusGlobals.CardinalNormalize(GladiusGlobals.Subtract(endPoint, startPoint));
        Point offset2 = GladiusGlobals.Cross(offset1);

        for (int i = 0; i < length; ++i)
        {
            int slotCount = m_coneSlots[i];
            Point rowPoint = GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(offset1, i));
            int midPoint = slotCount / 2;
            for (int j = 0; j < slotCount; ++j)
            {
                int diff = j - midPoint;
                Point offPoint = GladiusGlobals.Mult(offset2, diff);
                Point p = GladiusGlobals.Add(rowPoint, offPoint);
                SetSkillActivePoint(p, val);
            }
        }
    }

    public void BuildDiamond(Point startPoint, int armLength, bool val)
    {
        int rowCount = armLength;
        Point offset2 = new Point(1, 0);

        for (int i = 0; i < armLength; i++)
        {
            int midPoint = rowCount / 2;
            for (int j = 0; j < rowCount; ++j)
            {
                int diff = j - midPoint;
                Point offPoint = GladiusGlobals.Mult(offset2, diff);
                offPoint.Y = i;
                Point offPoint2 = new Point(offPoint.X, -offPoint.Y);
                Point p = GladiusGlobals.Add(startPoint, offPoint);
                Point p2 = GladiusGlobals.Add(startPoint, offPoint2);
                SetSkillActivePoint(p, val);
                SetSkillActivePoint(p2, val);

            }
            rowCount -= 2;
        }
    }

    public void BuildStar(Point startPoint, int armLength, bool val)
    {
        Point tl = new Point(-1, 1);
        Point t = new Point(0, 1);
        Point tr = new Point(1, 1);
        Point l = new Point(-1, 0);
        Point r = new Point(1, 0);
        Point bl = new Point(-1, -1);
        Point b = new Point(0, -1);
        Point br = new Point(1, -1);

        for (int i = 0; i < armLength; ++i)
        {
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(tl, i)), val);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(t, i)), val);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(tr, i)), val);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(l, i)), val);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(r, i)), val);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(bl, i)), val);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(b, i)), val);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(br, i)), val);
        }

    }

    public void BuildCenteredGrid(Point centerPoint, int size, bool val)
    {
        if (size > 0)
        {
            int width = size;//((size - 1) / 2);

            for (int i = -width; i <= width; ++i)
            {
                for (int j = -width; j <= width; ++j)
                {
                    Point p = new Point(centerPoint.X + i, centerPoint.Y + j);
                    SetSkillActivePoint(p, val);
                }
            }
        }
    }



    public void BuildMinMaxCircle(Point centerPoint, int min, int max, bool val)
    {
        int width = max;//((max - 1) / 2);
        float min2 = min * min;
        float max2 = max * max;
        for (int i = -width; i <= width; ++i)
        {
            for (int j = -width; j <= width; ++j)
            {
                Point p = new Point(centerPoint.X + i, centerPoint.Y + j);
                float dist2 = GladiusGlobals.PointDist2(p, centerPoint);
                if (dist2 >= min2 && dist2 <= max2)
                {
                    SetSkillActivePoint(p, val);
                }
            }
        }

    }



    public void SetSkillActivePoint(Point p, bool val)
    {
        if (p.X > 0 && p.X < GridSize && p.Y > 0 && p.Y < GridSize)
        {
            m_skillActiveSquares[p.X, p.Y] = val;
        }
    }



    private void RebuildForActor()
    {
        if (CurrentActor != null)
        {
            ResetGrid();
            DrawIfValid(CurrentActor.ArenaPoint, CurrentActor, GridSquareType.Select);

            //if (GladiusGlobals.TurnManager.CurrentControlState == ControlState.UsingGrid)
            {

                if (CurrentActor.CurrentAttackSkill != null)
                {
                    if (CurrentActor.CurrentAttackSkill.IsMoveToAttack)
                    {
                        DrawMovementPath(CurrentActor, CurrentActor.WayPointList);
                    }

                    for (int i = 0; i < GridSize; ++i)
                    {
                        for (int j = 0; j < GridSize; ++j)
                        {
                            if (m_skillActiveSquares[i, j])
                            {
                                m_arenaSquares[i, j] = CursorForSquare(new Point(i, j), CurrentActor);
                            }
                        }
                    }


                    // draw the cursor for attackskill (different to move path);
                    //DrawAttackSkillCursor(CurrentCursorPoint, CurrentActor);

                    // if the current position is on a valid target(which wouldn't be in the movement list
                    // then draw a target/select icon.
                    if (CursorOnTarget(CurrentActor))
                    {
                        DrawIfValid(CurrentCursorPoint, CurrentActor, GridSquareType.TargetSelect);
                    }
                }

                // draw target markers under all players of different team.
                //foreach (BaseActor actor in GladiusGlobals.TurnManager.AllActors)
                //{
                //    if (actor.Team != CurrentActor.Team)
                //    {
                //        DrawIfValid(actor.CurrentPosition, actor, GridSquareType.Target);
                //    }
                //}
            }

            RebuildMeshData();
        }
    }

    public void Update()
    {
        if (CurrentActor != null)
        {
            //renderer.material.SetColor("Main Color", CurrentActor.TeamColour);
        }

        if (RebuildMesh)
        {
            RebuildForActor();
            RebuildMesh = false;
        }
    }


    public void DrawIfValid(Point p, BaseActor actor, GridSquareType cursor = GridSquareType.None)
    {
        if (Arena.InLevel(p))
        {
            if (cursor == GridSquareType.None)
            {
                cursor = CursorForSquare(p, actor);
            }
            m_arenaSquares[p.X, p.Y] = cursor;
        }
    }


    public void DrawIfValid(Point prevPoint, Point point, Point nextPoint, BaseActor actor, bool prevExists, bool nextExists, bool disabled, GridSquareType cursor = GridSquareType.None)
    {

        if (cursor == GridSquareType.None)
        {
            cursor = CursorForSquare(point, actor);
        }

        if (cursor == GridSquareType.None)
        {

            Vector2 v2 = new Vector2(point.X, point.Y);
            Vector2 v2p = new Vector2(prevPoint.X, prevPoint.Y);
            Vector2 v2n = new Vector2(nextPoint.X, nextPoint.Y);

            //Matrix rot = Matrix.Identity;

            int steps = 0;

            Vector2 diffPrevious = v2 - v2p;
            Vector3 diffNext = v2n - v2;

            if (prevExists)
            {
                Side enterSide = Side.Left;
                Side exitSide = Side.Right;
                if (diffPrevious.x == 1)
                {
                    enterSide = Side.Left;
                }
                else if (diffPrevious.x == -1)
                {
                    enterSide = Side.Right;
                }
                else if (diffPrevious.y == -1)
                {
                    enterSide = Side.Bottom;
                }
                else
                {
                    enterSide = Side.Top;
                }

                if (diffNext.x == 1)
                {
                    exitSide = Side.Right;
                }
                else if (diffNext.x == -1)
                {
                    exitSide = Side.Left;
                }
                else if (diffNext.y == -1)
                {
                    exitSide = Side.Top;
                }
                else
                {
                    exitSide = Side.Bottom;
                }

                if (nextExists)
                {

                    if (CompareSide(enterSide, exitSide, Side.Left, Side.Right))
                    {
                        cursor = disabled ? GridSquareType.FHD : GridSquareType.FH;
                    }
                    else if (CompareSide(enterSide, exitSide, Side.Top, Side.Bottom))
                    {
                        cursor = disabled ? GridSquareType.FVD : GridSquareType.FV;
                    }
                    else if (CompareSide(enterSide, exitSide, Side.Left, Side.Top))
                    {
                        cursor = disabled ? GridSquareType.TLD : GridSquareType.TL;
                    }
                    else if (CompareSide(enterSide, exitSide, Side.Left, Side.Bottom))
                    {
                        cursor = disabled ? GridSquareType.BLD : GridSquareType.BL;
                    }
                    else if (CompareSide(enterSide, exitSide, Side.Right, Side.Top))
                    {
                        cursor = disabled ? GridSquareType.TRD : GridSquareType.TR;
                    }
                    else if (CompareSide(enterSide, exitSide, Side.Right, Side.Bottom))
                    {
                        cursor = disabled ? GridSquareType.BRD : GridSquareType.BR;
                    }
                }
                else
                {
                    switch (enterSide)
                    {
                        case (Side.Left):
                            cursor = disabled ? GridSquareType.ZLD : GridSquareType.ZL;
                            break;
                        case (Side.Right):
                            cursor = disabled ? GridSquareType.ZRD : GridSquareType.ZR;
                            break;
                        case (Side.Top):
                            cursor = disabled ? GridSquareType.ZTD : GridSquareType.ZT;
                            break;
                        case (Side.Bottom):
                            cursor = disabled ? GridSquareType.ZBD : GridSquareType.ZB;
                            break;
                    }
                }
            }
            else
            {
                cursor = GridSquareType.SM;
            }
        }
        m_arenaSquares[point.X, point.Y] = cursor;
    }

    public bool CompareSide(Side side1, Side side2, Side check1, Side check2)
    {
        return (side1 == check1 && side2 == check2) || (side1 == check2 && side2 == check1);
    }


    public GridSquareType CursorForSquare(Point p, BaseActor actor)
    {
        if (Arena.InLevel(p))
        {
            if (Arena.IsPointOccupied(p))
            {
                return GridSquareType.Target;
            }
            else
            {
                BaseActor target = Arena.GetActorAtPosition(p);
                if (GladiusGlobals.GameStateManager.ArenaStateCommon.CombatEngine.IsValidTarget(CurrentActor, target, CurrentActor.CurrentAttackSkill))
                {
                    if (GladiusGlobals.GameStateManager.ArenaStateCommon.CombatEngine.IsAttackerInRange(actor, target, cursorOnly: true))
                    {
                        return GridSquareType.Select;
                    }
                    else
                    {
                        return GridSquareType.Target;
                    }
                }
            }
        }
        return GridSquareType.None;
    }

    private List<Point> m_pointsCopy = new List<Point>();
    public void DrawMovementPath(BaseActor actor, List<Point> points)
    {
        m_pointsCopy.Clear();
        m_pointsCopy.AddRange(points);

        // if the last point we're moving to is next to our target , then we should probably move to the target as well?
        // still think the logic is wrong.
        if (actor.Target != null && m_pointsCopy.Count > 0)
        {
            if (GladiusGlobals.PathDistance(actor.Target.ArenaPoint, m_pointsCopy[m_pointsCopy.Count - 1]) == 1)
            {
                //m_pointsCopy.Add(actor.Target.ArenaPoint);
            }
        }

        int numPoints = m_pointsCopy.Count;
        Point prev = new Point();
        Point curr = new Point();
        Point next = new Point();

        int skillRange = CurrentActor.CurrentAttackSkill.TotalSkillRange;

        for (int i = 0; i < numPoints; ++i)
        {
            GridSquareType squareType = GridSquareType.None;
            bool disabled = !CurrentActor.CurrentAttackSkill.InRange(i);
            prev = curr;
            if (i == 0)
            {
                prev = CurrentActor.ArenaPoint;
            }


            curr = m_pointsCopy[i];
            if (i < (numPoints - 1))
            {
                next = m_pointsCopy[i + 1];
                DrawIfValid(prev, curr, next, actor, (i >= 0), true, disabled, squareType);
            }
            else
            {
                DrawIfValid(prev, curr, next, actor, true, false,disabled);
            }
        }
    }

    public bool CursorOnTarget(BaseActor source)
    {
        BaseActor ba = Arena.GetActorAtPosition(CurrentCursorPoint);
        return (ba != null && GladiusGlobals.GameStateManager.ArenaStateCommon.CombatEngine.IsValidTarget(source, ba, source.CurrentAttackSkill));
    }


    public Point CurrentCursorPoint
    {
        get
        {
            return m_currentPosition;
        }
        set
        {
            LastPosition = CurrentCursorPoint;
            m_currentPosition = value;

            m_cursorObject.transform.position = CurrentV3;

            if (!Arena.InLevel(m_currentPosition))
            {
                int ibreak = 0;
            }
            GladiusGlobals.GameStateManager.CurrentStateData.CameraManager.ReparentTarget(m_cursorObject);

        }
    }

    public Vector3 CurrentV3
    {

        get
        {
            return V3ForSquare(CurrentCursorPoint);
        }
    }

    private Point m_lastPosition;
    public Point LastPosition
    {
        get
        {
            return m_lastPosition;
        }
        set
        {
            m_lastPosition = value;
        }
    }

    public Vector3 LastV3
    {

        get
        {
            return V3ForSquare(LastPosition);
        }
    }



    public Vector3 V3ForSquare(Point p)
    {
        Vector3 result = Arena.ArenaToWorld(p);
        result.y += m_hover;
        return result;
    }

    public void BuildForPlayer(BaseActor actor)
    {

        // build a grid of 'x' values centered around player.
    }

    public BaseActor CurrentActor
    {
        get;
        set;
    }

    void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
    {
        CurrentActor = e.New;
        CurrentCursorPoint = CurrentActor.ArenaPoint;
        RebuildMesh = true;

    }

    public void CursorTexCoords(String type, BaseActor actor, ref Vector4 result)
    {
        TextureRegion textureRegion = m_moveAtlas.GetRegion(type);

    }

    public bool IsTeamColouredCursor(String type)
    {
        //return type == CursorType.EndMove || type == CursorType.StartMove || type == CursorType.InterMove || type == CursorType.TurnMove;
        return false;
    }

    private void BuildMesh()
    {
        int size = GridSize;
        int totalVertices = 4 * size * size;
        int totalIndices = 6 * size * size;
        m_mesh = new Mesh();

        m_vertices = new Vector3[totalVertices];
        m_normals = new Vector3[totalVertices];
        m_uvs = new Vector2[totalVertices];

        int[] triangles = new int[totalIndices];


        // need to double up as each square can have it's own uv.
        m_moveAtlasTexture = Resources.Load<Texture2D>("GladiusUI/Arena/ArenaUIAtlas");
        m_moveAtlas = new TextureAtlas();
        m_moveAtlas.BuildJSON("GladiusUI/Arena/ArenaUIAtlas_data",m_moveAtlasTexture);

        TextureRegion textureRegion = m_moveAtlas.GetRegion(CTCross);
        float meshScalar = 1f;
        int vcount = 0;
        int icount = 0;

        Vector3 topLeft = new Vector3(1, 0, 1) * GridSize * meshScalar;
        topLeft *= -0.5f;

        // minor offset so characters are centered.
        topLeft += new Vector3(-0.5f, 0, -0.5f);

        Rect bounds = textureRegion.BoundsUV;

        //FIXME look at changing grid height point so it aligns with terrain better?

        for (int y = 0; y < size; ++y)
        {
            for (int x = 0; x < size; ++x)
            {

                m_vertices[vcount] = topLeft + (new Vector3(x, 0, y) * meshScalar);
                m_vertices[vcount + 1] = topLeft + (new Vector3(x + 1, 0, y) * meshScalar);
                m_vertices[vcount + 2] = topLeft + (new Vector3(x + 1, 0, y + 1) * meshScalar);
                m_vertices[vcount + 3] = topLeft + (new Vector3(x, 0, y + 1) * meshScalar);

                m_uvs[vcount] = new Vector2(bounds.x, bounds.y);
                m_uvs[vcount + 1] = new Vector2(bounds.width, bounds.y);
                m_uvs[vcount + 2] = new Vector2(bounds.width, bounds.height);
                m_uvs[vcount + 3] = new Vector2(bounds.x, bounds.height);

                m_normals[vcount] = Vector3.up;
                m_normals[vcount + 1] = Vector3.up;
                m_normals[vcount + 2] = Vector3.up;
                m_normals[vcount + 3] = Vector3.up;


                triangles[icount] = vcount + 2;
                triangles[icount + 1] = vcount + 1;
                triangles[icount + 2] = vcount + 0;

                triangles[icount + 3] = vcount + 0;
                triangles[icount + 4] = vcount + 3;
                triangles[icount + 5] = vcount + 2;


                vcount += 4;
                icount += 6;
            }
        }

        m_mesh.vertices = m_vertices;
        m_mesh.normals = m_normals;
        m_mesh.uv = m_uvs;
        m_mesh.triangles = triangles;

        m_mesh.RecalculateBounds();
        m_meshFilter = (MeshFilter)transform.GetComponent(typeof(MeshFilter));
        m_meshFilter.mesh = m_mesh;
        //renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        //renderer.material = new Material(Shader.Find("Gladius/UITeamColours"));

        m_meshFilter.GetComponent<Renderer>().material.mainTexture = m_moveAtlasTexture;
    }

    private void RebuildMeshData()
    {
        //return;
        int size = GridSize;
        int vcount = 0;
        Rect uvg = new Rect();
        RaycastHit hitResult = new RaycastHit();
        for (int y = 0; y < size; ++y)
        {
            for (int x = 0; x < size; ++x)
            {
                UVForGrid(x, y, ref uvg);
                m_uvs[vcount] = new Vector2(uvg.x, uvg.y);
                m_uvs[vcount + 1] = new Vector2(uvg.width, uvg.y);
                m_uvs[vcount + 2] = new Vector2(uvg.width, uvg.height);
                m_uvs[vcount + 3] = new Vector2(uvg.x, uvg.height);
                Ray ray = new Ray(new Vector3(x,10,y),new Vector3(0,-1,0));
                float height = 0f;
                if (Physics.Raycast(ray, out hitResult))
                {
                    if (hitResult.collider.tag == "environment")
                    {
                        //height = hitResult.point.y;
                        if (height < -0.1f)
                        {
                            int ibreak = 0;
                        }
                        //height = vcount / 10f;

                    }
                }

                m_vertices[vcount + 0].y = height;
                m_vertices[vcount + 1].y = height;
                m_vertices[vcount + 2].y = height;
                m_vertices[vcount + 3].y = height;

                vcount += 4;
            }
        }

        m_mesh.uv = m_uvs;
        m_mesh.vertices = m_vertices;
    }

    private void UVForGrid(int x, int y, ref Rect uv)
    {
        GridSquareType squareType = m_arenaSquares[x, y];
        String regionKey = m_dictionary[squareType];
        TextureRegion tr = m_moveAtlas.GetRegion(regionKey);
        uv = tr.BoundsUV;
    }

    private Vector2[] m_uvs;
    private Vector3[] m_vertices;
    private Vector3[] m_normals;

    private bool[,] m_skillActiveSquares;


    private GridSquareType[,] m_arenaSquares;
    private TextureAtlas m_moveAtlas;
    private Texture2D m_moveAtlasTexture;
    public Vector3 m_cursorMovement = Vector3.zero;
    public int CurrentCursorSize = 5;
    public const float m_hover = 0.01f;

    public GameObject m_cursorObject;

    public Point m_currentPosition;

    private Mesh m_mesh;
    private MeshFilter m_meshFilter;

    private Dictionary<GridSquareType, String> m_dictionary = new Dictionary<GridSquareType, String>();

    public const String CTZero = "Zero.png";//"Zero.png";
    public const String CTBlank = "Blank.png";
    public const String CTSelect = "Select.png";
    public const String CTCross = "Cross.png";
    public const String CTBlocked = "Blocked.png";

    public const String CTForwardH = "ForwardH.png";
    public const String CTForwardV = "ForwardV.png";

    public const String CTForwardHDisabled = "ForwardHDisabled.png";
    public const String CTForwardVDisabled = "ForwardVDisabled.png";


    public const String CTTurnBL = "TurnBL.png";
    public const String CTTurnBR = "TurnBR.png";
    public const String CTTurnTL = "TurnTL.png";
    public const String CTTurnTR = "TurnTR.png";

    public const String CTEndMoveB = "EndMoveB.png";
    public const String CTEndMoveT = "EndMoveT.png";
    public const String CTEndMoveL = "EndMoveL.png";
    public const String CTEndMoveR = "EndMoveR.png";


    public const String CTTurnBLDisabled = "TurnBLDisabled.png";
    public const String CTTurnBRDisabled = "TurnBRDisabled.png";
    public const String CTTurnTLDisabled = "TurnTLDisabled.png";
    public const String CTTurnTRDisabled = "TurnTRDisabled.png";

    public const String CTEndMoveBDisabled = "EndMoveBDisabled.png";
    public const String CTEndMoveTDisabled = "EndMoveTDisabled.png";
    public const String CTEndMoveLDisabled = "EndMoveLDisabled.png";
    public const String CTEndMoveRDisabled = "EndMoveRDisabled.png";


    public const String CTTarget = "TargetRed.png";
    public const String CTTargetSelect = "TargetSelect.png";

}

public enum GridSquareType
{
    None,
    Blank,
    Select,
    Target,
    TargetSelect,
    Occupied,
    Blocked,
    SM,
    BL,
    BR,
    TL,
    TR,
    ZB,
    ZT,
    ZL,
    ZR,
    FH,
    FV,
    BLD,
    BRD,
    TLD,
    TRD,
    ZBD,
    ZTD,
    ZLD,
    ZRD,
    FHD,
    FVD
}


public enum Side
{
    Left,
    Right,
    Top,
    Bottom
}

public enum Rot
{
    Zero,
    Ninety,
    OneEighty,
    TwoSeventy
}
