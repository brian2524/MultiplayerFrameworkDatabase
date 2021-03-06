﻿using MWFDataLibrary.DataAccess;
using MWFModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Threading.Tasks;

namespace MWFDataLibrary.BuisnessLogic
{
    public static class GameInstanceProcessor
    {
        public static async Task<int> CreateGameInstanceAndReturnIdAsync(string connString, int processId, int game, string port, string args, int hostId)
        {
            // name of stored procedure to execute
            string procedureName = "spGameInstance_CreateAndOutputId";


            // create the data table representation of the user defined game instance table
            DataTable gameInstanceTable = new DataTable("@inGameInstance");
            gameInstanceTable.Columns.Add("ProcessId", typeof(int));
            gameInstanceTable.Columns.Add("Game", typeof(int));
            gameInstanceTable.Columns.Add("Port");
            gameInstanceTable.Columns.Add("Args");
            gameInstanceTable.Columns.Add("HostId");
            // fill in the data
            gameInstanceTable.Rows.Add(processId, game, port, args, hostId);

            // make parameters to pass to the stored procedure
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@inGameInstance", gameInstanceTable.AsTableValuedParameter("udtGameInstance"), DbType.Object);
            parameters.Add("@outId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // execute our stored procedure with the parameters we made
            await SqlDataAccess.ModifyDataAsync(connString, procedureName, parameters);
            // return the outputed id from the stored procedure
            return parameters.Get<int>("@outId");
        }

        public static async Task<int> DeleteGameInstanceByIdAsync(string connString, int id)
        {
            string storedProcedureName = "spGameInstance_DeleteById";

            // make parameters to pass to the stored procedure
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@inId", id, dbType: DbType.Int32);

            return await SqlDataAccess.ModifyDataAsync(connString, storedProcedureName, parameters);
        }
        public static async Task<int> DeleteGameInstancesByHostIdAsync(string connString, int hostId)
        {
            string storedProcedureName = "spGameInstance_DeleteByHostId";

            // make parameters to pass to the stored procedure
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@inHostId", hostId, dbType: DbType.Int32);
            parameters.Add("@outRowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

            return await SqlDataAccess.ModifyDataAsync(connString, storedProcedureName, parameters);
        }

        public static async Task<IEnumerable<GameInstanceModel>> GetGameInstancesAsync(string connString)
        {
            string storedProcedureName = "spGameInstance_SelectAll";
            return await SqlDataAccess.LoadDataAsync<GameInstanceModel>(connString, storedProcedureName);
        }
        public static async Task<IEnumerable<GameInstanceWithHostIpModel>> GetGameInstancesWithHostsAsync(string connString)
        {
            string storedProcedureName = "spGameInstanceWithHost_SelectAll";
            return await SqlDataAccess.LoadDataAsync<GameInstanceWithHostIpModel>(connString, storedProcedureName);
        }
    }
}
