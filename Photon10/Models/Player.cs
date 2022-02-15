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
        public int id { get; set; }

        [Required]
        [Column(TypeName = "char")]
        [StringLength(30)]
        public string first_name { get; set; }

        [Required]
        [Column(TypeName = "char")]
        [StringLength(30)]
        public string last_name { get; set; }

        [Required]
        [Column(TypeName = "char")]
        [StringLength(30)]
        public string codename { get; set; }
    }
}
