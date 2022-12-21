using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using FunctionAppProcessarMoedas.Models;

namespace FunctionAppProcessarMoedas;

public static class ProcessarSimulacaoDolarQueueStart
{
    [FunctionName("ProcessarSimulacaoDolar_QueueStart")]
    public static async Task QueueStart(
        [QueueTrigger("queue-start")] string instrucao,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        if (instrucao != "SIMULAR_DOLAR")
        {
            log.LogError(
                $"{nameof(QueueStart)} - " +
                $"A operacao {instrucao} nao e valida!");
            return;
        }

        string instanceId = await starter.StartNewAsync<ParametrosExecucao>(
            "ProcessarSimulacaoDolar",
            new ()
            {
                Moeda = "USD",
                DataReferencia = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}"
            });
       
        log.LogInformation($"{nameof(QueueStart)} - Iniciada orquestração com ID = '{instanceId}'.");
    }
}