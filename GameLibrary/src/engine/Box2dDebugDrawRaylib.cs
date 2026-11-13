using System.Numerics;
using Box2D.NET;
using static Box2D.NET.B2Types;
using Raylib_cs;

namespace GameLibrary;

static class Box2dDebugDrawRaylib
{
    public static B2DebugDraw Create()
    {
        var debugDraw = b2DefaultDebugDraw();
        debugDraw.DrawPolygonFcn = DrawPolygon;
        debugDraw.DrawSolidPolygonFcn = DrawSolidPolygon;
        debugDraw.DrawCircleFcn = DrawCircle;
        debugDraw.DrawSolidCircleFcn = DrawSolidCircle;
        debugDraw.DrawSolidCapsuleFcn = DrawSolidCapsule;
        debugDraw.drawLineFcn = DrawSegment;
        debugDraw.DrawTransformFcn = DrawTransform;
        debugDraw.DrawPointFcn = DrawPoint;
        debugDraw.DrawStringFcn = DrawString;

        debugDraw.drawShapes = true;
        debugDraw.drawJoints = true;
        debugDraw.drawJointExtras = true;
        // debugDraw.drawBounds = true;
        debugDraw.drawMass = true;
        debugDraw.drawBodyNames = true;
        debugDraw.drawGraphColors = true;
        debugDraw.drawContactNormals = true;
        // debugDraw.drawContactForces = true;
        // debugDraw.drawContactFeatures = true;
        // debugDraw.drawFrictionForces = true;
        debugDraw.drawIslands = true;

        return debugDraw;
    }
    
    private static void DrawPolygon(ReadOnlySpan<B2Vec2> vertices, int vertexCount, B2HexColor color, object o)
    {
        for (int i = 0; i < vertexCount; i++)
        {
            Raylib.DrawLineV(vertices[i].ToVec2(), vertices[(i + 1) % vertexCount].ToVec2(), color.ToRaylib());
        }
    }
    
    private static void DrawSolidPolygon(in B2Transform transform, ReadOnlySpan<B2Vec2> vertices, int vertexCount, float radius, B2HexColor color, object o)
    {
        Vector2[] verts = vertices.ToArray().Select(v => v.ToVec2()).ToArray();

        for (var i = 0; i < verts.Length; i++)
        {
            verts[i] = new Vector2(verts[i].X * transform.q.c - verts[i].Y * transform.q.s, verts[i].X * transform.q.s + verts[i].Y * transform.q.c);
            verts[i] += transform.p.ToVec2();
        }
        for (int i = 0; i < vertexCount; i++)
        {
            Raylib.DrawLineV(verts[i], verts[(i + 1) % vertexCount], color.ToRaylib());
        }
    }
    
    private static void DrawCircle(in B2Vec2 center, float radius, B2HexColor color, object o)
    {
        Raylib.DrawCircleLinesV(center.ToVec2(), radius, color.ToRaylib());
    }
    
    private static void DrawSolidCircle(in B2Transform transform, float radius, B2HexColor color, object o)
    {
        Vector2 center = transform.p.ToVec2();
        Raylib.DrawCircleV(center, radius, Raylib.ColorAlpha(color.ToRaylib(), 0.25f));
        Raylib.DrawCircleLinesV(center, radius, color.ToRaylib());
        Raylib.DrawLineV(center, center + new Vector2(transform.q.c, transform.q.s) * radius, color.ToRaylib());
    }
    
    private static void DrawSolidCapsule(in B2Vec2 point1, in B2Vec2 point2, float radius, B2HexColor color, object o)
    {
        Vector2 p1 = point1.ToVec2();
        Vector2 p2 = point2.ToVec2();
        Raylib.DrawCircleV(p1, radius, color.ToRaylib());
        Raylib.DrawCircleV(p2, radius, color.ToRaylib());
        Raylib.DrawLineEx(p1, p2, radius, color.ToRaylib());
    }
    
    private static void DrawSegment(in B2Vec2 point1, in B2Vec2 point2, B2HexColor color, object o)
    {
        Vector2 p1 = point1.ToVec2();
        Vector2 p2 = point2.ToVec2();
        Raylib.DrawLineV(p1, p2, color.ToRaylib());
    }
    
    private static void DrawTransform(in B2Transform transform, object o)
    {
        DrawSolidCircle(transform, 4, B2HexColor.b2_colorAqua, o);
    }
    
    private static void DrawPoint(in B2Vec2 point, float size, B2HexColor color, object o)
    {
        Raylib.DrawCircleV(point.ToVec2(), size, color.ToRaylib());
    }
    
    private static void DrawString(in B2Vec2 point, string s, B2HexColor color, object o)
    {
        Vector2 p = point.ToVec2();
        Raylib.DrawText(s, (int)p.X, (int)p.Y, 4, color.ToRaylib());
    }
}