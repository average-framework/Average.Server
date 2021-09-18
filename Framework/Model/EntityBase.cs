using System.ComponentModel.DataAnnotations;

namespace Average.Server.Framework.Model
{
    public abstract class EntityBase
    {
        [Key]
        public long Id { get; set; }
    }
}
