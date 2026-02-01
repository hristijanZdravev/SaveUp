using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaveUp.DTOs;
using SaveUp.Models;
using SaveUp.Services.Implementations;

namespace SaveUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IFeeCalculationService _feeService;

        public TransactionController(IFeeCalculationService feeService)
        {
            _feeService = feeService;
        }


        [HttpPost("calculate")]
        public async Task<ActionResult<TransactionRequestDTO>> Calculate([FromBody] TransactionRequestDTO request)
        {
            if (request == null) {
                return BadRequest("Transaction request is required.");
            }

            if(!await _feeService.ValidateTransactionRequestAsync(request)) {
                return BadRequest("Invalid transaction request. Please check the provided data.");
            }

            var result = await _feeService.CalculateFeeAsync(request);
            return Ok(result);
        }


        [HttpPost("calculate-batch")]
        public async Task<ActionResult<List<TransactionRequestDTO>>> CalculateBatch([FromBody] List<TransactionRequestDTO> requests)
        {
            if (requests == null || requests.Count == 0){
                return BadRequest("Batch of transactions is required.");
            }

            if (!await _feeService.ValidateTransactionRequestsAsync(requests))
            {
                return BadRequest("Invalid transaction requests. Please check the provided data.");
            }

            var results = await _feeService.CalculateFeesBatchAsync(requests);
            return Ok(results);
        }

        [HttpGet("history")]
        public async Task<ActionResult<List<FeeCalculationHistoryDTO>>> GetHistory()
        {
            var history = await _feeService.GetCalculationHistoryAsync();
            return Ok(history);
        }
    }
}
