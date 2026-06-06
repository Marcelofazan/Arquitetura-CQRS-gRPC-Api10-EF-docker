using MongoDB.Driver;
using Sistema.Reporter.Business.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema.Reporter.Business.Service
{
    public class RelatorioMongoService : IRelatorioMongoService
    {
        private readonly IMongoCollection<RelatorioDiario> _relatorioCollection;
        public RelatorioMongoService(IMongoDatabase mongoDatabase) => _relatorioCollection = mongoDatabase.GetCollection<RelatorioDiario>("relatorios_diarios");

        public async Task<RelatorioDiario?> GetRelatorioDiarioByDateAsync(string data)
        {
            var filtro = Builders<RelatorioDiario>.Filter.Eq(r => r.Data, data);
            return await _relatorioCollection.Find(filtro).FirstOrDefaultAsync();
        }

        public async Task InsertOrUpdateClienteAsync(ClienteDataModel cliente, string data)
        {
            var filtro = Builders<RelatorioDiario>.Filter.Eq(r => r.Data, data);
            var updateCliente = Builders<RelatorioDiario>.Update.Push(r => r.Clientes, cliente);
            await _relatorioCollection.UpdateOneAsync(filtro, updateCliente, new UpdateOptions { IsUpsert = true });
        }

        public async Task UpdateTotalClientesAsync(string data, double totalFisica, double totalJuridica)
        {
            var filtro = Builders<RelatorioDiario>.Filter.Eq(r => r.Data, data);
            var updateTotais = Builders<RelatorioDiario>.Update
                .Set(r => r.TotalFisica, totalFisica)
                .Set(r => r.TotalJuridica, totalJuridica);
            await _relatorioCollection.UpdateOneAsync(filtro, updateTotais);
        }
    }
}
