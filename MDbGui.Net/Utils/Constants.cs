using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.Utils
{
    public static class Constants
    {
        public const string AggregateOperation = "Aggregate";
        public const string CommandOperation = "Command";
        public const string DistinctOperation = "Distinct";
        public const string EvalOperation = "Eval";
        public const string FindOperation = "Find";
        public const string InsertOperation = "Insert";
        public const string RemoveOperation = "Remove";
        public const string ReplaceOperation = "Replace";
        public const string UpdateOperation = "Update";

        public const string AggregateParseException = "AggregateParseException";
        public const string AggregatePipelineProperty = "AggregatePipeline";

        public const string CommandPropertyProperty = "Command";
        public const string CommandParseException = "CommandParseException";

        public const string DistinctFilterProperty = "DistinctFilter";
        public const string DistinctParseException = "DistinctParseException";

        public const string FindParseException = "FindParseException";
        public const string FindProperty = "Find";
        public const string SortProperty = "Sort";
        public const string ProjectionProperty = "Projection";

        public const string InsertProperty = "Insert";
        public const string InsertParseException = "InsertParseException";

        public const string DeleteQueryProperty = "DeleteQuery";
        public const string RemoveParseException = "RemoveParseException";
        public const string DeleteResultMessage = "DeleteResult";
        public const string ConfirmDeleteResultMessage = "ConfirmDeleteResult";

        public const string ReplaceFilterProperty = "ReplaceFilter";
        public const string ReplacementProperty = "Replacement";
        public const string ReplaceParseException = "ReplaceParseException";

        public const string ReplaceOneParseException = "ReplaceOneParseException";

        public const string UpdateFilterProperty = "UpdateFilter";
        public const string UpdateDocumentProperty = "UpdateDocument";
        public const string UpdateParseException = "UpdateParseException";
        public const string UpdateDocumentMessage = "UpdateDocument";

        public const string LoggingInMessage = "LoggingIn";
        public const string DisconnectMessage = "Disconnect";

        public const string OpenTab = "OpenTab";
        public const string CloseTab = "CloseTab";

        public const string ItemExpandingMessage = "ItemExpanding";

        public const string EditResultMessage = "EditResult";

        public const string OpenCreateIndexMessage = "OpenCreateIndex";
        public const string CreateIndexMessage = "CreateIndex";
        public const string RecreateIndexMessage = "RecreateIndex";
        public const string EditIndexMessage = "EditIndex";
        public const string ConfirmDropIndexMessage = "ConfirmDropIndex";
        public const string DropIndexMessage = "DropIndex";

        public const string ConfirmDropCollectionMessage = "ConfirmDropCollection";
        public const string DropCollectionMessage = "DropCollection";
        public const string OpenCreateNewCollectionMessage = "OpenCreateNewCollection";
        public const string CreateCollectionMessage = "CreateCollection";

        public const string ConfirmDropDatabaseMessage = "ConfirmDropDatabase";
        public const string DropDatabaseMessage = "DropDatabase";

        public const string EditUserMessage = "EditUser";
        public const string ConfirmDeleteUserMessage = "ConfirmDeleteUser";
        public const string DeleteUserMessage = "DeleteUser";
    }
}
