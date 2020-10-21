using System;

namespace SimpleReactionMachine
{
    class Tester
    {
        private static IController controller;
        private static IGui gui;
        private static string displayText;
        private static int randomNumber;
        private static int passed = 0;

        static void Main(string[] args)
        {
            // run enhanced test
            EnhancedTest();
            Console.WriteLine("\n=====================================\nSummary: {0} tests passed out of 35", passed);
            Console.ReadKey();
        }

        private static void EnhancedTest()
        {
            //Construct a ReactionController
            controller = new EnhancedReactionController();
            gui = new DummyGui();

            //Connect them to each other
            gui.Connect(controller);
            controller.Connect(gui, new RndGenerator());

            //Reset the components()
            gui.Init();

            //Test the EnhancedReactionController
            //GameOn Phase
            DoReset('A', controller, "Insert coin");
            
            DoGoStop('B', controller, "Insert coin");    //Pressing Go/Stop buton has no effect            
            DoTicks('C', controller, 1, "Insert coin");  //Doing Tick has no effect

            //coinInserted : GameReady Phase
            DoInsertCoin('D', controller, "Press GO!");           
            DoInsertCoin('E', controller, "Press GO!");  //Inserting coin has no effect
           
            //Waiting for 10 seconds in GameReadyPhase resets the controller back to GameOnPhase
            DoTicks('F', controller, 999, "Press GO!");  
            DoTicks('G', controller, 1, "Insert coin");   //Now Display should be 'Insert coin'
           
            randomNumber = 117;
            DoInsertCoin('H', controller, "Press GO!");
            //Pressed Go/Stop: GameWaiting Phase
            DoGoStop('I', controller, "Wait...");
            DoInsertCoin('J', controller, "Wait...");    //Inserting coin has no effect
            
            //Go/Stop pressed in GameWaitingPhase is considered cheating, it takes the game back to GameOnPhase
            DoGoStop('K', controller, "Insert coin");

            //Inserting coin and pressing go/stop : GameOn -> GameReady -> GameWaiting
            DoInsertCoin('L', controller, "Press GO!");
            DoGoStop('M', controller, "Wait...");   //GameWaiting Phase
            
            //After the random wait time, GameRunningPhase is called and it displays '0.00'
            DoTicks('N', controller, randomNumber, "0.00");
            DoInsertCoin('O', controller, "0.00");    //Inserting coin has no effect

            //Go/Stop records the total time and proceeds to GameOverPhase : Display should be same same as total time(1.50 sec)
            DoTicks('P', controller, 150, "1.50");           
            DoGoStop('Q', controller, "1.50");  //GameOver Phase

            //Ticking for 3 seconds, takes the game to GameWaiting Phase 
            DoTicks('R', controller, 300, "Wait...");

            //GameRunningPhase
            DoTicks('S', controller, randomNumber, "0.00");

            randomNumber = 1;

            //Running 3 games
            //First game
            DoReset('T', controller, "Insert coin");    //Reset the game
            DoInsertCoin('U', controller, "Press GO!"); //GameReady Phase
            DoGoStop('V', controller, "Wait...");       //GamWaiting Phase
            DoTicks('X', controller, randomNumber, "0.00"); //GameRunning Phase
            DoTicks('Y', controller, 50, "0.50");       
            DoGoStop('Z', controller, "0.50");          //GameOver Phase
            DoTicks('a', controller, 300, "Wait...");   //GameWaiting Phase

            //Second game
            DoTicks('b', controller, randomNumber, "0.00"); //GameRunning Phase
            DoTicks('c', controller, 30, "0.30");
            DoGoStop('d', controller, "0.30");          //GameOver Phase
            DoTicks('e', controller, 300, "Wait...");   //GameWaiting Phase

            //Third game: after plaing three games, the game should advance to GameOutcome Phase and display the average time
            DoTicks('f', controller, randomNumber, "0.00"); //GameRunning Phase
            DoTicks('g', controller, 20, "0.20");
            DoGoStop('h', controller, "0.20");          //GameOver Phase
            DoGoStop('i', controller, "Average: 0.33");   //GameOutcome Phase

            
            //After 5 seconds, the controller is set to GameOn Phase and should display 'Insert coin'
            DoTicks('j', controller, 500, "Insert coin");

                     
        }

        private static void DoReset(char ch, IController controller, string msg)
        {
            try
            {
                controller.Init();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoGoStop(char ch, IController controller, string msg)
        {
            try
            {
                controller.GoStopPressed();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoInsertCoin(char ch, IController controller, string msg)
        {
            try
            {
                controller.CoinInserted();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoTicks(char ch, IController controller, int n, string msg)
        {
            try
            {
                for (int t = 0; t < n; t++) controller.Tick();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

       

        private static void GetMessage(char ch, string msg)
        {
            if (msg.ToLower() == displayText.ToLower())
            {
                Console.WriteLine("test {0}: passed successfully", ch);
                passed++;
            }
            else
                Console.WriteLine("test {0}: failed with message ( expected {1} | received {2})", ch, msg, displayText);
        }

        private class DummyGui : IGui
        {

            private IController controller;

            public void Connect(IController controller)
            {
                this.controller = controller;
            }

            public void Init()
            {
                displayText = "?reset?";
            }

            public void SetDisplay(string msg)
            {
                displayText = msg;
            }
        }

        private class RndGenerator : IRandom
        {
            public int GetRandom(int from, int to)
            {
                return randomNumber;
            }
        }

    }

}
