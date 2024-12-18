using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Project_Game.Entities
{
    public class CachedAnimation
    {
        public List<Image> Frames { get; set; }
        public List<string> FrameNames { get; set; }

        public CachedAnimation(List<Image> frames, List<string> frameNames)
        {
            Frames = frames;
            FrameNames = frameNames;
        }
    }

}
