using System;

namespace FunctionAppProcessarMoedas.Models;

public class DadosCotacao
{
    public string Sigla { get; set; }
    public string Horario { get; set; }
    public decimal? Valor { get; set; }
}