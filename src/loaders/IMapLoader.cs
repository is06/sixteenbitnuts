﻿namespace SixteenBitNuts
{
    public interface IMapLoader
    {
        void LoadMapData(Map map, string name);
        void SetEntityFactory(EntityFactory factory);
    }
}
