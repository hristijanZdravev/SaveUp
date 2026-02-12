using Azure;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using SaveUp.Data;
using SaveUp.DTOs;
using SaveUp.Repository.Interfaces;
using SaveUp.Services.Implementations;
using SaveUp.Models.Transactions;

namespace SaveUp.Services
{
    public class FeeCalculationService : IFeeCalculationService
    {

        private readonly IRuleEngineService _ruleEngineService;
        private readonly IFeeCalculationHistoryRepository _feeCalculationHistoryRepository;
        private readonly IFeeRuleRepository _feeRuleRepository;
        public readonly ITransactionRepository _transactionRepository;
        public readonly ICommonRepository<Currency, CurrencyDTO> _currencyRepository;
        public readonly ICommonRepository<Client, ClientDTO> _clientRepository;
        public readonly ICommonRepository<TransactionType, TransactionTypeDTO> _transactionTypeRepository;

        public FeeCalculationService(IRuleEngineService ruleEngine, IFeeCalculationHistoryRepository feeCalculationHistoryRepository, IFeeRuleRepository feeRuleRepository, ITransactionRepository transactionRepository, ICommonRepository<Currency, CurrencyDTO> currencyRepository, ICommonRepository<Client, ClientDTO> clientRepository, ICommonRepository<TransactionType, TransactionTypeDTO> transactionTypeRepository)
        {
            _ruleEngineService = ruleEngine;
            _feeCalculationHistoryRepository = feeCalculationHistoryRepository;
            _feeRuleRepository = feeRuleRepository;
            _transactionRepository = transactionRepository;
            _currencyRepository = currencyRepository;
            _clientRepository = clientRepository;
            _transactionTypeRepository = transactionTypeRepository;
        }

        public async Task<bool> ValidateTransactionRequestAsync(TransactionRequestDTO request)
        {
            return await _currencyRepository.ExistsAsync(request.Currency.Id) &&
                   await _transactionTypeRepository.ExistsAsync(request.Type.Id) &&
                   await _clientRepository.ExistsAsync(request.Client.Id);
        }

        public async Task<bool> ValidateTransactionRequestsAsync(List<TransactionRequestDTO> requests)
        {
            return await Task.Run(async () =>
            {
                var currencyIds = requests.Select(r => r.Currency.Id).Distinct().ToList();
                var typeIds = requests.Select(r => r.Type.Id).Distinct().ToList();
                var clientIds = requests.Select(r => r.Client.Id).Distinct().ToList();

                var currencyExists = await _currencyRepository.ExistAllAsync(currencyIds);
                var typeExists = await _transactionTypeRepository.ExistAllAsync(typeIds);
                var clientExists = await _clientRepository.ExistAllAsync(clientIds);

                return currencyExists && typeExists && clientExists;
            });
        }

        public async Task<TransactionResponseDTO> CalculateFeeAsync(TransactionRequestDTO request)
        {         
            List<FeeRule> rules = await _feeRuleRepository.GetAllAsync();
            TransactionResponseDTO transactionResponseDTO = _ruleEngineService.Evaluate(request, rules);

            var history = new FeeCalculationHistory
            {
                Timestamp = DateTime.UtcNow,
                Transaction = TransactionRequestDTO.ToTransaction(request),
                FeeRules = rules.Where(rule => transactionResponseDTO.FeeRuleIds.Contains(rule.Id)).ToList(),
                CalculatedFee = transactionResponseDTO.Fee,
            };

            await _feeCalculationHistoryRepository.AddAsync(history);
            return transactionResponseDTO;
        }

        public async Task<List<TransactionResponseDTO>> CalculateFeesBatchAsync(List<TransactionRequestDTO> requests)
        {
            List<FeeRule> rules = await _feeRuleRepository.GetAllAsync();

            List<TransactionResponseDTO> response = new List<TransactionResponseDTO>();
            List<FeeCalculationHistory> historyList = new List<FeeCalculationHistory>();

            requests.ForEach(req =>
            {
                TransactionResponseDTO transactionResponseDTO = _ruleEngineService.Evaluate(req, rules);
                response.Add(transactionResponseDTO);

                historyList.Add(new FeeCalculationHistory
                {
                    Timestamp = DateTime.UtcNow,
                    Transaction = TransactionRequestDTO.ToTransaction(req),
                    FeeRules = rules.Where(rule => transactionResponseDTO.FeeRuleIds.Contains(rule.Id)).ToList(),
                    CalculatedFee = transactionResponseDTO.Fee,
                });
            });

            await _feeCalculationHistoryRepository.AddRangeAsync(historyList);

            return response;
        }

        public async Task<List<FeeCalculationHistoryDTO>> GetCalculationHistoryAsync()
        {
            return await _feeCalculationHistoryRepository.GetAllAsync();
        }
    }
}
