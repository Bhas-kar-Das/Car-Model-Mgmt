using DemoAppAdo.Data;
using DemoAppAdo.DTOs;
using DemoAppAdo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoAppAdo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesmanCommissionController : ControllerBase
    {
        private readonly SalesRecordRepository _repository;

        public SalesmanCommissionController(SalesRecordRepository repository)
        {
            _repository = repository;
        }

        // GET: api/salesmancommission/commission-report
        [HttpGet("commission-report")]
        public async Task<ActionResult<IEnumerable<SalesmanCommissionReport>>> GetSalesmanCommissionReport()
        {
            var report = await _repository.GetSalesmanCommissionReportAsync();
            return Ok(report);
        }

        // POST: api/salesmancommission/update-commission
        [HttpPost("update-commission")]
        public async Task<IActionResult> UpdateCommission([FromBody] UpdateCommissionDto updateCommissionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _repository.UpdateCommissionAsync(updateCommissionDto);
                return Ok("Commission updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/salesmancommission/create
        [HttpPost]
        public async Task<IActionResult> CreateSalesRecord([FromBody] SalesRecordDto salesRecordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var salesRecord = new SalesRecord
            {
                SalesmanId = salesRecordDto.SalesmanId,
                CarModelId = salesRecordDto.CarModelId,
                NumberOfCarsSold = salesRecordDto.NumberOfCarsSold,
                Brand = salesRecordDto.Brand,
                Class = salesRecordDto.Class,
            };

            await _repository.InsertSalesRecord(salesRecord);
            return CreatedAtAction(nameof(CreateSalesRecord), new { id = salesRecord.Id }, salesRecord);
        }
    }
}
