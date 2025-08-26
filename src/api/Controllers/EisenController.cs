using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NCoreWebApp.Dtos;
using NCoreWebApp.Sagas.Services;
using NCoreWebApp.Sagas.Messages;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NCoreWebApp.Controllers
{
    [Route("api/[controller]")]
    public class EisenController : Controller
    {
        private readonly ISagaService _sagaService;

        public EisenController(ISagaService sagaService)
        {
            _sagaService = sagaService;
        }
        // GET: api/eisen
        [HttpGet]
        public IEnumerable<EisDto> Get()
        {
            return new EisDto[] {
                new EisDto {
                    ID = "1",
                    Title = "Hyper-loop",
                    ModelImage = "http://www.stonybrook.edu/happenings/wp-content/uploads/Hyperloop-interior.jpg",
                    Description= "Build hyper-loop tunnel from Amsterdam to Madrid",
                    Phase = "Initial ontwerp"
                },
                new EisDto{
                    ID= "1.1.0",
                    Title= "Space Elevator",
                    ModelImage= "https://fm.cnbc.com/applications/cnbc.com/resources/img/editorial/2015/08/18/102928436-thoth_2.1910x1000.jpg",
                    Description= "Build a Space Elevator",
                    Phase= "Schets ontwerp"
                },
                new EisDto{
                    ID= "2.0.0",
                    Title= "Space colonization",
                    ModelImage= "http://toucharcade.com/wp-content/uploads/2016/10/Space-Haven-Pre-Alpha.jpg",
                    Description= "Build Space colonization in Mars",
                    Phase= "Schets ontwerp"
                }
            };
        }

        // POST: api/eisen
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EisDto eisenData)
        {
            if (eisenData == null)
            {
                return BadRequest("Eisen data is required");
            }

            try
            {
                // Start SAGA for Eisen creation
                var sagaId = await _sagaService.StartEisenCreationSaga(eisenData);
                
                return Ok(new { 
                    SagaId = sagaId, 
                    Message = "Eisen creation SAGA started successfully",
                    EisenData = eisenData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // GET: api/eisen/saga/{sagaId}/status
        [HttpGet("saga/{sagaId}/status")]
        public async Task<IActionResult> GetSagaStatus(Guid sagaId)
        {
            try
            {
                var status = await _sagaService.GetSagaStatus(sagaId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // GET: api/eisen/sagas/count
        [HttpGet("sagas/count")]
        public async Task<IActionResult> GetActiveSagasCount()
        {
            try
            {
                var count = await _sagaService.GetActiveSagasCount();
                return Ok(new { ActiveSagasCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
