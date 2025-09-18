using GestionApi.Data;
using GestionApi.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GestionApi.Controllers
{
    [ApiController] //Decirle a Swagger que esto es un controlador de API
    [Route("api/[controller]")]
    [Authorize] // Requerir autenticación JWT para todos los endpoints
    public class TaskController : Controller
    {
        #region CONTEXTO

        private readonly GestionDBContext _context;  //Usar el Contexto
        private readonly IHubContext<TasksHub>? _hub;

        public TaskController(GestionDBContext context, IHubContext<TasksHub>? hub = null)
        {
            _context = context;
            _hub = hub;
        }

        #endregion


        #region GET

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Func<Task<List<Models.Tareas>>> getAllTasks = async () => await _context.Task.ToListAsync();
            var tasks = await getAllTasks();
            return Ok(tasks);
        }
        #endregion

        #region GETBYID

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Func<int, Task<Models.Tareas?>> findTaskById = async (taskId) => await _context.Task.FindAsync(taskId);
            var task = await findTaskById(id);
            if (task == null)
            {
                return NotFound("No se encontró el registro con el ID especificado");
            }
            return Ok(task);
        }
        #endregion

        #region POST


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Models.Tareas task)
        {
            if (task == null)
            {
                return BadRequest("No puede estar nulo");
            }
            Action<Models.Tareas> addTask = t => _context.Task.Add(t);
            Func<Task> saveChanges = async () => await _context.SaveChangesAsync();

            addTask(task);
            await saveChanges();
            if (_hub != null)
            {
                await _hub.Clients.All.SendAsync("taskCreated", task);
            }
            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }
        #endregion

        #region EDIT

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Models.Tareas task)
        {
            if (id != task.Id)
            {
                return BadRequest("El ID no coincide");
            }
            Action setModified = () => _context.Entry(task).State = EntityState.Modified;
            Func<Task> saveChanges = async () => await _context.SaveChangesAsync();
            try
            {
                setModified();
                await saveChanges();
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
            if (_hub != null)
            {
                await _hub.Clients.All.SendAsync("taskUpdated", task);
            }
            return NoContent();
        }
        #endregion

        #region DELETE

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Func<int, Task<Models.Tareas?>> findTaskById = async (taskId) => await _context.Task.FindAsync(taskId);
            var task = await findTaskById(id);
            if (task == null)
            {
                return NotFound("No se encontró la tarea con el ID especificado");
            }
            Action<Models.Tareas> removeTask = t => _context.Task.Remove(t);
            Func<Task> saveChanges = async () => await _context.SaveChangesAsync();

            removeTask(task);
            await saveChanges();
            if (_hub != null)
            {
                await _hub.Clients.All.SendAsync("taskDeleted", id);
            }
            return NoContent();
        }

        #endregion



    }
}
