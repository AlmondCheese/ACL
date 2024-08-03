// this needs a rewrite

using ACL;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ACL.Graphics;

public class DrawBatch : IDisposable
{
    readonly GameInstance Game;
    private BasicEffect basicEffect;
    private VertexPositionColor[] vertices;
    private int[] indices;

    private int vertexCount;
    private int indexCount;
    bool isStarted;
    bool isDisposed;

    public DrawBatch(GameInstance game)
    {
        isDisposed = false;
        Game = game;

        basicEffect = new(game.GraphicsDevice)
        {
            World = Matrix.Identity,
            View = Matrix.Identity,
            Projection = Matrix.Identity,

            VertexColorEnabled = true,
            TextureEnabled = false,
            FogEnabled = false,
            LightingEnabled = false,
        };


        const uint MaxVertexCount = 2048;
        const uint MaxIndicesCount = MaxVertexCount * 3;

        vertices = new VertexPositionColor[MaxVertexCount];
        indices = new int[MaxIndicesCount];

        vertexCount = 0;
        indexCount = 0;

        isStarted = false;
    }

    public void Dispose()
    {
        if(isDisposed) return;

        basicEffect?.Dispose();
        isDisposed = true;
    }

    public void Begin()
    {
        if (isStarted) throw new Exception("Batching already started.");

        Viewport viewport = Game.GraphicsDevice.Viewport;
        basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);

        isStarted = true;
    }

    public void End()
    {
        Flush();
        isStarted = false;
    }

    public void Flush()
    {
        StartCheck();

        foreach(EffectPass pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, vertexCount, indices, 0, indexCount/3);
        }

        vertexCount = 0;
        indexCount = 0;
    }

    public void StartCheck()
    {
        if (!isStarted) throw new Exception("Batching wasn't started.");
    }

    public void CheckCapacity(uint uVertexCount, uint uIndexCount)
    {
        if(uVertexCount > vertices.Length)
        {
            throw new Exception($"Max vertex count limit reached: {vertices.Length}");
        }
        if(uIndexCount > indices.Length)
        {
            throw new Exception($"Max index count limit reached: {indices.Length}");
        }

        if(uVertexCount+ vertexCount > vertices.Length || uIndexCount + indexCount > indices.Length)
        {
            Flush();
        }
    }

    public void DrawRectangle(float x, float y, float width, float height, Color color)
    {
        // Check for conditions
        StartCheck();
        const uint rectVertexCount = 4;
        const uint rectIndexCount = 6;
        CheckCapacity(rectVertexCount, rectIndexCount);

        float left = x;
        float right = x + width;
        float top = y;
        float bottom = y + height;

        // Vertices
        Vector2 vertex0 = new(left, top);
        Vector2 vertex1 = new(right, top);
        Vector2 vertex2 = new(right, bottom);
        Vector2 vertex3 = new(left, bottom);

        // Indices
        indices[indexCount++] = 0 + vertexCount;
        indices[indexCount++] = 1 + vertexCount;
        indices[indexCount++] = 2 + vertexCount;
        indices[indexCount++] = 0 + vertexCount;
        indices[indexCount++] = 2 + vertexCount;
        indices[indexCount++] = 3 + vertexCount;

        vertices[vertexCount++] = new VertexPositionColor(new(vertex0, 0f), color);
        vertices[vertexCount++] = new VertexPositionColor(new(vertex1, 0f), color);
        vertices[vertexCount++] = new VertexPositionColor(new(vertex2, 0f), color);
        vertices[vertexCount++] = new VertexPositionColor(new(vertex3, 0f), color);
    }
}