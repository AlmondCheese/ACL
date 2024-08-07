﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ACL.UI;
public class Screen
{
    public GameInstance Game;
    protected SpriteBatch SpriteBatch => Game.SpriteBatch;
    public ContentManager Content => Game.Content;
    public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;
    public GameServiceContainer Services => Game.Services;
    public ComponentManager ComponentManager => Game.ComponentManager;
    public ScreenManager ScreenManager => Game.ScreenManager;
        
    public List<Component> ScreenComponents {get; private set;} = new List<Component>();
    public void AddScreenComponents(params Component[] components)
    {
        ScreenComponents.AddRange(components);
    }

    public Screen(GameInstance CurrentGame)
    {
        Game = CurrentGame;
    }
        
    public virtual void OnLoad(){}
    public virtual void OnUnload(){}
    public virtual void OnActivation(){}
    public virtual void OnUnactivation(){}

    public virtual void Update(GameTime gameTime){}
    public virtual void Draw(GameTime gameTime){}
}