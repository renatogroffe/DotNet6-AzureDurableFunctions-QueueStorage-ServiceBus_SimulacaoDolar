using System;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using FunctionAppProcessarMoedas.Models;

namespace FunctionAppProcessarMoedas;

public class FunctionSimularCotacaoDolar
{
    private const decimal VALOR_BASE = 5.20m;

    [FunctionName("SimularCotacaoDolar")]
    public static DadosCotacao SimularCotacaoDolar(
        [ActivityTrigger] ParametrosExecucao parametrosExecucao, ILogger log)
    {
        log.LogInformation(
            $"{nameof(SimularCotacaoDolar)} - Iniciando a execucao...");
        var cotacao = new DadosCotacao()
        {
            Sigla = parametrosExecucao.Moeda,
            Horario = parametrosExecucao.DataReferencia,
            Valor = Math.Round(VALOR_BASE + new Random().Next(0, 21) / 1000m, 3)
        };
        log.LogInformation(
            $"{nameof(SimularCotacaoDolar)} - Dados gerados: " +
            JsonSerializer.Serialize(cotacao));
        return cotacao;
    }
}