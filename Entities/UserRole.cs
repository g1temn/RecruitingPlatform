using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitingPlatform.Entities;

[Table("user_roles")]
public class UserRole : IdentityRole<int>
{

}
