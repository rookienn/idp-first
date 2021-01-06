using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace is4FirstDemo.IdentityUserStore
{
    public class IdentityUserClaim1
    {
        [Key]
        public string ClaimId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public string UserSubjectId { get; set; }
        [ForeignKey("UserSubjectId")]
        public virtual IdentityUser1 IdentityUser { get; set; }
    }
}
