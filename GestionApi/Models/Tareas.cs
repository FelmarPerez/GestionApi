using System.ComponentModel.DataAnnotations;

namespace GestionApi.Models
{
    public class Tareas //Creacion de modelo generico.
    {
        [Required]
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string AdditionalData { get; set; } //Informacion adicional que es de tipo Generico.

        //public T AdditionalData { get; set; } //Informacion adicional que es de tipo Generico.
    }

}
