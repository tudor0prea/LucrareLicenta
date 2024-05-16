using System.ComponentModel.DataAnnotations.Schema;

namespace SafeWheel3.Models
{
    public class AnuntBookmark
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? AnuntId { get; set; }
        public int? BookmarkId { get; set; }
        public virtual Anunt? Anunt { get; set; }
        public virtual Bookmark? Bookmark { get; set; }
        public DateTime BookmarkDate { get; set; }
    }
}
