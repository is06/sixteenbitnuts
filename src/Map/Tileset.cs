﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class Tileset
    {
        private readonly Texture2D texture;
        private readonly Box debugHitBox;
        protected readonly Dictionary<int, TileElement> elements;

        public string Name { get; private set; }
        public Game Game { get; private set; }
        public Dictionary<string, TilesetGroup> Groups { get; private set; }

        public Tileset(Game game, string name)
        {
            // Properties
            Name = name;
            Game = game;
            Groups = new Dictionary<string, TilesetGroup>();

            // Components
            try
            {
                texture = Game.Content.Load<Texture2D>("Graphics/Tilesets/" + name);
            }
            catch (ContentLoadException e)
            {
                throw new GameException("Exception while loading tileset texture '" + name + "' (" + e.Message + ")");
            }
            
            debugHitBox = new Box(Game, new Rectangle(0, 0, 16, 16), Color.DarkRed);
            
            elements = new Dictionary<int, TileElement>();

            LoadFromFile("Content/Descriptors/Tilesets/" + name + ".tileset");
        }

        public void Draw(Vector2 position, Vector2 size, Vector2 offset, Vector2 scale, Matrix transform)
        {
            Game.SpriteBatch?.Begin(transformMatrix: transform, samplerState: SamplerState.PointClamp);

            Game.SpriteBatch?.Draw(
                texture: texture,
                position: new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y)),
                sourceRectangle: new Rectangle((int)offset.X, (int)offset.Y, (int)size.X, (int)size.Y),
                color: Color.White,
                rotation: 0f,
                origin: new Vector2(0, 0),
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );

            Game.SpriteBatch?.End();
        }

        public void DebugDraw(Point position, Point size, Color color, Matrix transform)
        {
            debugHitBox.Color = color;
            debugHitBox.Bounds = new Rectangle(position, size);
            debugHitBox.Update();
            debugHitBox.Draw(transform);
        }

        public Vector2 GetOffsetFromId(int id)
        {
            try
            {
                return elements[id].Offset;
            }
            catch (KeyNotFoundException)
            {
                throw new GameException("Offset for tile " + id + " was not found in tileset '" + Name + ".tileset'. The file may be empty.");
            }   
        }

        public Size GetSizeFromId(int id)
        {
            try
            {
                return elements[id].Size;
            }
            catch (KeyNotFoundException)
            {
                throw new GameException("Size for tile " + id + " was not found in tileset '" + Name + ".tileset'. The file may be empty.");
            }
        }

        public TileType GetTypeFromId(int id)
        {
            return elements[id].Type;
        }

        public TileLayer GetLayerFromId(int id)
        {
            return elements[id].Layer;
        }

        protected virtual void LoadFromFile(string fileName)
        {
            int elementIndex = 0;
            string groupName = "";

            string[] lines;
            try
            {
                lines = File.ReadAllLines(fileName);
            }
            catch (DirectoryNotFoundException)
            {
                throw new GameException("Unable to find tileset descriptor file " + fileName);
            }

            foreach (string line in lines)
            {
                string[] components = line.Split(' ');

                if (components[0] == "gr")
                {
                    TilesetGroup group;
                    group.Name = groupName = components[1];
                    group.IsAutoTilingEnabled = components[2] == "1";
                    group.Definitions = new Dictionary<TilesetGroupDefinitionType, TilesetGroupDefinition>();

                    Groups[groupName] = group;
                }

                if (components[0] == "gd")
                {
                    TilesetGroupDefinition definition;
                    definition.Type = TilesetGroupDefinition.GetTypeFromString(components[1]);
                    definition.TileIndex = int.Parse(components[2]);
                    if (definition.Type is TilesetGroupDefinitionType type)
                    {
                        Groups[groupName].Definitions?.Add(type, definition);
                    }
                }

                if (components[0] == "ti")
                {
                    TileElement element;
                    element.Size = new Size(int.Parse(components[1]), int.Parse(components[2]));
                    element.Offset = new Vector2(int.Parse(components[3]), int.Parse(components[4]));
                    element.Type = (TileType)int.Parse(components[5]);
                    element.Layer = (TileLayer)int.Parse(components[6]);

                    elements[elementIndex] = element;
                    elementIndex++;
                }
            }
        }
    }
}
