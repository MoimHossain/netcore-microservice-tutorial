using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NCoreWebApp.Dtos;
using NCoreWebApp.Hubs;

namespace NCoreWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EisenController : ControllerBase
    {
        private readonly IHubContext<EisenHub> _hubContext;
        private static List<EisDto> _eisenItems = new List<EisDto>
        {
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

        public EisenController(IHubContext<EisenHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // GET: api/eisen
        [HttpGet]
        public IEnumerable<EisDto> Get()
        {
            return _eisenItems;
        }

        // GET: api/eisen/5
        [HttpGet("{id}")]
        public ActionResult<EisDto> Get(string id)
        {
            var item = _eisenItems.FirstOrDefault(x => x.ID == id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        // POST: api/eisen
        [HttpPost]
        public async Task<ActionResult<EisDto>> Post([FromBody] EisDto eisDto)
        {
            if (eisDto == null)
            {
                return BadRequest();
            }

            eisDto.ID = Guid.NewGuid().ToString();
            _eisenItems.Add(eisDto);

            // Notify all connected clients about the new item
            await _hubContext.Clients.All.SendAsync("EisenAdded", eisDto);

            return CreatedAtAction(nameof(Get), new { id = eisDto.ID }, eisDto);
        }

        // PUT: api/eisen/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] EisDto eisDto)
        {
            if (eisDto == null || id != eisDto.ID)
            {
                return BadRequest();
            }

            var existingItem = _eisenItems.FirstOrDefault(x => x.ID == id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Title = eisDto.Title;
            existingItem.Description = eisDto.Description;
            existingItem.Phase = eisDto.Phase;
            existingItem.ModelImage = eisDto.ModelImage;

            // Notify all connected clients about the update
            await _hubContext.Clients.All.SendAsync("EisenUpdated", existingItem);

            return NoContent();
        }

        // DELETE: api/eisen/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var item = _eisenItems.FirstOrDefault(x => x.ID == id);
            if (item == null)
            {
                return NotFound();
            }

            _eisenItems.Remove(item);

            // Notify all connected clients about the deletion
            await _hubContext.Clients.All.SendAsync("EisenDeleted", id);

            return NoContent();
        }
    }
}
