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

namespace HRManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;


        public RolesController(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
          if (_context.Roles == null)
          {
              return NotFound();
          }
            var roles = await _context.Roles.ToListAsync();
            var rolesDto = new List<RoleDto>();
            foreach (var role in roles)
            {
                rolesDto.Add(_mapper.Map<Role,RoleDto>(role));
            }
            return Ok(rolesDto);
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
          if (_context.Roles == null)
          {
              return NotFound();
          }
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return _mapper.Map<Role, RoleDto>(role);
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, RoleDto role)
        {
            if (id != role.Id)
            {
                return BadRequest();
            }

            _context.Entry(_mapper.Map<RoleDto, Role>(role)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoleDto>> PostRole(RoleToCreateDto role)
        {
             if (_context.Roles == null)
             {
                 return Problem("Entity set 'Context.Roles'  is null.");
             }
            var roleToAdd = _mapper.Map<RoleToCreateDto, Role>(role);
            _context.Roles.Add(roleToAdd);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRole", new { id = roleToAdd.Id }, _mapper.Map<Role, RoleDto>(roleToAdd));
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            if (_context.Roles == null)
            {
                return NotFound();
            }
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return (_context.Roles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
