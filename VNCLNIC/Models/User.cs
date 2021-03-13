using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using VNCLNIC.Common.Enums;

namespace VNCLNIC.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true)]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ResetPasswordCode { get; set; }
        public string ConfirmPasswordCode { get; set; }
        public bool IsRemember { get; set; }
        public bool IsDoctor { get; set; }
        public UserType UserType { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; }
    }
}