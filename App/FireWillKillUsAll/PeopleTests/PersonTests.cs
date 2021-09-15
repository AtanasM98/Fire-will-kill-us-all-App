using Microsoft.VisualStudio.TestTools.UnitTesting;
using FireWillKillUsAll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FireWillKillUsAll.Tests
{
    [TestClass()]
    public class PersonTests
    {
        [TestMethod()]
        public void PersonTest()
        {
            //Constructor works
        }

        [TestMethod()]
        public void SetDestinationTest()
        {
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 0, 0);
            Tile jose = new Tile(null, 0, null, null, null, null, Color.Black, 1, 9);
            List<Tile> empty = new List<Tile>();
            Tile[,] another = new Tile[1, 1];
            
            Person p1 = new Person(huan, empty, empty, another); //init with huan
            p1.SetDestination(jose); //change it to jose
            Assert.AreNotEqual(huan, p1.EndLocation); //if after the change they are different, it works
        }

        [TestMethod()]
        public void MoveUpTest()
        {
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 0, 0);
            Tile jose = new Tile(null, 0, huan, null, null, null, Color.Black, 1, 9); //jose.up is huan
            List<Tile> empty = new List<Tile>();
            Tile[,] another = new Tile[1, 1];
            
            huan.walkable = true;
            huan.onFire = false;

            Person p1 = new Person(jose, empty, empty, another); //init with jose

            p1.MoveUp();

            Assert.AreEqual(huan, p1.CurrentLocation);
        }

        [TestMethod()]
        public void MoveDownTest()
        {
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 0, 0);
            Tile jose = new Tile(null, 0, null, huan, null, null, Color.Black, 1, 9); //jose.down is huan
            List<Tile> empty = new List<Tile>();
            Tile[,] another = new Tile[1, 1];

            huan.walkable = true;
            huan.onFire = false;

            Person p1 = new Person(jose, empty, empty, another); //init with jose

            p1.MoveDown();

            Assert.AreEqual(huan, p1.CurrentLocation);
        }

        [TestMethod()]
        public void MoveLeftTest()
        {
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 0, 0);
            Tile jose = new Tile(null, 0, huan, null, huan, null, Color.Black, 1, 9); //jose.left is huan
            List<Tile> empty = new List<Tile>();
            Tile[,] another = new Tile[1, 1];

            huan.walkable = true;
            huan.onFire = false;

            Person p1 = new Person(jose, empty, empty, another); //init with jose

            p1.MoveLeft();

            Assert.AreEqual(huan, p1.CurrentLocation);
        }

        [TestMethod()]
        public void MoveRightTest()
        {
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 0, 0);
            Tile jose = new Tile(null, 0, huan, null, null, huan, Color.Black, 1, 9); //jose.right is huan
            List<Tile> empty = new List<Tile>();
            Tile[,] another = new Tile[1, 1];

            huan.walkable = true;
            huan.onFire = false;

            Person p1 = new Person(jose, empty, empty, another); //init with jose

            p1.MoveRight();

            Assert.AreEqual(huan, p1.CurrentLocation);
        }

        [TestMethod()]
        public void RunForTheExitTest()
        {
            #region INITIAL SETUP
            //Okay, these will be hard to recreate
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 1, 0); huan.onFire = false; huan.walkable = true; //left
            Tile jose = new Tile(null, 0, null, null, null, null, Color.Black, 1, 2); jose.onFire = false; jose.walkable = true; //right
            Tile antoan = new Tile(null, 0, null, null, null, null, Color.Red, 0, 1); antoan.onFire = false; antoan.walkable = true; //up
            Tile esmeraldo = new Tile(null, 0, null, null, null, null, Color.Pink, 2, 2); esmeraldo.onFire = false; esmeraldo.walkable = true; //down
            Tile ricardo = new Tile(null, 0, antoan, esmeraldo, huan, jose, Color.Azure, 1, 1); ricardo.onFire = false; ricardo.walkable = true; //center, start

            antoan.down = ricardo;
            esmeraldo.up = ricardo;
            huan.right = ricardo;
            jose.left = ricardo;
            
            List<Tile> escape = new List<Tile>();
            List<Tile> ext = new List<Tile>();
            escape.Add(antoan); ext.Add(esmeraldo); //escape is up, extinguisher is down

            Tile[,] gridSim = new Tile[3,3];

            gridSim[0, 0] = null;
            gridSim[0, 1] = antoan;
            gridSim[0, 2] = null;
            gridSim[1, 0] = huan;
            gridSim[1, 1] = ricardo;
            gridSim[1, 2] = jose;
            gridSim[2, 0] = null;
            gridSim[2, 1] = esmeraldo;
            gridSim[2, 2] = null;

            foreach(Tile t in gridSim)
            {
                if(t != null)
                    t.AddNeighbours(); //add the neighbours to each tile
            }
            #endregion

            //TEST THE SAVE
            Person p1 = new Person(ricardo, escape, ext, gridSim); //init with ricardo
            p1.SWITCHPERSONALITYFORTESTING(PERSONALITY.PUSSY); //force the pussy personality
            p1.RunForTheExit(); //do the move
            p1.RunForTheExit(); //do the save
            Assert.AreEqual(antoan, p1.CurrentLocation);
            Assert.IsTrue(p1.IsSaved);
            Assert.IsFalse(p1.IsDead);

            //TEST THE DEATH
            p1 = new Person(esmeraldo, escape, ext, gridSim); //init with esmeraldo
            p1.SWITCHPERSONALITYFORTESTING(PERSONALITY.PUSSY); //force the pussy personality
            p1.RunForTheExit(); //do the move
            ricardo.onFire = true; //spawn the fire
            p1.RunForTheExit(); //do the kill
            Assert.AreEqual(ricardo, p1.CurrentLocation);
            Assert.IsTrue(p1.IsDead);
            Assert.IsFalse(p1.IsSaved);
            
            //TEST BECOMING SHAGGY WHEN EXIT IS BLOCKED
            p1 = new Person(esmeraldo, escape, ext, gridSim); //init with esmeraldo
            p1.SWITCHPERSONALITYFORTESTING(PERSONALITY.PUSSY); //force the pussy personality
            p1.RunForTheExit();
            Assert.AreEqual(PERSONALITY.SHAGGY_FROM_SCOOBY_DOO, p1.Personality);
            Assert.AreEqual(esmeraldo, p1.CurrentLocation);
        }

        [TestMethod()]
        public void ExitOrExtinguisherTest()
        {
            #region INITIAL SETUP
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 2, 0); huan.onFire = false; huan.walkable = true; //left
            Tile jose = new Tile(null, 0, null, null, null, null, Color.Black, 2, 2); jose.onFire = false; jose.walkable = true; //right
            Tile antoan = new Tile(null, 0, null, null, null, null, Color.Red, 1, 1); antoan.onFire = false; antoan.walkable = true; //up
            Tile esmeraldo = new Tile(null, 0, null, null, null, null, Color.Pink, 3, 2); esmeraldo.onFire = false; esmeraldo.walkable = true; //down
            Tile julio = new Tile(null, 0, esmeraldo, null, null, null, Color.FromArgb(255, 150, 0, 150), 4, 2); julio.onFire = false; julio.walkable = false; //the extinguisher tile
            Tile ricardo = new Tile(null, 0, antoan, esmeraldo, huan, jose, Color.Azure, 2, 1); ricardo.onFire = false; ricardo.walkable = true; //center, start
            Tile javier = new Tile(null, 0, null, antoan, null, null, Color.Purple, 0, 1); javier.onFire = false; javier.walkable = true; //escape

            antoan.down = ricardo;
            esmeraldo.up = ricardo;
            esmeraldo.down = julio;
            huan.right = ricardo;
            jose.left = ricardo;
            antoan.up = javier;

            List<Tile> escape = new List<Tile>();
            List<Tile> ext = new List<Tile>();
            escape.Add(javier); ext.Add(julio); //escape is up, extinguisher is down

            Tile[,] gridSim = new Tile[5, 3];

            gridSim[0, 0] = null;
            gridSim[0, 1] = javier;
            gridSim[0, 2] = null;
            gridSim[1, 0] = null;           // The pattern is like this
            gridSim[1, 1] = antoan;         //          X - exit
            gridSim[1, 2] = null;           //          X 
            gridSim[2, 0] = huan;           //        X X X
            gridSim[2, 1] = ricardo;        //          X 
            gridSim[2, 2] = jose;           //          X- wall with extinguisher
            gridSim[3, 0] = null;
            gridSim[3, 1] = esmeraldo;
            gridSim[3, 2] = null;
            gridSim[4, 0] = null;
            gridSim[4, 1] = julio;
            gridSim[4, 2] = null;

            foreach (Tile t in gridSim)
            {
                if (t != null)
                    t.AddNeighbours(); //add the neighbours to each tile
            }
            #endregion

            //TEST THE SAVE
            Person p1 = new Person(ricardo, escape, ext, gridSim); //init with ricardo
            p1.SWITCHPERSONALITYFORTESTING(PERSONALITY.SELFISH_PRICK); //force the selfish personality
            p1.ExitOrExtinguisher(); //do the move
            p1.ExitOrExtinguisher(); //do the move
            p1.ExitOrExtinguisher(); //do the save
            Assert.AreEqual(javier, p1.CurrentLocation);
            Assert.IsTrue(p1.IsSaved);
            Assert.IsFalse(p1.IsDead);
            
            //TEST SWITCH TO EXTINGUISHER
            escape.Add(javier); //add it back after the previous removal
            ext.Clear();
            ext.Add(julio);
            p1 = new Person(ricardo, escape, ext, gridSim); //init with ricardo
            p1.SWITCHPERSONALITYFORTESTING(PERSONALITY.SELFISH_PRICK); //force the selfish personality
            antoan.onFire = true;
            p1.ExitOrExtinguisher(); //do the switch
            p1.ExitOrExtinguisher(); //do the move
            Assert.AreEqual(esmeraldo, p1.CurrentLocation);

        }

        [TestMethod()]
        public void RunForExtinguisherTest()
        {
            #region INITIAL SETUP
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 1, 0); huan.onFire = false; huan.walkable = true; //left
            Tile jose = new Tile(null, 0, null, null, null, null, Color.Black, 1, 2); jose.onFire = false; jose.walkable = true; //right
            Tile antoan = new Tile(null, 0, null, null, null, null, Color.Red, 0, 1); antoan.onFire = false; antoan.walkable = true; //up
            Tile esmeraldo = new Tile(null, 0, null, null, null, null, Color.Pink, 2, 2); esmeraldo.onFire = false; esmeraldo.walkable = true; //down
            Tile julio = new Tile(null, 0, esmeraldo, null, null, null, Color.FromArgb(255, 150, 0, 150), 3, 2); julio.onFire = false; julio.walkable = false; //the extinguisher tile
            Tile ricardo = new Tile(null, 0, antoan, esmeraldo, huan, jose, Color.Azure, 1, 1); ricardo.onFire = false; ricardo.walkable = true; //center, start

            antoan.down = ricardo;
            esmeraldo.up = ricardo;
            esmeraldo.down = julio;
            huan.right = ricardo;
            jose.left = ricardo;

            List<Tile> escape = new List<Tile>();
            List<Tile> ext = new List<Tile>();
            escape.Add(antoan); ext.Add(julio); //escape is up, extinguisher is down

            Tile[,] gridSim = new Tile[4, 3];

            gridSim[0, 0] = null;           // The pattern is like this
            gridSim[0, 1] = antoan;         //          X - exit
            gridSim[0, 2] = null;           //        X X X
            gridSim[1, 0] = huan;           //          X
            gridSim[1, 1] = ricardo;        //          X - wall with extinguisher
            gridSim[1, 2] = jose;
            gridSim[2, 0] = null;
            gridSim[2, 1] = esmeraldo;
            gridSim[2, 2] = null;
            gridSim[3, 0] = null;
            gridSim[3, 1] = julio;
            gridSim[3, 2] = null;

            foreach (Tile t in gridSim)
            {
                if (t != null)
                    t.AddNeighbours(); //add the neighbours to each tile
            }

            #endregion

            //TEST THE EXTINGUISHER
            Person p1 = new Person(ricardo, escape, ext, gridSim); //init with ricardo
            p1.SWITCHPERSONALITYFORTESTING(PERSONALITY.HERO_OF_MANKIND); //force the hero personality
            p1.RunForExtinguisher(); //do the move
            p1.RunForExtinguisher(); //do the move
            Assert.AreEqual(esmeraldo, p1.CurrentLocation);
            Assert.IsFalse(p1.IsSaved);
            Assert.IsFalse(p1.IsDead);
            jose.SetFire(new Fire(jose));
            p1.RunForExtinguisher();
            Assert.AreEqual(ricardo, p1.CurrentLocation);
            p1.RunForExtinguisher();
            p1.RunForExtinguisher();
            Assert.IsFalse(jose.onFire);

            //TEST THE DEATH
            ext.Add(julio); //add back the extinguisher
            p1 = new Person(ricardo, escape, ext, gridSim); //init with ricardo
            p1.SWITCHPERSONALITYFORTESTING(PERSONALITY.HERO_OF_MANKIND); //force the hero personality
            p1.RunForExtinguisher(); //do the move
            esmeraldo.onFire = true; //spawn the fire
            p1.RunForExtinguisher(); //do the kill
            Assert.AreEqual(esmeraldo, p1.CurrentLocation);
            Assert.IsTrue(p1.IsDead);
            Assert.IsFalse(p1.IsSaved);


            //TEST PERSONALITY SWITCH WHEN NO EXTINGUISHERS
            ext.Clear();
            ext.Add(julio);
            escape.Add(antoan); //add it twice if it is removed if the person is created as a pussy
            p1 = new Person(ricardo, escape, ext, gridSim); //init with esmeraldo
            p1.SWITCHPERSONALITYFORTESTING(PERSONALITY.HERO_OF_MANKIND); //force the hero personality
            p1.RunForExtinguisher();
            Assert.AreEqual(antoan, p1.EndLocation);
            Assert.AreNotEqual(esmeraldo, p1.CurrentLocation);
            Assert.AreEqual(antoan, p1.CurrentLocation);
        }

        [TestMethod()]
        public void BeShaggyTest()
        {
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 1, 0); huan.onFire = false; huan.walkable = true; //left
            Tile jose = new Tile(null, 0, null, null, null, null, Color.Black, 1, 2); jose.onFire = false; jose.walkable = true; //right
            Tile antoan = new Tile(null, 0, null, null, null, null, Color.Red, 0, 1); antoan.onFire = false; antoan.walkable = true; //up
            Tile esmeraldo = new Tile(null, 0, null, null, null, null, Color.Pink, 2, 2); esmeraldo.onFire = false; esmeraldo.walkable = true; //down
            Tile ricardo = new Tile(null, 0, antoan, esmeraldo, huan, jose, Color.Azure, 1, 1); ricardo.onFire = false; ricardo.walkable = true; //center, start

            antoan.down = ricardo;
            esmeraldo.up = ricardo;
            huan.right = ricardo;
            jose.left = ricardo;

            List<Tile> escape = new List<Tile>();
            List<Tile> ext = new List<Tile>();

            Tile[,] gridSim = new Tile[3, 3];

            gridSim[0, 0] = null;
            gridSim[0, 1] = antoan;
            gridSim[0, 2] = null;
            gridSim[1, 0] = huan;
            gridSim[1, 1] = ricardo;
            gridSim[1, 2] = jose;
            gridSim[2, 0] = null;
            gridSim[2, 1] = esmeraldo;
            gridSim[2, 2] = null;

            foreach (Tile t in gridSim)
            {
                if (t != null)
                    t.AddNeighbours(); //add the neighbours to each tile
            }

            Person p1 = new Person(ricardo, escape, ext, gridSim);
            p1.SWITCHPERSONALITYFORTESTING(PERSONALITY.SHAGGY_FROM_SCOOBY_DOO); //force a shaggy
            p1.BeShaggy(); //Move up
            Assert.AreEqual(antoan, p1.CurrentLocation); //should be on upper tile
            p1.BeShaggy(); //move right
            Assert.AreEqual(antoan, p1.CurrentLocation); //should stay here, because right is null
            p1.BeShaggy(); //move down
            Assert.AreEqual(ricardo, p1.CurrentLocation); //should be back to start, below the up tile
            p1.BeShaggy(); //move left
            Assert.AreEqual(huan, p1.CurrentLocation); //should be at the left of the start tile
        }

        [TestMethod()]
        public void DieTest()
        {
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 1, 0);
            List<Tile> empty = new List<Tile>();
            Tile[,] gr = new Tile[1, 1];
            Person p1 = new Person(huan, empty, empty, gr);
            p1.Die();
            Assert.IsFalse(p1.IsSaved);
            Assert.IsTrue(p1.IsDead);
        }
    }
}