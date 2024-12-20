using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Project_Game.Entities
{
    public class Kapybara : GameObject
    {
        private Dictionary<string, AnimationManager> animations = new Dictionary<string, AnimationManager>();
        private string currentState = "Idle";
        private static Random rand = new Random();

        private List<string> possibleStates = new List<string>
        {
            "Bubbles",
            "Dive",
            "Emerge",
            "LookAround",
            "LookAround_submerged"
        };

        private int stateEndTime = 0;
        private bool isIdle = true;

        private int minX, maxX;

        // Thêm trạng thái PreIdle
        // PreIdle: trạng thái trung gian, ngắn, sau Idle trước khi sang trạng thái mới
        private bool isPreIdle = false;
        private int preIdleDuration = 500; // 0.5 giây
        private string nextStateAfterPreIdle = null; // Lưu trữ trạng thái sẽ chuyển sau PreIdle

        public bool IsAttacking => false;
        public bool ShouldRemove => false;

        public Kapybara(string name, int startX, int startY, int minX, int maxX, int width = 60, int height = 50)
            : base(startX, startY, width, height, name)
        {
            this.minX = minX;
            this.maxX = maxX;

            LoadAnimations();

            // Bắt đầu ở Idle
            SetState("Idle", loop: true);
        }

        private void LoadAnimations()
        {
            LoadAnimation("Idle", "Kapybara_Idle_cuts", frameRate: 15, loop: true);
            LoadAnimation("PreIdle", "Kapybara_Idle_cuts", frameRate: 15, loop: true); // sử dụng chung Idle cho PreIdle
            LoadAnimation("Bubbles", "Kapybara_Bubbles_cuts", frameRate: 15, loop: false);
            LoadAnimation("Dive", "Kapybara_Dive_cuts", frameRate: 15, loop: false);
            LoadAnimation("Emerge", "Kapybara_Emerge_cuts", frameRate: 15, loop: false);
            LoadAnimation("LookAround", "Kapybara_LookAround_cuts", frameRate: 15, loop: false);
            LoadAnimation("LookAround_submerged", "Kapybara_LookAround_submerged_cuts", frameRate: 15, loop: false);
        }

        private void LoadAnimation(string stateName, string folderName, int frameRate = 15, bool loop = false)
        {
            string path = Path.Combine("Assets", "Kapybara", folderName);
            var anim = new AnimationManager(frameRate);
            anim.LoadFrames(path);
            animations[stateName] = anim;
        }

        private void SetState(string state, bool loop = true)
        {
            currentState = state;
            animations[currentState].ResetAnimation();

            if (state == "Idle")
            {
                isIdle = true;
                isPreIdle = false;
                nextStateAfterPreIdle = null;
                int idleTime = rand.Next(2000, 4001);
                stateEndTime = Environment.TickCount + idleTime;
            }
            else if (state == "PreIdle")
            {
                // PreIdle là một trạng thái idle ngắn
                isIdle = false;
                isPreIdle = true;
                nextStateAfterPreIdle = null; // Sẽ đặt sau
                stateEndTime = Environment.TickCount + preIdleDuration;
            }
            else
            {
                isIdle = false;
                isPreIdle = false;
                nextStateAfterPreIdle = null;
            }
        }

        public void Update()
        {
            var anim = animations[currentState];

            if (currentState == "Idle")
            {
                anim.UpdateAnimation(loop: true);

                if (Environment.TickCount >= stateEndTime)
                {
                    // Hết thời gian idle, chuyển sang PreIdle trước khi sang trạng thái mới
                    SetState("PreIdle", loop: true);
                    // Chọn nextState ở đây
                    string ns = possibleStates[rand.Next(possibleStates.Count)];
                    nextStateAfterPreIdle = ns;
                }
            }
            else if (currentState == "PreIdle")
            {
                // PreIdle giống Idle nhưng rất ngắn
                anim.UpdateAnimation(loop: true);
                if (Environment.TickCount >= stateEndTime)
                {
                    // Hết thời gian PreIdle, chuyển sang nextStateAfterPreIdle
                    if (!string.IsNullOrEmpty(nextStateAfterPreIdle))
                    {
                        SetState(nextStateAfterPreIdle, loop: false);
                    }
                    else
                    {
                        // Nếu vì lý do nào đó nextStateAfterPreIdle null, quay lại Idle
                        SetState("Idle", loop: true);
                    }
                }
            }
            else
            {
                // Update animation state hiện tại (loop = false)
                anim.UpdateAnimation(loop: false);

                if (anim.IsComplete())
                {
                    // Animation state này hoàn tất, quay lại Idle
                    SetState("Idle", loop: true);
                }
                // Không di chuyển, Kapybara chỉ đứng yên chạy animation
            }
        }

        public Image GetCurrentFrame()
        {
            if (!animations.ContainsKey(currentState)) return null;
            return animations[currentState].GetCurrentFrame();
        }

        public override void TakeDamage(int damage)
        {
            // Kapybara không nhận sát thương
        }

        // Thêm phương thức Draw để vẽ Kapybara
        public void Draw(Graphics g)
        {
            Image currentFrame = GetCurrentFrame();
            if (currentFrame != null)
            {
                g.DrawImage(currentFrame, X, Y, Width, Height);
                Console.WriteLine($"[Debug] Vẽ Kapybara '{Name}' tại ({X}, {Y}) với kích thước ({Width}x{Height}).");
            }
            else
            {
                Console.WriteLine($"[Error] Kapybara '{Name}' tại ({X}, {Y}) không có hình ảnh để vẽ.");
            }
        }
    }
}
