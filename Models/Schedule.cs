using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Video_conference_app.Models
{
    public class Schedule
    {
        //PRIMARY KEY
        [Required]
        public int Id { get; set; }

        //Title of meeting, "Meeting" by default 
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = "Meeting";

        //Start time of meeting
        [Required]
        public DateTime StartTime { get; set; }
        
        //FOREIGN KEY 
        [Required]
        public int OrganizerId { get; set; } // Id of the user who creates the meeting

        [ForeignKey("OrganizerId")] // Specify the foreign key name
        public User Organizer { get; set; } // Navigation property
    }
}
