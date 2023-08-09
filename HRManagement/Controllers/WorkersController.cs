using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagement.Models;
using HR_Management.Models;
using HRManagement.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace HRManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public WorkersController(Context context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration; 
        }

        // GET: api/Workers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkerDto>>> GetWorkers()
        {
          if (_context.Workers == null)
          {
              return NotFound();
          }
            List<WorkerDto> workers = new();
            _context.Workers.Include(x => x.Roles).ToList().ForEach(x => workers.Add(_mapper.Map<Worker, WorkerDto>(x)));
            return Ok(workers);
        }
        // GET: api/Workers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkerDto>> GetWorker(int id)
        {
          if (_context.Workers == null)
          {
              return NotFound();
          }
            var workers = await _context.Workers.Include(x => x.Roles).ToListAsync();
            var worker = workers.FirstOrDefault(x => x.Id == id);

            if (worker == null)
            {
                return NotFound();
            }

            return _mapper.Map<Worker, WorkerDto>(worker);
        }

        // PUT: api/Workers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorker(int id, WorkerToUpdateDto worker)
        {
            if (id != worker.Id)
            {
                return BadRequest();
            }
            var workerToUpdate = _context.Workers.Include(x => x.Roles).FirstOrDefault(x => x.Id == id);

            if (workerToUpdate is null)
            {
                return NotFound();
            }
            List<Role> roles = new();
            foreach (var item in worker.RolesId)
            {
                var role = await _context.Roles.FindAsync(item);
                if (role is null)
                {
                    return BadRequest("Invalid Rol Id");
                }
                roles.Add(role);
            }
            List<SalaryRecord> salaryRecords = new();
            salaryRecords.Add(new SalaryRecord
            {
                Salary = worker.WorkingStartSalary,
                IncreaseSalaryDate = worker.WorkingStartDate
            });
            _context.SalaryRecords.ToList().RemoveAll(x => x.Worker.Id == id);
            workerToUpdate.Name = worker.Name;
            workerToUpdate.Address = worker.Address;
            workerToUpdate.Phone = worker.Phone;
            workerToUpdate.Email = worker.Email;
            workerToUpdate.LastName = worker.LastName;
            workerToUpdate.WorkingStartDate = worker.WorkingStartDate;
            workerToUpdate.Roles.Clear();
            workerToUpdate.Roles = roles;
            workerToUpdate.SalaryRecord = salaryRecords;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
                throw;
            }

            return NoContent();
        }

        // POST: api/Workers
        [HttpPost]
        public async Task<ActionResult<Worker>> PostWorker(WorkerToCreateDto worker)
        {
          if (_context.Workers == null)
          {
              return Problem("Entity set 'Context.Workers'  is null.");
          }
            List<Role> roles = new();
            foreach (var item in worker.RolesId)
            {
                var role = await _context.Roles.FindAsync(item);
                if (role is  null)
                {
                    return BadRequest("Invalid Rol Id");
                }
                roles.Add(role);
            }
            List<SalaryRecord> salaryRecords = new();
            salaryRecords.Add(new SalaryRecord 
            { 
                Salary = worker.WorkingStartSalary, 
                IncreaseSalaryDate = worker.WorkingStartDate 
            });
            var workerTest = _mapper.Map<WorkerToCreateDto, Worker>(worker);
            workerTest.Roles = roles;
            workerTest.SalaryRecord = salaryRecords;
            _context.Workers.Add(workerTest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorker", new { id = workerTest.Id },_mapper.Map<Worker, WorkerDto>(workerTest));
        }

        // DELETE: api/Workers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorker(int id)
        {
            if (_context.Workers == null)
            {
                return NotFound();
            }
            var worker = await _context.Workers.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }

            _context.Workers.Remove(worker);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // PUT: api/CalculateSalaryIncrease/5
        [HttpPut("CalculateSalaryIncrease/{id}")]
        public async Task<IActionResult> CalculateSalaryIncreaseAsync(int id)
        {
            var worker = _context.Workers.Include(x => x.Roles).Include(x => x.SalaryRecord).FirstOrDefault(x => x.Id == id);

            if (worker is null)
            {
                return NotFound();
            }
            var lastSalaryIncreaseDate = worker.SalaryRecord.Last().IncreaseSalaryDate;
            var currentDate = DateTime.Now;
            while (currentDate > worker.SalaryRecord.Last().IncreaseSalaryDate.AddMonths(3))
            {
                double increase = 0;
                var lastSalaryIncrease = worker.SalaryRecord.Last().Salary;
                foreach (var rol in worker.Roles)
                {
                    switch (rol.Name)
                    {
                        case "Worker":
                            increase+= 3*lastSalaryIncrease/100;
                            break;
                        case "Specialist":
                            increase += 8 * lastSalaryIncrease / 100;
                            break;
                        case "Manager":
                            increase += 12 * lastSalaryIncrease / 100;
                            break;
                    }
                }
                worker.SalaryRecord.Add(new SalaryRecord()
                {
                    Salary = lastSalaryIncrease + increase,
                    IncreaseSalaryDate = worker.SalaryRecord.Last().IncreaseSalaryDate.AddMonths(3)
                });
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;
            }

            return NoContent();
        }
        // GET: api/Workers
        [HttpGet("WorkersSP2")]
        public async Task<List<WorkerDto>> GetWorkersInfoProcessed()
        {
            string json = GetInfo("dbo.sp_SELECT_WORKERS_WITH_ROLES", null);
            var data = JsonConvert.DeserializeObject<List<dynamic>>(json);

            var groups = data.GroupBy(d => d.Id);
            var workers = groups.Select(g =>
            {
                var first = g.First();
                WorkerDto worker = new WorkerDto
                {
                    Id = first.Id,
                    Name = first.Name,
                    Email = first.Email,
                    LastName = first.LastName,
                    Phone = first.Phone,
                    Address = first.Address,
                    WorkingStartDate = DateOnly.FromDateTime(DateTime.Parse(first.WorkingStartDate.ToString()))
                };

                worker.Roles = g.Select(d => new RoleDto { Id = d.RoleId, Name = d.RoleName }).ToList();

                return worker;
            }).ToList();

            return workers;
        }
        // GET: api/Workers/RecordSalary/5
        [HttpGet("RecordSalary/{id}")]
        public async Task<ActionResult<IEnumerable<SalaryRecordrDto>>> GetRecordSalaryInfoProcessed(int id)
        {
            var worker = await _context.Workers.FirstOrDefaultAsync(x => x.Id == id);
            if (worker is null)
            {
                return NotFound();
            }
            await CalculateSalaryIncreaseAsync(id);
            string json = GetInfo("dbo.sp_Salary_Record_Per_User", id);
            var data = JsonConvert.DeserializeObject<List<dynamic>>(json);

            var salaryRecords = data.Select(g =>
            {
                SalaryRecordrDto record = new SalaryRecordrDto
                {
                    Salary = g.Salary,
                    IncreaseSalaryDate = DateTime.Parse(g.IncreaseSalaryDate.ToString())
                };
                return record;
            }).ToList();

            return salaryRecords;
        }
        private string GetInfo(string sp_name, int? id)
        {
            var conStr = _configuration.GetConnectionString("DefaultConnection");
            DataTable dataTable = new DataTable();

            try  //
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                using (SqlCommand command = new SqlCommand(sp_name, connection))
                {
                    if (id is not null)
                    { 
                        command.Parameters.AddWithValue("@WorkerId", id); 
                    }
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message);
            }
            string json = JsonConvert.SerializeObject(dataTable);

            return json;
        }
        private bool WorkerExists(int id)
        {
            return (_context.Workers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
