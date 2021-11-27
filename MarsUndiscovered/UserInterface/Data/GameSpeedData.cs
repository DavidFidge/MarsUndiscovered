using System;

namespace MarsUndiscovered.UserInterface.Data
{
    public class GameSpeedData
    {
        public bool IsPaused { get; set; } = false;

        public int GameSpeedPercent { get; set; } = 100;

        public TimeSpan TotalGameTime { get; set; }
    }
}