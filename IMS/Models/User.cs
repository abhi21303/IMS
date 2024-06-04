using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    [NotMapped]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Order> Orders { get; set; }

    public virtual Role Role { get; set; } = null!;
}
