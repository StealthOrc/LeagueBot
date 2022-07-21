﻿using InputManager;
using LeagueBot.ApiHelpers;
using LeagueBot.Game.Entities;
using LeagueBot.Game.Enums;
using LeagueBot.Game.Misc;
using LeagueBot.IO;
using LeagueBot.Utils;
using LeagueBot.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeagueBot.Api
{
    /*
     * In Game Behaviour
     * -> Move to Nearest tower
     * -> Wait until minion spawn
     * -> Perpetual near entities analysis
     * -> Our objective is lane tower, once tower is destroyed, we go even futher
     * -> Perpetual state analysis States : {Agressive,Farming,Flee,Back,Walking to turret}
     * -> * Agressive if player is agressed and loose life fast
     * -> * Farming if NearChampions == 0 and NearMinions > 0
     * -> * Flee if life percentage is low
     * -> * We back after Flee
     * -> * Walk to our first turret
     */
    public class GameApi : IApi // Teams[] CameraIndex PlayerIndex
    {
        public Shop shop
        {
            get;
            private set;
        }
        public Camera camera
        {
            get;
            private set;
        }
        public Chat chat
        {
            get;
            private set;
        }
        private SideEnum? side
        {
            get;
            set;
        }
        public ActivePlayer player
        {
            get;
            private set;
        }
        public GameApi()
        {
            this.shop = new Shop(this);
            this.camera = new Camera(this);
            this.chat = new Chat(this);
            this.player = new ActivePlayer(this);
        }

       
        public void waitUntilGameStart()
        {
            while (true)
            {
                if (GameLCU.IsApiReady() && GameLCU.GetGameTime() > 2d)
                {
                    break;
                }
           
                Thread.Sleep(2000);
            }

        }

        public SideEnum getSide()
        {
            if (side == null)
            {
                side = GameLCU.GetPlayerSide();
            }
            return side.Value;
        }

        public void moveCenterScreen()
        {
            InputHelper.RightClick(1330, 490);
            BotHelper.InputIdle();
        }
    }
}
