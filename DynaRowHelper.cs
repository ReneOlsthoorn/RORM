using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RORM
{
    public class DynaRowHelper
    {
        public enum State
        {
            New, Existing
        }

        public enum ActionPlan
        {
            Create, Nothing, Update, Delete
        }

        public Connector Connector { get; set; }
        public Dictionary<string, object> Data = null;    // Herinner je de AdTaker data dictionary? Twintig jaar later bestaat ie nog steeds...
        public Dictionary<string, object> OldData = null;
        public State DBState { get; set; }
        public ActionPlan DBActionPlan { get; set; }
        public string TableName = null;
        public string AutoincrementField = null;
        public string UpdateId = null;
        public Action<string, List<Parameter>> OnAfterParameterCreation = null;

        public DynaRowHelper(string tableName = null)
        {
            TableName = tableName;
            Data = new Dictionary<string, object>();
            OldData = new Dictionary<string, object>();
            DBState = State.New;
            DBActionPlan = ActionPlan.Nothing;
        }

        public object OldestValue(string kolom)
        {
            if (OldData.ContainsKey(kolom))
                return OldData[kolom];

            if (Data.ContainsKey(kolom))
                return Data[kolom];

            return null;
        }

        public void Change(string kolom, object waarde)
        {
            if (DBState == State.New)
            {
                Data[kolom] = waarde;
                DBActionPlan = ActionPlan.Create;
                return;
            }
            //Vanaf nu is DBState == State.Existing

            if (!OldData.ContainsKey(kolom))
            {
                if (Data.ContainsKey(kolom))   // if-statement added when the existing object has more columns in the database not selected in the query.
                    OldData[kolom] = Data[kolom];
                else
                    OldData[kolom] = null;
            }
            Data[kolom] = waarde;

            if (DBActionPlan == ActionPlan.Nothing)
                DBActionPlan = ActionPlan.Update;
        }

        public async Task<int> DeleteAsync(Transaction transaction = null)
        {
            DBActionPlan = ActionPlan.Delete;
            return await DBPersistAsync(transaction);
        }

        public async Task<int> UpdateAsync(Transaction transaction = null)
        {
            return await DBPersistAsync(transaction);
        }

        public async Task<int> InsertAsync(string table = null, string autoincrement = null, Transaction transaction = null)
        {
            if (table != null)
                TableName = table;

            if (autoincrement != null)
                AutoincrementField = autoincrement;

            SetToCreate();
            return await DBPersistAsync(transaction);
        }

        public async Task<int> DBPersistAsync(Transaction tran = null)
        {
            int rowsAffected = -1;
            if (TableName == null)
                return rowsAffected;

            List<Parameter> parameters = new List<Parameter>();
            var connection = Connector.Connection;

            Transaction transaction = null;
            if (tran == null)
                transaction = connection.BeginTransaction();

            if (this.DBActionPlan == DynaRowHelper.ActionPlan.Update)
            {
                (string setPart, int parameterCounter) = GetSet(parameters);
                string wherePart = GetWhere(parameters, parameterCounter);
                ParameterCorrection(parameters);
                string updateStatement = $"update {TableName} set {setPart}{wherePart}";
                rowsAffected = await Connector.ExecuteNonQueryAsync(updateStatement, parameters);
                JustSavedInDatabase();
            }
            else if (this.DBActionPlan == DynaRowHelper.ActionPlan.Create)
            {
                string columnsPart;
                string valuesPart;
                GetInsertColumns(parameters, out columnsPart, out valuesPart);
                ParameterCorrection(parameters);
                string updateStatement = "";
                if (columnsPart == null)
                    updateStatement = $"insert into {TableName} DEFAULT VALUES";
                else
                    updateStatement = $"insert into {TableName} ({columnsPart}) values ({valuesPart})";

                rowsAffected = await Connector.ExecuteNonQueryAsync(updateStatement, parameters);
                if (AutoincrementField != null)
                {
                    string[] autoinc = AutoincrementField.Split(";");
                    string autoIncColumnName = autoinc[0];
                    Data[autoIncColumnName] = await Connector.GetAutoincrementValue(transaction, autoinc[1]);
                }
                JustSavedInDatabase();
            }
            else if (this.DBActionPlan == DynaRowHelper.ActionPlan.Delete)
            {
                string wherePart = GetWhere(parameters);
                ParameterCorrection(parameters);
                string updateStatement = $"delete from {TableName} {wherePart}";
                rowsAffected = await Connector.ExecuteNonQueryAsync(updateStatement, parameters);
                JustDeletedInDatabase();
            }
            else if (this.DBActionPlan == DynaRowHelper.ActionPlan.Nothing) { }

            if (transaction != null)
                transaction.Commit();

            return rowsAffected;
        }

        public void JustLoadedFromDatabase()
        {
            DBActionPlan = ActionPlan.Nothing;
            DBState = State.Existing;
        }

        public void JustSavedInDatabase()
        {
            DBActionPlan = ActionPlan.Nothing;
            DBState = State.Existing;
            OldData = new Dictionary<string, object>();
        }

        public void JustDeletedInDatabase()
        {
            this.SetToCreate();
        }

        public void ParameterCorrection(List<Parameter> parameters)
        {
            if (OnAfterParameterCreation != null)
                OnAfterParameterCreation(TableName, parameters);
        }

        public void SetToCreate()
        {
            DBActionPlan = ActionPlan.Create;
            DBState = State.New;
            OldData = new Dictionary<string, object>();
        }

        StringBuilder wherePart = null;
        public void EnsureAnd()
        {
            if (wherePart == null)
            {
                wherePart = new StringBuilder();
                wherePart.Append(" where ");
            }
            else
                wherePart.Append(" and ");
        }

        public void AddWhereAnd(string sqlString)
        {
            EnsureAnd();
            wherePart.Append(sqlString);
        }

        public string GetWhere(List<Parameter> parameters, int parameterCounter = 1)
        {
            wherePart = null;

            if (UpdateId != null)
            {
                Parameter parameter = Connector.GetParameter(OldestValue(UpdateId), "where" + UpdateId);
                parameters.Add(parameter);
                string parameterName = parameter.ParameterName;
                if (parameterName == null)
                    parameterName = $"@{parameterCounter++}";

                AddWhereAnd($"{UpdateId} = {parameterName}");
            }
            else
            {
                foreach (string key in Data.Keys)
                {
                    object waarde = OldestValue(key);
                    if (waarde == null)
                        continue;

                    Parameter parameter = Connector.GetParameter(OldestValue(key), "where" + key);
                    parameters.Add(parameter);
                    string parameterName = parameter.ParameterName;
                    if (parameterName == null)
                        parameterName = $"@{parameterCounter++}";

                    AddWhereAnd($"{key} = {parameterName}");
                }
            }
            return wherePart.ToString();
        }


        StringBuilder setPart = null;
        public void EnsureSetComma()
        {
            if (setPart == null)
                setPart = new StringBuilder();
            else
                setPart.Append(",");
        }

        public void AddSetAnd(string sqlString)
        {
            EnsureSetComma();
            setPart.Append(sqlString);
        }

        public (string, int) GetSet(List<Parameter> parameters)
        {
            setPart = null;
            var parameterCounter = 1;
            foreach (string key in Data.Keys)
            {
                Parameter parameter = Connector.GetParameter(Data[key], "set" + key);
                parameters.Add(parameter);
                string parameterName = parameter.ParameterName;
                if (parameterName == null)
                    parameterName = $"@{parameterCounter++}";

                AddSetAnd($"{key} = {parameterName}");
            }
            return (setPart.ToString(), parameterCounter);
        }


        StringBuilder insertPart1 = null;
        StringBuilder insertPart2 = null;
        public void EnsureInsertComma()
        {
            if (insertPart1 == null)
            {
                insertPart1 = new StringBuilder();
                insertPart2 = new StringBuilder();
            }
            else
            {
                insertPart1.Append(",");
                insertPart2.Append(",");
            }
        }

        public void AddInsertAnd(string part1, string part2)
        {
            EnsureInsertComma();
            insertPart1.Append(part1);
            insertPart2.Append(part2);
        }

        public void GetInsertColumns(List<Parameter> parameters, out string columnsPart, out string valuesPart)
        {
            insertPart1 = null;
            insertPart2 = null;
            int teller = 0;
            int parameterCounter = 1;
            foreach (string key in Data.Keys)
            {
                Parameter parameter = Connector.GetParameter(Data[key], key, teller++);
                parameters.Add(parameter);
                string parameterName = parameter.ParameterName;
                if (parameterName == null)
                    parameterName = $"@{parameterCounter++}";

                AddInsertAnd(parameter.EnsureValidColumnName(key), parameterName);
            }
            if (insertPart1 == null)
            {
                columnsPart = null;
                valuesPart = null;
                return;
            }
            columnsPart = insertPart1.ToString();
            valuesPart = insertPart2.ToString();
        }

    }
}
