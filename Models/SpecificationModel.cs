using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class SpecificationModel
    {
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(Parent))]
        [Display(Name = "Идентификатор родительской позиции")]
        public int? ParentId { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [StringLength(200)]
        [Display(Name = "Описание")]
        public string? Description { get; set; }
        [Required]
        [Display(Name = "Необходимое количество")]
        [DisplayFormat(DataFormatString = "{0:0.####}")]
        public decimal CountForParent { get; set; }
        [StringLength(20)]
        [Display(Name = "Единица измерения")]
        public string? MeasureUnit { get; set; }

        [NotMapped]
        [Display(Name = "Родительская позиция")]
        public virtual SpecificationModel? Parent { get; set; }
        [NotMapped]
        [Display(Name = "Необходимые позиции")]
        public virtual List<SpecificationModel>? Childrens { get; set; }

        [NotMapped]
        public virtual string SearchString { get => Id.ToString() + Name.ToLower() + Description?.ToLower(); }

        [NotMapped]
        public virtual decimal? AllCount { get; set; } = 1;

        [NotMapped]
        [ValidateNever]
        public virtual List<WarehouseRecordModel> WarehouseRecords { get; set;}

        [NotMapped]
        [ValidateNever]
        public virtual List<OrderModel> Orders { get; set; }


    }
}
