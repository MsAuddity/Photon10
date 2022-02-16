using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Photon10.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "char")]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Required]
        [Column(TypeName = "char")]
        [StringLength(30)]
        public string LastName { get; set; }

        [Required]
        [Column(TypeName = "char")]
        [StringLength(30)]
        public string Codename { get; set; }
    }
}
