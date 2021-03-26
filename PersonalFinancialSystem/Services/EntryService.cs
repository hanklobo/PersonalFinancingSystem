using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace PersonalFinancialSystem.Services
{
    public class EntryService : IEntryService
    {
        private IConfiguration _config;
        private readonly MySqlConnection _conn;
        public EntryService(IConfiguration config)
        {
            _config = config;
            _conn = new MySqlConnection(_config.GetConnectionString("Pfs"));
        }
        public EntryOutput Get(DateTime startDate)
        {
            var endDate = startDate.AddMonths(1).AddTicks(-1);
            var sqlLancamento =
                $"SELECT DATE_FORMAT(data,'%Y/%m/%d') as data, valor, descricao, conta, tipo FROM entry WHERE Data BETWEEN '{startDate.ToString("u")}' AND '{endDate.ToString("u")}'";
            var sqlTotal =
                $"SELECT IFNULL(conta, 'Saldo') as conta, sum(Case When tipo = 'c' Then valor Else valor*-1 End) as valor FROM entry WHERE Data BETWEEN '{startDate.ToString("u")}' AND '{endDate.ToString("u")}' GROUP BY conta WITH ROLLUP";
            var totais = _conn.Query<Totalizador>(sqlTotal).ToList();
            var retorno = new EntryOutput();
            foreach (var total in totais)
                retorno.Totalizadores.Add(total.Conta, total.Valor);
            retorno.Lancamentos = _conn.Query<Entry>(sqlLancamento).ToList();
            return retorno;
        }

        public void Post(IList<Entry> lancamentos)
        {
            _conn.Open();
            var trans = _conn.BeginTransaction();
            try
            {
                _conn.Execute(@"
                    insert entry(data, descricao, valor, conta, tipo)
                    values(@Data, @Descricao, @Valor, @Conta, @Tipo)", lancamentos, trans);
                trans.Commit();
                _conn.Close();
            }
            catch (Exception e)
            {
                trans.Rollback();
                _conn.Close();
                throw new Exception(e.Message);
            }
        }
    }
}