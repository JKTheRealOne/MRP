using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication.Models
{
    public enum WarehouseRecordType
    {
        [Description("Поступление")]
        Поступление,
        [Description("Убывание")]
        Убывание,
        //[Description("Создано")]
        //Created,
        //[Description("Использовано")]
        //Used
    }
    public class WarehouseRecordModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Идентификатор спецификации")]
        public int SpecificationModelId { get; set; }
        [Required]
        [Display(Name = "Тип записи")]
        public WarehouseRecordType RecordType { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:0.####}", ApplyFormatInEditMode = true)]
        [Display(Name = "Количество")]
        public decimal Quantity { get; set; }

        //[Required]
        //[StringLength(20)]
        //[Display(Name = "Единица измерения")]
        //public string? MeasureUnit { get; set; }

        [NotMapped]
        [Display(Name = "Спецификация")]
        public virtual SpecificationModel? SpecificationModel { get; set; }

    }
}
