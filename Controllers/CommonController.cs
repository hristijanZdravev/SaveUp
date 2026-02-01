using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaveUp.DTOs;
using SaveUp.Models;
using SaveUp.Repository.Interfaces;

namespace SaveUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        public readonly ICommonRepository<Currency, CurrencyDTO> _currencyRepository;
        public readonly ICommonRepository<Client, ClientDTO> _clientRepository;
        public readonly ICommonRepository<TransactionType, TransactionTypeDTO> _transactionTypeRepository;

        public CommonController(
            ICommonRepository<Currency, CurrencyDTO> currencyRepository,
            ICommonRepository<Client, ClientDTO> clientRepository,
            ICommonRepository<TransactionType, TransactionTypeDTO> transactionTypeRepository)
        {
            _currencyRepository = currencyRepository;
            _clientRepository = clientRepository;
            _transactionTypeRepository = transactionTypeRepository;
        }

        [HttpGet("clients")]
        public async Task<ActionResult<List<ClientDTO>>> GetClients()
        {
            List<ClientDTO> clientDTOs = await _clientRepository.GetAllAsync();
            return Ok(clientDTOs);
        }

        [HttpGet("type")]
        public async Task<ActionResult<List<TransactionTypeDTO>>> GetTransactionTypes()
        {
            List<TransactionTypeDTO> transactionTypeDTOs = await _transactionTypeRepository.GetAllAsync();
            return Ok(transactionTypeDTOs);
        }

        [HttpGet("currency")]
        public async Task<ActionResult<List<CurrencyDTO>>> GetCurrencies()
        {
            List<CurrencyDTO> currencyDTOs = await _currencyRepository.GetAllAsync();
            return Ok(currencyDTOs);
        }

    }
}
