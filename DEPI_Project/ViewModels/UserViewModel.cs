using System.Collections.Generic;
namespace DEPI_Project.ViewModels
{

		public class UserViewModel
		{
			public string Id { get; set; }
			public string UserType { get; set; }
			public string Email { get; set; }
			public IEnumerable<string> Roles { get; set; }
		}
	
}
