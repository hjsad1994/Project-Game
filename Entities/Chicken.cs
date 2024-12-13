using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Project_Game.Entities
{
    public class Chicken : GameObject
    {
        private AnimationManager movementAnimationLeft;
        private AnimationManager movementAnimationRight;
        private AnimationManager idleAnimationLeft;
        private AnimationManager idleAnimationRight;
        private AnimationManager sleepAnimationLeft;
        private AnimationManager sleepAnimationRight;
        private AnimationManager eatAnimationLeft;
        private AnimationManager eatAnimationRight;
        private AnimationManager resetAnimationLeft;
        private AnimationManager resetAnimationRight; // Thêm reset animations

        private int movementSpeed = 2;
        private string currentState = "Idle";
        private string currentDirection = "Right";
        private int minX, maxX;
        private int movedDistance = 0;
        private int moveLimit = 50;
        private bool isIdle = true;
        private int idleTime = 0;

        public bool IsAttacking => false;
        public bool ShouldRemove => false;

        private static Random rand = new Random();

        private int sleepDuration = 5000;
        private int sleepEndTime = 0;
        private int nextSleepTime = 0;

        private bool isPreIdle = false;
        private int preIdleDuration = 500; // 0.5s
        private bool goingToSleep = false;
        private bool goingToEat = false;

        private int originalWidth;
        private int originalHeight;

        private string nextFinalState = null; // Trạng thái cuối cùng sau Reset

        public Chicken(string name, int startX, int startY, int minX, int maxX)
            : base(startX, startY, 16, 25, name)
        {
            this.minX = minX;
            this.maxX = maxX;

            originalWidth = Width;   //16
            originalHeight = Height; //25

            movementAnimationLeft = new AnimationManager(frameRate: 25);
            movementAnimationRight = new AnimationManager(frameRate: 25);
            idleAnimationLeft = new AnimationManager(frameRate: 25);
            idleAnimationRight = new AnimationManager(frameRate: 25);
            sleepAnimationLeft = new AnimationManager(frameRate: 25);
            sleepAnimationRight = new AnimationManager(frameRate: 25);
            eatAnimationLeft = new AnimationManager(frameRate: 10);
            eatAnimationRight = new AnimationManager(frameRate: 10);

            resetAnimationLeft = new AnimationManager(frameRate: 25);
            resetAnimationRight = new AnimationManager(frameRate: 25);

            LoadMovementFrames();
            LoadIdleFrames();
            LoadSleepFrames();
            LoadEatFrames();
            LoadResetFrames(); // Load reset animations

            if (rand.Next(2) == 0)
                currentDirection = "Right";
            else
                currentDirection = "Left";

            int initialIdleDuration = rand.Next(1000, 3000);
            idleTime = Environment.TickCount + initialIdleDuration;

            ScheduleNextSleep();
        }

        private void LoadMovementFrames()
        {
            var moveLeftFrames = Path.Combine("Assets", "Chicken", "Chicken_Movement", "MoveLeft");
            var moveRightFrames = Path.Combine("Assets", "Chicken", "Chicken_Movement", "MoveRight");

            if (Directory.Exists(moveLeftFrames))
            {
                movementAnimationLeft.LoadFrames(moveLeftFrames);
                Console.WriteLine("Loaded MoveLeft frames");
            }

            if (Directory.Exists(moveRightFrames))
            {
                movementAnimationRight.LoadFrames(moveRightFrames);
                Console.WriteLine("Loaded MoveRight frames");
            }
        }

        private void LoadIdleFrames()
        {
            var idleLeftFrames = Path.Combine("Assets", "Chicken", "Chicken_Idle", "IdleLeft");
            var idleRightFrames = Path.Combine("Assets", "Chicken", "Chicken_Idle", "IdleRight");

            if (Directory.Exists(idleLeftFrames))
            {
                idleAnimationLeft.LoadFrames(idleLeftFrames);
                Console.WriteLine("Loaded IdleLeft frames");
            }

            if (Directory.Exists(idleRightFrames))
            {
                idleAnimationRight.LoadFrames(idleRightFrames);
                Console.WriteLine("Loaded IdleRight frames");
            }
        }

        private void LoadSleepFrames()
        {
            var sleepLeftFrames = Path.Combine("Assets", "Chicken", "Chicken_Sleep", "SleepLeft");
            var sleepRightFrames = Path.Combine("Assets", "Chicken", "Chicken_Sleep", "SleepRight");

            if (Directory.Exists(sleepLeftFrames))
            {
                sleepAnimationLeft.LoadFrames(sleepLeftFrames);
                Console.WriteLine("Loaded SleepLeft frames");
            }

            if (Directory.Exists(sleepRightFrames))
            {
                sleepAnimationRight.LoadFrames(sleepRightFrames);
                Console.WriteLine("Loaded SleepRight frames");
            }
        }

        private void LoadEatFrames()
        {
            var eatLeftFrames = Path.Combine("Assets", "Chicken", "Chicken_Eat", "EatLeft");
            var eatRightFrames = Path.Combine("Assets", "Chicken", "Chicken_Eat", "EatRight");

            if (Directory.Exists(eatLeftFrames))
            {
                eatAnimationLeft.LoadFrames(eatLeftFrames);
                Console.WriteLine("Loaded EatLeft frames");
            }

            if (Directory.Exists(eatRightFrames))
            {
                eatAnimationRight.LoadFrames(eatRightFrames);
                Console.WriteLine("Loaded EatRight frames");
            }
        }

        private void LoadResetFrames()
        {
            var resetLeftFrames = Path.Combine("Assets", "Chicken", "Chicken_Reset", "ResetLeft");
            var resetRightFrames = Path.Combine("Assets", "Chicken", "Chicken_Reset", "ResetRight");

            if (Directory.Exists(resetLeftFrames))
            {
                resetAnimationLeft.LoadFrames(resetLeftFrames);
                Console.WriteLine("Loaded ResetLeft frames");
            }

            if (Directory.Exists(resetRightFrames))
            {
                resetAnimationRight.LoadFrames(resetRightFrames);
                Console.WriteLine("Loaded ResetRight frames");
            }
        }


        private void ScheduleNextSleep()
        {
            int sleepWait = rand.Next(5000, 10001);
            nextSleepTime = Environment.TickCount + sleepWait;
            Console.WriteLine($"{Name} will sleep at {nextSleepTime - Environment.TickCount}ms from now.");
        }

        public void Update(Player player)
        {
            if (currentState == "Sleep")
            {
                this.Width = 16;
                this.Height = 40;

                if (Environment.TickCount >= sleepEndTime)
                {
                    Console.WriteLine($"{Name} woke up from sleep!");
                    this.Width = originalWidth;
                    this.Height = originalHeight;
                    SwitchToIdle(currentDirection);
                    ScheduleNextSleep();
                }
                else
                {
                    if (currentDirection == "Left")
                        sleepAnimationLeft.UpdateAnimation(loop: true);
                    else
                        sleepAnimationRight.UpdateAnimation(loop: true);
                }
                return;
            }
            else if (currentState == "Eat")
            {
                this.Width = originalWidth;
                this.Height = originalHeight;

                if (currentDirection == "Left")
                    eatAnimationLeft.UpdateAnimation(loop: false);
                else
                    eatAnimationRight.UpdateAnimation(loop: false);

                if ((currentDirection == "Left" && eatAnimationLeft.IsComplete()) ||
                    (currentDirection == "Right" && eatAnimationRight.IsComplete()))
                {
                    Console.WriteLine($"{Name} finished eating.");
                    SwitchToIdle(currentDirection);
                }
                return;
            }
            else if (currentState == "Reset")
            {
                // Reset state: chạy animation ResetLeft/ResetRight một lần
                this.Width = originalWidth;
                this.Height = originalHeight;

                if (currentDirection == "Left")
                {
                    resetAnimationLeft.UpdateAnimation(loop: false);
                    if (resetAnimationLeft.IsComplete())
                    {
                        OnResetComplete();
                    }
                }
                else
                {
                    resetAnimationRight.UpdateAnimation(loop: false);
                    if (resetAnimationRight.IsComplete())
                    {
                        OnResetComplete();
                    }
                }
                return;
            }
            else
            {
                this.Width = originalWidth;
                this.Height = originalHeight;
            }

            if (currentState == "Idle")
            {
                if (Environment.TickCount >= idleTime)
                {
                    goingToSleep = false;
                    goingToEat = false;

                    if (Environment.TickCount >= nextSleepTime)
                    {
                        goingToSleep = true;
                    }
                    else
                    {
                        if (rand.Next(2) == 0)
                            goingToEat = true;
                    }

                    SetState("PreIdle");
                }
                else
                {
                    if (currentDirection == "Right")
                        idleAnimationRight.UpdateAnimation();
                    else
                        idleAnimationLeft.UpdateAnimation();
                }
            }
            else if (currentState == "PreIdle")
            {
                if (currentDirection == "Right")
                    idleAnimationRight.UpdateAnimation(loop: true);
                else
                    idleAnimationLeft.UpdateAnimation(loop: true);

                if (Environment.TickCount >= idleTime)
                {
                    // Kết thúc PreIdle, xác định final action
                    // Trước khi vào final action (Sleep/Eat/Move), vào Reset
                    if (goingToSleep)
                    {
                        nextFinalState = "Sleep";
                    }
                    else if (goingToEat)
                    {
                        nextFinalState = "Eat";
                    }
                    else
                    {
                        if (currentDirection == "Right")
                            nextFinalState = "MoveRight";
                        else
                            nextFinalState = "MoveLeft";
                    }

                    // Chuyển sang Reset state
                    SetState("Reset");
                }
            }
            else if (currentState == "MoveRight" && !isIdle)
            {
                int newX = X + movementSpeed;
                if (!WillCollideWithPlayer(newX, Y, player))
                {
                    X = newX;
                    movedDistance += movementSpeed;
                    if (movedDistance >= moveLimit)
                    {
                        SwitchToIdle("Left");
                        movementAnimationRight.ResetAnimation();
                    }
                    movementAnimationRight.UpdateAnimation();
                }
                else
                {
                    SwitchToIdle("Left");
                    movementAnimationRight.ResetAnimation();
                    Console.WriteLine($"{Name} collided with player while moving right, switching to idle.");
                }
            }
            else if (currentState == "MoveLeft" && !isIdle)
            {
                int newX = X - movementSpeed;
                if (!WillCollideWithPlayer(newX, Y, player))
                {
                    X = newX;
                    movedDistance += movementSpeed;
                    if (movedDistance >= moveLimit)
                    {
                        SwitchToIdle("Right");
                        movementAnimationLeft.ResetAnimation();
                    }
                    movementAnimationLeft.UpdateAnimation();
                }
                else
                {
                    SwitchToIdle("Right");
                    movementAnimationLeft.ResetAnimation();
                    Console.WriteLine($"{Name} collided with player while moving left, switching to idle.");
                }
            }

            if (X < 0) X = 0;
            if (X + Width > 800) X = 800 - Width;
        }

        private void OnResetComplete()
        {
            // Reset animation xong, chuyển sang nextFinalState
            Console.WriteLine($"{Name} finished reset, going to {nextFinalState}.");

            if (nextFinalState == "Sleep")
            {
                currentState = "Sleep";
                sleepAnimationLeft.ResetAnimation();
                sleepAnimationRight.ResetAnimation();
                sleepEndTime = Environment.TickCount + sleepDuration;
            }
            else if (nextFinalState == "Eat")
            {
                currentState = "Eat";
                if (currentDirection == "Left")
                    eatAnimationLeft.ResetAnimation();
                else
                    eatAnimationRight.ResetAnimation();
            }
            else if (nextFinalState == "MoveRight")
            {
                currentState = "MoveRight";
                movementAnimationRight.ResetAnimation();
                movedDistance = 0;
            }
            else if (nextFinalState == "MoveLeft")
            {
                currentState = "MoveLeft";
                movementAnimationLeft.ResetAnimation();
                movedDistance = 0;
            }

            nextFinalState = null; // reset final state
        }

        private void SetState(string state)
        {
            currentState = state;
            if (state == "PreIdle")
            {
                isIdle = false;
                isPreIdle = true;
                int idleTimeShort = preIdleDuration;
                idleTime = Environment.TickCount + idleTimeShort;
            }
            else if (state == "Reset")
            {
                // Reset animation
                // Chạy animation ResetLeft/ResetRight từ đầu
                if (currentDirection == "Left")
                    resetAnimationLeft.ResetAnimation();
                else
                    resetAnimationRight.ResetAnimation();
            }
        }

        private bool WillCollideWithPlayer(int newX, int newY, Player player)
        {
            Rectangle chickenRect = new Rectangle(newX, newY, Width, Height);
            Rectangle playerRect = new Rectangle(player.playerX, player.playerY, player.playerWidth, player.playerHeight);

            return chickenRect.IntersectsWith(playerRect);
        }

        private void SwitchToIdle(string newDirection)
        {
            currentState = "Idle";
            isIdle = true;
            isPreIdle = false;
            int nextIdleDuration = rand.Next(1000, 3000);
            idleTime = Environment.TickCount + nextIdleDuration;
            movedDistance = 0;
            currentDirection = newDirection;
            Console.WriteLine($"{Name} switched to idle facing {currentDirection}, will idle for {nextIdleDuration}ms.");
        }

        public Image GetCurrentFrame()
        {
            if (currentState == "Sleep")
            {
                if (currentDirection == "Left")
                    return sleepAnimationLeft.GetCurrentFrame();
                else
                    return sleepAnimationRight.GetCurrentFrame();
            }
            else if (currentState == "PreIdle")
            {
                if (currentDirection == "Left")
                    return idleAnimationLeft.GetCurrentFrame();
                else
                    return idleAnimationRight.GetCurrentFrame();
            }
            else if (currentState == "Idle")
            {
                if (currentDirection == "Left")
                    return idleAnimationLeft.GetCurrentFrame();
                else
                    return idleAnimationRight.GetCurrentFrame();
            }
            else if (currentState == "MoveLeft")
            {
                return movementAnimationLeft.GetCurrentFrame();
            }
            else if (currentState == "MoveRight")
            {
                return movementAnimationRight.GetCurrentFrame();
            }
            else if (currentState == "Eat")
            {
                if (currentDirection == "Left")
                    return eatAnimationLeft.GetCurrentFrame();
                else
                    return eatAnimationRight.GetCurrentFrame();
            }
            else if (currentState == "Reset")
            {
                if (currentDirection == "Left")
                    return resetAnimationLeft.GetCurrentFrame();
                else
                    return resetAnimationRight.GetCurrentFrame();
            }

            return idleAnimationRight.GetCurrentFrame();
        }

        public override void TakeDamage(int damage)
        {
            // Chicken không nhận sát thương
        }

        public void Move(Player player)
        {
            Update(player);
        }
    }
}
