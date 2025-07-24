using GestionApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionApi.Controllers
{
    [ApiController] //Decirle a Swagger que esto es un controlador de API
    [Route("Task")]
    public class TaskController : Controller
    {
        #region CONTEXTO

        private readonly GestionDBContext _context;  //Usar el Contexto

        public TaskController(GestionDBContext context)
        {
            _context = context;
        }

        #endregion


        #region GET

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            var tasks = await _context.Task.ToListAsync();
            return Ok(tasks);
        }
        #endregion

        #region GETBYID

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound("No se encontró el registro con el ID especificado");
            }
            return Ok(task);
        }
        #endregion

        #region POST


        [HttpPost]
        [Route("Post")]
        public async Task<IActionResult> Post([FromBody] Models.Tareas task)
        {
            if (task == null)
            {
                return BadRequest("No puede estar nulo");
            }
            _context.Task.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }
        #endregion

        #region EDIT

        [HttpPut]
        [Route("Put/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Models.Tareas task)
        {
            if (id != task.Id)
            {
                return BadRequest("El ID no coincide");
            }
            _context.Entry(task).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Task.Any(e => e.Id == id))
                {
                    return NotFound("No se encontró la tarea con el ID especificado");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        #endregion

        #region DELETE

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound("No se encontró la tarea con el ID especificado");
            }
            _context.Task.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        #endregion



    }
}
