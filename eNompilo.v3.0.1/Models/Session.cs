using eNompilo.v3._0._1.Models.Counselling;
using eNompilo.v3._0._1.Models.SystemUsers;
using eNompilo.v3._0._1.Models.Vaccination;
using eNompilo.v3._0._1.Models.Family_Planning;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eNompilo.v3._0._1.Models
{
    public class Session
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        [Required]
        [ForeignKey("Practitioner")]
        public int PractitionerId { get; set; }
        public virtual Practitioner? Practitioner { get; set; }

        [ForeignKey("VaccinationAppointment")]
        public int? VaccinationAppointmentId { get; set; }
        public virtual VaccinationAppointment? VaccinationAppointment { get; set; }

        [ForeignKey("CounsellingAppointment")]
        public int? CounsellingAppointmentId { get; set; }
        public virtual CounsellingAppointment? CounsellingAppointment { get; set; }

        [ForeignKey("GeneralAppointment")]
        public int? GeneralAppointmentId { get; set; }
        public virtual GeneralAppointment? GeneralAppointment { get; set; }

        [ForeignKey("FamilyPlanningAppointment")]
        public int? FamilyPlanningAppointmentId { get; set; }
        public virtual FamilyPlanningAppointment? FamilyPlanningAppointment { get; set; }

        [Required]
        [Display(Name = "Did patient arrive for session?")]
        public bool Arrived { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: HH:mm}")]
        [DataType(DataType.Time)]
        [Display(Name = "Patient arrival time")]
        public DateTime? ArrivalTime { get; set; }

        [Display(Name = "Practitioner's Notes")]
        public string? PractitionerNotes { get; set; }

        [Display(Name = "Does patient indicate any signs for a potential condition?")]
        public bool? ConditionIndication { get; set; }

        [Display(Name = "What potential condition may the patient have?")]
        public string? PotentialCondition { get; set; }

        [Display(Name = "Does the patient indicate to be a danger to themselves or others?")]
        public bool? IsADanger { get; set; }

        [Display(Name = "Does the patient indicate to be in danger (in an active abusive relationship?)")]
        public bool? IsAbused { get; set; } //if yes, mark patient account as abused and patient folder, and enable booking link

        [DisplayName("Prescription Medication")]
        public string? Prescription { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: HH:mm}")]
        [DataType(DataType.Time)]
        [Display(Name = "Session end time")]
        public DateTime? EndTime { get; set; }

        public bool Archived { get; set; } = false;

    }
}
