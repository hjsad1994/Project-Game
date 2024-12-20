using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Game.Entities
{
    public interface IRemovable
    {
        bool ShouldRemove { get; set; }
    }
}
