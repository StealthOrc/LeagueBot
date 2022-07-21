using LeagueBot.Game.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBot.Game.Entities
{
    public class HealthBarEntity : IEntity
    {
        public bool Ally
        {
            get;
            private set;
        }
        public Point Position
        {
            get;
            private set;
        }

        public HealthBarEntity(bool ally, Point position)
        {
            this.Ally = ally;
            this.Position = position;
        }

    }
}
