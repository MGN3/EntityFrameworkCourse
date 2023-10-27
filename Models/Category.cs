using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCourse.Models {
	public class Category {

		//[Key]
		public Guid CategoryId { get; set; }

		//[Required] 
		//[StringLength(150)]
		public string? Name { get; set; }
		//[StringLength(500)]
		public string? Description { get; set; }

		public virtual ICollection<Task> Tasks { get; set;}
	}
}
	