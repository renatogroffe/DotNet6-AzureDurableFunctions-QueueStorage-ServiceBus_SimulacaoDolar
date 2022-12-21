using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using FunctionAppProcessarMoedas.Models;

namespace FunctionAppProcessarMoedas;

public static class ProcessarSimulacaoDolar
{
    [FunctionName("ProcessarSimulacaoDolar")]
    public static async Task RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var parametrosExecucao = context.GetInput<ParametrosExecucao>();

        var dadosCotacao = await context.CallActivityAsync<DadosCotacao>(
            "SimularCotacaoDolar", parametrosExecucao);

        var tasks = new Task<bool>[2];
        tasks[0] = context.CallActivityAsync<bool>(
            "NotificarAzureQueueStorage", dadosCotacao );
        tasks[1] = context.CallActivityAsync<bool>(
            "NotificarAzureServiceBus", dadosCotacao );
        await Task.WhenAll(tasks);
    }
}