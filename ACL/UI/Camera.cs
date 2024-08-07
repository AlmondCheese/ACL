using ACL.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ACL.UI;

public class Camera : INode
{
    public GameInstance Game;
    protected ComponentManager ComponentManager => Game.ComponentManager;
    protected PhysicsEngine PhysicsEngine => Game.PhysicsEngine;
    protected Viewport Viewport => Game.GraphicsDevice.Viewport; // Game viewport
    public Rectangle Cursor; // Cursor relative to the camera viewport

    // Subcomponents
    public List<Component> Subcomponents {get; set;} = new();
    public List<Component> PendingAdditions {get; set;} = new();
    internal List<Component> PendingRemovals {get; set;} = new();

    #region Properties
    public bool Enabled {get; set;} = true;
    public bool AllowComponentResize {get; set;} = false; // If set to true, the component manager will resize the components if the game's window is resized.
    public float Zoom {get; set;} = 1f;
    public Vector2 Position {get; set;} = Vector2.Zero;
    public float Rotation {get; set;} = 0f;
    public Component? Target {get; set;} // If not null, camera will follow the "target" component
    public Matrix Transform {get; protected set;} = Matrix.Identity; // Camera Matrix
    #endregion

    public Camera(GameInstance gameInstance)
    {
        Game = gameInstance;
        ComponentManager.AddCamera(this);
    }

    #region Methods
    // Methods for adding subcomponents.
    public void AddSubcomponents(params Component[] Components) 
    {
        foreach (Component component in Components)
        {
            component.Bound = this;
            PendingAdditions.Add(component);
            if (component is PhysicsComponent PhysicsObject)
            {
                PhysicsEngine.AddComponent(PhysicsObject);
            }
        }
    }
    public void AddSubcomponents(IEnumerable<Component> Components)
    {
        foreach (Component component in Components)
        {
            component.Bound = this;
            PendingAdditions.Add(component);
            if (component is PhysicsComponent PhysicsObject)
            {
                PhysicsEngine.AddComponent(PhysicsObject);
            }
        }
    }

    // Methods for removing subcomponents.
    public void RemoveSubcomponents(params Component[] Components) 
    {
        foreach (Component component in Components)
        {
            component.Bound = null;
            PendingRemovals.Add(component);
            if (component is PhysicsComponent PhysicsObject)
            {
                PhysicsEngine.RemoveComponent(PhysicsObject);
            }
        }
    }
    public void RemoveSubcomponents(IEnumerable<Component> Components) 
    {
        foreach (Component component in Components)
        {
            component.Bound = null;
            PendingRemovals.Add(component);
            if (component is PhysicsComponent PhysicsObject)
            {
                PhysicsEngine.RemoveComponent(PhysicsObject);
            }
        }
    }

    public void Update()
    {
        // Update Transform
        Transform = GetTransform();

        // Calculate cursor position from camera's perspective.
        Vector2 TransformedPosition = Vector2.Transform(new(Game.Cursor.X, Game.Cursor.Y), Transform);
        Cursor.X = (int)TransformedPosition.X; Cursor.Y = (int)TransformedPosition.Y;
    }

    public void Draw() {}
        
    public void SetTarget(Component Component) // Set the camera to follow a specific component
    {
        Target = Component;
    }

    public void RemoveTarget() // Set Target to null
    {
        Target = null;
    }

    public void Recenter() // This method is for calculating the camera's position in case there is a target assigned.
    {
        // Check for target
        if (Target != null)
        {
            Position = new(Target.ActualPosition.X + Target.Size.X / 2, Target.ActualPosition.Y + Target.Size.Y / 2);
        }
    } 

    public Matrix GetTransform() // Get the camera's tranform matrix;
    {
        return Transform = Matrix.CreateTranslation(new Vector3(-Position, 0f)) *
                           Matrix.CreateScale(Zoom) *
                           Matrix.CreateFromYawPitchRoll(0, 0, Rotation) *
                           Matrix.CreateTranslation(new Vector3(Viewport.Width * 0.5f, Viewport.Height * 0.5f, 0f));
    }
    #endregion
}