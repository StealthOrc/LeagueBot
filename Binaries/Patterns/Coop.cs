using System;
using System.Collections.Generic;
using System.Drawing;
using LeagueBot;
using LeagueBot.Patterns;
using LeagueBot.Game.Enums;
using LeagueBot.Game.Misc;

namespace LeagueBot
{
    public class Coop : PatternScript
    {
        const double healthManaPercentThresh = 0.8d;

        private Point CastTargetPoint
        {
            get;
            set;
        }
        private int AllyIndex
        {
            get;
            set;
        }
        /* ADC Items Build path */
        private Item[] Items = new Item[]
        {
            new Item("Long Sword",350),
            new Item("Berserker",1100),
            new Item("Noonquiver",1300),
            new Item("Kraken Slayer",2100),
            new Item("Zeal",1050),
            new Item("Rapid",1450),
            new Item("B.F.",1300),
            new Item("Pickaxe",875),
            new Item("Infinity",1225),
            new Item("Lord",3000),
            new Item("Blood",3400),
        };

        public override bool ThrowException
        {
            get
            {
                return false;
            }
        }

        public override void Execute()
        {
            bot.log("Waiting for league of legends process...");

            bot.waitProcessOpen(Constants.GameProcessName);

            bot.waitUntilProcessBounds(Constants.GameProcessName, 1040, 800);

            bot.wait(200);

            bot.log("Waiting for game to load.");

            bot.bringProcessToFront(Constants.GameProcessName);
            bot.centerProcess(Constants.GameProcessName);

            game.waitUntilGameStart();

            bot.log("Game Started");

            bot.bringProcessToFront(Constants.GameProcessName);
            bot.centerProcess(Constants.GameProcessName);

            bot.wait(3000);

            if (game.getSide() == SideEnum.Blue)
            {
                CastTargetPoint = new Point(1084, 398);
                bot.log("We are blue side !");
            }
            else
            {
                CastTargetPoint = new Point(644, 761);
                bot.log("We are red side !");
            }

            game.player.upgradeSpellOnLevelUp();

            OnSpawnJoin();

            bot.log("Playing...");

            GameLoop();

            this.End();
        }
        private void BuyItems()
        {
            int golds = game.player.getGolds();

            game.shop.toogle();
            bot.wait(5000);
            foreach (Item item in Items)
            {
                if (item.Cost > golds)
                {
                    break;
                }
                if (!item.Buyed)
                {
                    game.shop.searchItem(item.Name);

                    game.shop.buySearchedItem();

                    item.Buyed = true;

                    golds -= item.Cost;
                }
            }

            game.shop.toogle();

        }
        private void CheckBuyItems()
        {
            int golds = game.player.getGolds();

            foreach (Item item in Items)
            {
                if (item.Cost > golds)
                {
                    break;
                }
                if (!item.Buyed)
                {
                    game.player.recall();
                    bot.wait(10000);
                    if (game.player.getManaPercent() >= healthManaPercentThresh)
                    {
                        OnSpawnJoin();
                    }
                    

                }
            }


        }

        private void GameLoop()
        {
            int level = game.player.getLevel();

            bool dead = false;

            bool isRecalling = false;

            while (bot.isProcessOpen(Constants.GameProcessName))
            {
                bot.bringProcessToFront(Constants.GameProcessName);

                bot.centerProcess(Constants.GameProcessName);

                int newLevel = game.player.getLevel();

                if (newLevel != level)
                {
                    level = newLevel;
                    game.player.upgradeSpellOnLevelUp();
                }


                if (game.player.dead())
                {
                    if (!dead)
                    {
                        dead = true;
                        isRecalling = false;
                        OnDie();
                    }

                    bot.wait(4000);
                    continue;
                }

                if (dead)
                {
                    dead = false;
                    OnRevive();
                    continue;
                }

                if (isRecalling)
                {
                    game.player.recall();
                    bot.wait(8500);

                    if (game.player.getManaPercent() >= healthManaPercentThresh)
                    {
                        OnSpawnJoin();
                        isRecalling = false;
                    }
                    continue;
                }
                
                if (game.player.getHealthPercent() <= 0.07d)
                {
                    isRecalling = true;
                    continue;
                }

                CastAndMove();

                AttackMove();

                CheckBuyItems();
            }
        }
        private void OnDie()
        {
            BuyItems();
        }
        private void OnSpawnJoin()
        {
            BuyItems();
            AllyIndex = game.getAllyIdToFollow();
            //TODO: Detect Next Lane Objective
            //TODO: Focus on Next Lane Objective (Tier 1 Turret > Tier 2 Turret > Tier 3 Turret > Inhib > Nexus Turret Left > Nexus Turret Right 
        }
        private void OnRevive()
        {
            AllyIndex = game.getAllyIdToFollow();
        }

        private void CastAndMove() // Replace this by Champion pattern script.
        {
            if (game.player.getManaPercent() <= healthManaPercentThresh)
                return;

            int Ripeti = 0;
            while (Ripeti < 2)
            {

                Ripeti = Ripeti + 1;

                game.moveCenterScreen();
                game.player.tryCastSpellOnTarget("Q"); 

                game.moveCenterScreen();
                game.player.tryCastSpellOnTarget("W");

                game.moveCenterScreen();
                game.player.tryCastSpellOnTarget("W");

                game.moveCenterScreen();
                game.player.tryCastSpellOnTarget("R");

            }
            Ripeti = 0;
            
        }

        private void AttackMove()
        {
            game.player.tryAttackMoveOnTarget();
            game.moveCenterScreen();
        }


        public override void End()
        {
            bot.executePattern("EndCoop");
            base.End();
        }
    }
}