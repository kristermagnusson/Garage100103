﻿using GarageV3.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GarageV3.Core.ViewModels
{
#nullable disable
    public class ParkCarViewModel
    {
        public IEnumerable<VehicleType> VehicleTypes { get; set; } = new List<VehicleType>();

        public VehicleType VehicleType { get; set; } = new();

        public SelectListItem[] SelectListItems { get; set; }

        public Vehicle Vehicle { get; set; }

        public Owner Owner { get; set; }

        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Regnr")]
        [RegularExpression(@"^[a-zA-Z-0-9 ]+$", ErrorMessage = "Endast siffror och text är tillåtet")]
        public string RegNr { get; set; }

        [Required]
        [StringLength(15)]
        [Display(Name = "Färg")]
        public string Color { get; set; }

        [Range(0, 6)]
        [Display(Name = "Antal Hjul")]
        public int Wheels { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Märke")]
        public string Brand { get; set; }

        [Required]
        [Display(Name = "Modell")]
        public string Model { get; set; }


        [Display(Name = "Ankomsttid")]
        public DateTime ArrivalTime { get; set; } = DateTime.MinValue;
    }
}