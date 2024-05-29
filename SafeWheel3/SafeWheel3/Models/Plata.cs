namespace SafeWheel3.Models
{
    public class Plata
    {
       public int Id { get; set; }
       public string UserID {  get; set; } 

       public int CommentID {  get; set; }  
       public Comment Comment { get; set; } 

    }
}
