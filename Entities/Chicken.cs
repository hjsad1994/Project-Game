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

        public Chicken(string name, int startX, int startY, int minX, int maxX)
            : base(startX, startY, 20, 20, name)
        {
            this.minX = minX;
            this.maxX = maxX;
            movementAnimationLeft = new AnimationManager(frameRate: 20);
            movementAnimationRight = new AnimationManager(frameRate: 20);
            idleAnimationLeft = new AnimationManager(frameRate: 20);
            idleAnimationRight = new AnimationManager(frameRate: 20);
            sleepAnimationLeft = new AnimationManager(frameRate: 15);
            sleepAnimationRight = new AnimationManager(frameRate: 15);

            LoadMovementFrames();
            LoadIdleFrames();
            LoadSleepFrames();

            // Random hướng ban đầu
            if (rand.Next(2) == 0)
                currentDirection = "Right";
            else
                currentDirection = "Left";

            // Random thời gian idle ban đầu
            int initialIdleDuration = rand.Next(1000, 3000);
            idleTime = Environment.TickCount + initialIdleDuration;

            // Lên lịch Sleep lần đầu
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
            // 5-10s random
            int sleepWait = rand.Next(5000, 10001);
            nextSleepTime = Environment.TickCount + sleepWait;
            Console.WriteLine($"{Name} will sleep at {nextSleepTime - Environment.TickCount}ms from now.");
        }

        public void Update(Player player)
        {
            // Kiểm tra trạng thái Sleep
            if (currentState == "Sleep")
            {
                // Đang ngủ
                if (Environment.TickCount >= sleepEndTime)
                {
                    // Hết thời gian ngủ
                    Console.WriteLine($"{Name} woke up from sleep!");
                    SwitchToIdle(currentDirection);
                    ScheduleNextSleep();
                }
                else
                {
                    // Đang ngủ, update sleep animation
                    if (currentDirection == "Left")
                        sleepAnimationLeft.UpdateAnimation(loop: true);
                    else
                        sleepAnimationRight.UpdateAnimation(loop: true);
                }
                return; // return để không chạy các logic Idle/Move
            }

            // Kiểm tra xem đã đến lúc Sleep chưa (chỉ sleep khi Idle)
            if (currentState == "Idle" && Environment.TickCount >= nextSleepTime)
            {
                Console.WriteLine($"{Name} is going to sleep now!");
                currentState = "Sleep";
                // Reset animation sleep
                sleepAnimationLeft.ResetAnimation();
                sleepAnimationRight.ResetAnimation();
                sleepEndTime = Environment.TickCount + sleepDuration;
                return;
            }

            if (currentState == "Idle")
            {
                if (Environment.TickCount >= idleTime)
                {
                    if (currentDirection == "Right")
                    {
                        currentState = "MoveRight";
                        movementAnimationRight.ResetAnimation();
                        movedDistance = 0;
                        Console.WriteLine($"{Name} starts moving right.");
                    }
                    else
                    {
                        currentState = "MoveLeft";
                        movementAnimationLeft.ResetAnimation();
                        movedDistance = 0;
                        Console.WriteLine($"{Name} starts moving left.");
                    }
                    isIdle = false;
                }
                else
                {
                    if (currentDirection == "Right")
                        idleAnimationRight.UpdateAnimation();
                    else
                        idleAnimationLeft.UpdateAnimation();
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

            if (currentState == "Idle")
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
