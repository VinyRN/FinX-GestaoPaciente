using FinX.Api.Mapping;
using FinX.Core.Dto.Request.Hospital;
using FinX.Core.Entity;
using FinX.Core.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinX.Api.Controllers
{
    [ApiController]
    [Route("api/Hospitals")]
    [Authorize]
    public class HospitalController : ControllerBase
    {
        private readonly IHospitalService _hospitalService;
        private readonly IGrupoPacienteHospitalService _grupoService;

        public HospitalController(IHospitalService hospitalService, IGrupoPacienteHospitalService grupoService)
        {
            _hospitalService = hospitalService;
            _grupoService = grupoService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HospitalRequestDto hospital)
        {
            var existing = await _hospitalService.GetByCnpjAsync(hospital.Cnpj);
            if (existing != null)
                return BadRequest($"Hospital com CNPJ {hospital.Cnpj} já existe.");

            var created = await _hospitalService.CreateAsync(hospital.ToEntity());
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created.ToResponse());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var hospital = await _hospitalService.GetAsync(id);
            if (hospital == null) return NotFound();
            return Ok(hospital.ToResponse());
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var hospitals = await _hospitalService.GetAllAsync();
            return Ok(hospitals.Select(x => x.ToResponse()));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] HospitalRequestDto hospital)
        {
            if (id != hospital.Id) return BadRequest();
            var updated = await _hospitalService.UpdateAsync(hospital.ToEntity());
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _hospitalService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPost("{hospitalId:guid}/patients/{patientId:guid}")]
        public async Task<IActionResult> AddPatientToHospital(Guid hospitalId, Guid patientId, [FromBody] string codigo)
        {
            var existing = await _grupoService.GetByPacienteHospitalAsync(patientId, hospitalId);
            if (existing != null)
                return BadRequest("Paciente já está vinculado a este hospital.");

            var grupo = new GrupoPacienteHospital
            {
                PacienteId = patientId,
                HospitalId = hospitalId,
                Codigo = codigo
            };

            var created = await _grupoService.CreateAsync(grupo);
            return CreatedAtAction(nameof(GetPatientHospitalRelation),
                new { hospitalId, patientId }, created);
        }

        [HttpGet("{hospitalId:guid}/patients/{patientId:guid}")]
        public async Task<IActionResult> GetPatientHospitalRelation(Guid hospitalId, Guid patientId)
        {
            var grupo = await _grupoService.GetByPacienteHospitalAsync(patientId, hospitalId);
            if (grupo == null) return NotFound();
            return Ok(grupo);
        }

        [HttpGet("{hospitalId:guid}/patients")]
        public async Task<IActionResult> GetHospitalPatients(Guid hospitalId)
        {
            var grupos = await _grupoService.GetByHospitalAsync(hospitalId);
            return Ok(grupos);
        }
    }
}
