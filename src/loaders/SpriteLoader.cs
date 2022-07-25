using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SixteenBitNuts
{
    public class SpriteLoader : ISpriteLoader
    {
        private readonly Dictionary<string, Dictionary<string, SpriteAnimation>> loadedSpriteAnimations;

        public SpriteLoader()
        {
            loadedSpriteAnimations = new Dictionary<string, Dictionary<string, SpriteAnimation>>();
        }

        public Dictionary<string, SpriteAnimation> LoadAnimations(string name)
        {
            if (loadedSpriteAnimations.ContainsKey(name))
            {
                return loadedSpriteAnimations[name];
            }

            string fileName = "Content/Definitions/Sprites/" + name + ".sprite";
            string[] lines = File.ReadAllLines(fileName);

            var animations = new Dictionary<string, SpriteAnimation>();
            var currentAnimation = new SpriteAnimation();

            foreach (string line in lines)
            {
                string[] chunks = line.Split(' ');

                switch (chunks[0])
                {
                    case "an":
                        currentAnimation = new SpriteAnimation
                        {
                            Name = chunks[1],
                            Size = new Point(int.Parse(chunks[2]), int.Parse(chunks[3])),
                            Offset = new Point(int.Parse(chunks[4]), int.Parse(chunks[5])),
                            Length = uint.Parse(chunks[6]),
                            Speed = float.Parse(chunks[7], CultureInfo.InvariantCulture),
                            IsLooped = int.Parse(chunks[8]) == 1,
                            Directions = new Dictionary<Direction, SpriteAnimationDirection>()
                        };
                        break;
                    case "di":
                        Direction direction = (Direction)int.Parse(chunks[1]);
                        var spriteDirection = new SpriteAnimationDirection
                        {
                            Offset = new Point(
                                int.Parse(chunks[2]),
                                int.Parse(chunks[3])
                            ),
                            IsFlippedHorizontally = int.Parse(chunks[4]) == 1,
                            IsFlippedVertically = int.Parse(chunks[5]) == 1
                        };
                        try
                        {
                            spriteDirection.OverrideOffset = new Point(
                                int.Parse(chunks[6]),
                                int.Parse(chunks[7])
                            );
                        }
                        catch (IndexOutOfRangeException) { }

                        if (currentAnimation.Directions != null)
                        {
                            currentAnimation.Directions[direction] = spriteDirection;
                        }
                        if (currentAnimation.Name != null)
                        {
                            animations[currentAnimation.Name] = currentAnimation;
                        }

                        break;
                }
            }

            loadedSpriteAnimations.Add(name, animations);
            return animations;
        }
    }
}
