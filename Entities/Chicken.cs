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

        // Thời gian ngủ cố định: 5 giây
        private int sleepDuration = 5000;
        private int sleepEndTime = 0;

        // Thời điểm vào Sleep tiếp theo (5-10s)
        private int nextSleepTime = 0;

        // PreIdle tương tự Idle nhưng rất ngắn
        private bool isPreIdle = false;
        private int preIdleDuration = 500; // 0.5s
        private bool goingToSleep = false; // Chỉ thị xem sau PreIdle có Sleep không

        public Chicken(string name, int startX, int startY, int minX, int maxX)
            : base(startX, startY, 16, 25, name)
        {
            this.minX = minX;
            this.maxX = maxX;
            movementAnimationLeft = new AnimationManager(frameRate: 25);
            movementAnimationRight = new AnimationManager(frameRate: 25);
            idleAnimationLeft = new AnimationManager(frameRate: 25);
            idleAnimationRight = new AnimationManager(frameRate: 25);
            sleepAnimationLeft = new AnimationManager(frameRate: 30);
            sleepAnimationRight = new AnimationManager(frameRate: 30); ///

            LoadMovementFrames();
            LoadIdleFrames();
            LoadSleepFrames();

            // Random hướng ban đầu
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
                Console.WriteLine("Loaded SleepLeft frames, count: " + sleepAnimationLeft.GetFrameCount());
            }
            else
            {
                Console.WriteLine("SleepLeft frames folder not found: " + sleepLeftFrames);
            }

            if (Directory.Exists(sleepRightFrames))
            {
                sleepAnimationRight.LoadFrames(sleepRightFrames);
                Console.WriteLine("Loaded SleepRight frames, count: " + sleepAnimationRight.GetFrameCount());
            }
            else
            {
                Console.WriteLine("SleepRight frames folder not found: " + sleepRightFrames);
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
                if (Environment.TickCount >= sleepEndTime)
                {
                    Console.WriteLine($"{Name} woke up from sleep!");
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

            if (currentState == "Idle")
            {
                // Idle logic
                if (Environment.TickCount >= idleTime)
                {
                    // Thay vì vào Move thẳng, vào PreIdle
                    // Kiểm tra xem đã đến lúc Sleep chưa
                    goingToSleep = false;
                    if (Environment.TickCount >= nextSleepTime)
                    {
                        goingToSleep = true; // Sau PreIdle sẽ sleep
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
                // PreIdle giống Idle ngắn
                if (currentDirection == "Right")
                    idleAnimationRight.UpdateAnimation(loop: true);
                else
                    idleAnimationLeft.UpdateAnimation(loop: true);

                if (Environment.TickCount >= idleTime) // idleTime bây giờ đại diện cho thời gian PreIdle
                {
                    // Kết thúc PreIdle
                    if (goingToSleep)
                    {
                        // Vào Sleep
                        Console.WriteLine($"{Name} is going to sleep now!");
                        currentState = "Sleep";
                        sleepAnimationLeft.ResetAnimation();
                        sleepAnimationRight.ResetAnimation();
                        sleepEndTime = Environment.TickCount + sleepDuration;
                    }
                    else
                    {
                        // Chọn MoveRight hoặc MoveLeft tùy vào currentDirection
                        if (currentDirection == "Right")
                        {
                            currentState = "MoveRight";
                            movementAnimationRight.ResetAnimation();
                            movedDistance = 0;
                            Console.WriteLine($"{Name} starts moving right after PreIdle.");
                        }
                        else
                        {
                            currentState = "MoveLeft";
                            movementAnimationLeft.ResetAnimation();
                            movedDistance = 0;
                            Console.WriteLine($"{Name} starts moving left after PreIdle.");
                        }
                        isIdle = false;
                    }
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

        private void SetState(string state)
        {
            currentState = state;
            if (state == "PreIdle")
            {
                isIdle = false;
                isPreIdle = true;
                // PreIdle ngắn 500ms
                int idleTimeShort = preIdleDuration;
                idleTime = Environment.TickCount + idleTimeShort;
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
                // PreIdle dùng chung animation Idle
                if (currentDirection == "Left")
                    return idleAnimationLeft.GetCurrentFrame();
                else
                    return idleAnimationRight.GetCurrentFrame();
            }
            else if (currentState == "Idle")
            {
                if (currentDirection == "Left")
                {
                    return idleAnimationLeft.GetCurrentFrame();
                }
                else
                {
                    return idleAnimationRight.GetCurrentFrame();
                }
            }
            else if (currentState == "MoveLeft")
            {
                return movementAnimationLeft.GetCurrentFrame();
            }
            else if (currentState == "MoveRight")
            {
                return movementAnimationRight.GetCurrentFrame();
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
