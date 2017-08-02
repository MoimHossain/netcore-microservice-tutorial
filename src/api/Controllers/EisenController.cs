using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NCoreWebApp.Dtos;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NCoreWebApp.Controllers
{
    [Route("api/[controller]")]
    public class EisenController : Controller
    {
        // GET: api/values
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
    }
}
