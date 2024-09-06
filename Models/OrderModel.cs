using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApplication.Models
{
    public enum OrderStatus
    {
        [Description("Принят")]
        Принят,
        [Description("Обработка")]
        Обработка,
        [Description("Выполнен")]
        Выполнен,
        [Description("Отменен")]
        Отменен,
    }
    public class OrderModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Идентификатор спецификации")]
        public int SpecificationModelId { get; set; }

        [Required]
        [Display(Name = "Дата оформления заказа")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "Дата выполнения заказа")]
        public DateTime OrderDateDeadline { get; set; }
        [Required]
        [Display(Name = "Статус заказа")]
        public OrderStatus Status { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Заказчик")]
        public string ClientName { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.####}", ApplyFormatInEditMode = true)]
        [Display(Name = "Количество")]
        public decimal Quantity { get; set; }

        //[Required]
        //[StringLength(20)]
        //[Display(Name = "Единица измерения")]
        //public string? MeasureUnit { get; set; }

        [NotMapped]
        [Display(Name = "Наименование Товара")]
        public virtual SpecificationModel? SpecificationModel { get; set; }
    }
}
