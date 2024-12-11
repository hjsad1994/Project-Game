// File: Chicken.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Project_Game.Entities
{
    public class Chicken : GameObject
    {
        private AnimationManager movementAnimation;
        private AnimationManager idleAnimation;
        private AnimationManager sleepAnimation;
        private bool isSleeping = false;
        private int movementSpeed = 2;
        private string currentState = "Idle"; // "Idle", "MoveLeft", "MoveRight", "Sleep"

        private int minX, maxX; // Define the movement range for the chicken
        private bool isMovingRight = true; // Flag to determine direction of movement

        public bool IsAttacking => false; // Chicken không tấn công
        public bool ShouldRemove => false; // Chicken không bị loại bỏ

        // Constructor with minX and maxX parameters to define movement range
        public Chicken(string name, int startX, int startY, int minX, int maxX)
            : base(startX, startY, 32, 32, name)
        {
            this.minX = minX;
            this.maxX = maxX;
            movementAnimation = new AnimationManager(frameRate: 10);
            idleAnimation = new AnimationManager(frameRate: 10);
            sleepAnimation = new AnimationManager(frameRate: 10);

            // Load frames for the states
            LoadMovementFrames();
            LoadIdleFrames();
            LoadSleepFrames();
        }

        private void LoadMovementFrames()
        {
            movementAnimation.LoadFrames(Path.Combine("Assets", "Chicken", "Chicken_Movement", "MoveLeft"));
            movementAnimation.LoadFrames(Path.Combine("Assets", "Chicken", "Chicken_Movement", "MoveRight"));
        }

        private void LoadIdleFrames()
        {
            idleAnimation.LoadFrames(Path.Combine("Assets", "Chicken", "Chicken_Idle", "IdleLeft"));
            idleAnimation.LoadFrames(Path.Combine("Assets", "Chicken", "Chicken_Idle", "IdleRight"));
            idleAnimation.LoadFrames(Path.Combine("Assets", "Chicken", "Chicken_Idle", "EatLeft"));
            idleAnimation.LoadFrames(Path.Combine("Assets", "Chicken", "Chicken_Idle", "EatRight"));
        }

        private void LoadSleepFrames()
        {
            sleepAnimation.LoadFrames(Path.Combine("Assets", "Chicken", "Chicken_Sleep", "SleepLeft"));
            sleepAnimation.LoadFrames(Path.Combine("Assets", "Chicken", "Chicken_Sleep", "SleepRight"));
        }

        public void SetSleeping(bool sleeping)
        {
            isSleeping = sleeping;
            currentState = sleeping ? "Sleep" : "Idle";
            if (isSleeping)
            {
                sleepAnimation.ResetAnimation();
            }
            else
            {
                idleAnimation.ResetAnimation();
            }
        }

        public void Update()
        {
            if (isSleeping)
            {
                sleepAnimation.UpdateAnimation();
                return;
            }

            // Random chance to change direction or start eating
            Random rand = new Random();
            int action = rand.Next(0, 100);

            if (action < 2) // 2% chance to change direction
            {
                currentState = currentState == "MoveLeft" ? "MoveRight" : "MoveLeft";
            }
            else if (action < 10) // 8% chance to switch to eating state
            {
                currentState = currentState == "IdleLeft" ? "EatLeft" : "EatRight";
            }

            // Chicken movement logic
            if (currentState == "MoveLeft" && X > minX)
            {
                X -= movementSpeed;
                movementAnimation.UpdateAnimation();
            }
            else if (currentState == "MoveRight" && X < maxX)
            {
                X += movementSpeed;
                movementAnimation.UpdateAnimation();
            }
            else if (currentState == "EatLeft" || currentState == "EatRight")
            {
                idleAnimation.UpdateAnimation(); // Use idle animation for eating
            }
            else
            {
                idleAnimation.UpdateAnimation(); // Default to idle
            }

            // Ensure chicken stays within the boundaries of the screen (800x600)
            if (X < 0) X = 0;
            if (X + Width > 800) X = 800 - Width;
        }

        public Image GetCurrentFrame()
        {
            if (isSleeping)
            {
                return sleepAnimation.GetCurrentFrame();
            }

            if (currentState == "MoveLeft" || currentState == "MoveRight")
            {
                return movementAnimation.GetCurrentFrame();
            }
            else if (currentState == "IdleLeft" || currentState == "IdleRight")
            {
                return idleAnimation.GetCurrentFrame();
            }
            else // Idle
            {
                return idleAnimation.GetCurrentFrame();  // Default to Idle
            }
        }

        public override void TakeDamage(int damage)
        {
            // Chicken không nhận sát thương
        }
    }
}
