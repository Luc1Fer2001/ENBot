using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.LevelSystem
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
