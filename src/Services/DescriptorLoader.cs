using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class DescriptorLoader
    {
        private readonly Dictionary<string, Dictionary<string, SpriteAnimation>> loadedSpriteAnimations = new Dictionary<string, Dictionary<string, SpriteAnimation>>();

        public DescriptorLoader()
        {

        }

        public Dictionary<string, SpriteAnimation> LoadSpriteAnimations(string name)
        {
            // Return cache if exists
            if (loadedSpriteAnimations.ContainsKey(name))
            {
                return loadedSpriteAnimations[name];
            }

            // Read file
            string fileName = "Content/Descriptors/Sprites/" + name + ".sprite";
            string[] lines;
            try { lines = File.ReadAllLines(fileName); }
            catch (DirectoryNotFoundException) { throw new GameException("Unable to find sprite descriptor file " + fileName); }

            var animations = new Dictionary<string, SpriteAnimation>();
            var currentAnimation = new SpriteAnimation();

            foreach (string line in lines)
            {
                string[] components = line.Split(' ');

                switch (components[0])
                {
                    case "an":
                        currentAnimation = new SpriteAnimation()
                        {
                            Name = components[1],
                            Size = new Size(int.Parse(components[2]), int.Parse(components[3])),
                            HitBoxOffset = new Point(int.Parse(components[4]), int.Parse(components[5])),
                            Length = int.Parse(components[6]),
                            Speed = float.Parse(components[7], CultureInfo.InvariantCulture),
                            Looped = int.Parse(components[8]) == 1,
                            Directions = new Dictionary<Direction, SpriteDirection>()
                        };
                        break;
                    case "di":
                        Direction direction = (Direction)int.Parse(components[1]);
                        var spriteDirection = new SpriteDirection
                        {
                            Offset = new Point(
                                int.Parse(components[2]),
                                int.Parse(components[3])
                            ),
                            FlippedHorizontally = int.Parse(components[4]) == 1,
                            FlippedVertically = int.Parse(components[5]) == 1
                        };
                        try
                        {
                            spriteDirection.OverrideHitBoxOffset = new Point(
                                int.Parse(components[6]),
                                int.Parse(components[7])
                            );
                        }
                        catch (IndexOutOfRangeException) {}

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
