using NCalc;
using System.Data;
using SaveUp.DTOs;
using SaveUp.DTOs.Helper;
using SaveUp.Repository.Interfaces;
using SaveUp.Services.Implementations;
using SaveUp.Models.Transactions;

namespace SaveUp.Services
{
    public class RuleEngineService : IRuleEngineService
    {
        public TransactionResponseDTO Evaluate(TransactionRequestDTO tx, List<FeeRule> rules)
        {
            double total = 0;
            TransactionResponseDTO transactionResponse = new TransactionResponseDTO();

            foreach (FeeRule rule in rules)
            {
                var context = BuildContext(tx);

                Expression condition = new Expression(rule.ConditionExpression);
                SetParameters(condition, context);

                if (EvaluateCondition(rule.ConditionExpression, context))
                {
                    total += EvaluateCalculation(rule.CalculationExpression, context, rule);
                    transactionResponse.FeeRuleIds.Add(rule.Id);
                    transactionResponse.FeeRuleNames.Add(rule.Name);
                }
            }

            //Round the total fee to 3 decimal places
            transactionResponse.Fee = Math.Round(ExchangeRates.ConvertFromEUR(total, tx.Currency.Name), 3);

            return transactionResponse;
        }

        private bool EvaluateCondition(string conditionExpression, Dictionary<string, object> context)
        {
            var expression = new Expression(conditionExpression);
            SetParameters(expression, context);

            var result = expression.Evaluate();

            if (result is bool booleanResult)
                return booleanResult;
            else
                return false;
        }

        private double EvaluateCalculation(string calculationExpression, Dictionary<string, object> context, FeeRule rule)
        {
            Expression calc = new Expression(calculationExpression);
            SetParameters(calc, context);

            double fee = Convert.ToDouble(calc.Evaluate());

            if (rule.MaxFee.HasValue)
            {
                fee = Math.Min(fee, rule.MaxFee.Value);
            }

            if (rule.DiscountPercent.HasValue)
            {
                fee -= fee * (rule.DiscountPercent.Value / (double)100m);
            }

            return fee;
        }

        private Dictionary<string, object> BuildContext(TransactionRequestDTO tx)
        {
            double amountInEUR = ExchangeRates.ConvertToEUR(tx.Amount, tx.Currency.Name);

            return new Dictionary<string, object>
            {

                ["Type"] = tx.Type.Name,
                ["Amount"] = amountInEUR,
                ["Currency"] = tx.Currency.Name,
                ["IsDomestic"] = tx.IsDomestic,
                ["CreditScore"] = tx.Client?.CreditScore ?? 0,
                ["Segment"] = tx.Client?.Segment.Name ?? "",
                ["RiskLevel"] = tx.Client?.RiskLevel.ToString() ?? ""
            };
        }

        private void SetParameters(Expression expr, Dictionary<string, object> ctx)
        {
            foreach (var kvp in ctx)
                expr.Parameters[kvp.Key] = kvp.Value;
        }
    }
}
