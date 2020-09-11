﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NutritionManager.Application.Nutrients;
using NutritionManager.Application.Nutrients.Commands;
using NutritionManager.Application.Nutrients.Handlers;
using NutritionManager.Application.Nutrients.Queries;
using NutritionManager.Crosscutting.Templates;

namespace NutritionManager.Web.Api.Nutrients
{
    [ApiController]
    [Route("/api/v1/nutrient")]
    public class NutrientController : Controller
    {
        private readonly CreateNutrientHandler createNutrientHandler;

        private readonly DeleteNutrientByIdHandler deleteNutrientByIdHandler;

        private readonly GetNutrientDetailsHandler getNutrientDetailsHandler;

        private readonly ListNutrientsHandler listNutrientsHandler;

        public NutrientController(
            ListNutrientsHandler listNutrientsHandler,
            CreateNutrientHandler createNutrientHandler,
            DeleteNutrientByIdHandler deleteNutrientByIdHandler,
            GetNutrientDetailsHandler getNutrientDetailsHandler)
        {
            this.listNutrientsHandler = listNutrientsHandler;
            this.createNutrientHandler = createNutrientHandler;
            this.deleteNutrientByIdHandler = deleteNutrientByIdHandler;
            this.getNutrientDetailsHandler = getNutrientDetailsHandler;
        }

        [HttpGet("list-all")]
        public Task<NutrientsListViewModel> ListAll()
        {
            var query = new ListNutrients(ExpressionTemplates.AllExpression<Nutrient>());
            var handlerTask = this.listNutrientsHandler.HandleQueryAsync(query);

            return handlerTask
                .ContinueWith(antecedent => new NutrientsListViewModel(antecedent.Result));
        }
        
        [HttpGet]
        public Task<NutrientDetailsViewModel> Get([FromQuery] [Required] Guid nutrientId)
        {
            var query = new GetNutrientDetails(nutrientId);
            var handlerTask = this.getNutrientDetailsHandler.HandleQueryAsync(query);

            return handlerTask
                .ContinueWith(antecedent => new NutrientDetailsViewModel(antecedent.Result));
        }

        [HttpPost("add")]
        public Task Add([FromQuery] [Required] string title)
        {
            var command = new CreateNutrient(title);

            return this.createNutrientHandler.HandleCommandAsync(command);
        }

        [HttpPost("delete")]
        public Task Delete([FromQuery] [Required] Guid id)
        {
            var command = new DeleteNutrientById(id);

            return this.deleteNutrientByIdHandler.HandleCommandAsync(command);
        }
    }
}