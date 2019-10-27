using CSM.Commands.Data;
using CSM.Commands.Handler;
using CSM.Helpers;
using System.Collections.Generic;
using CSM.Commands.Data.Internal;
using CSM.Container;

namespace CSM.Commands
{
    public static class TransactionHandler
    {
        private static bool _sendStarted = false;
        private static readonly List<Tuple<CommandHandler, CommandBase, int>> _receivedTransactions = new List<Tuple<CommandHandler, CommandBase, int>>();

        /// <summary>
        /// Starts a transaction.
        /// </summary>
        public static void StartTransaction()
        {
            _sendStarted = true;
        }

        /// <summary>
        /// Starts a transaction if the command is not a finish command.
        /// </summary>
        /// <param name="command">A received command</param>
        public static void StartTransaction(CommandBase command)
        {
            if (Command.GetCommandHandler(command.GetType()).TransactionCmd)
            {
                _sendStarted = true;
            }
        }

        /// <summary>
        /// Finishes all transactions that were started before.
        /// </summary>
        public static void FinishSend()
        {
            if (_sendStarted)
            {
                Command.SendToAll(new FinishTransactionCommand());
                _sendStarted = false;
            }
        }

        /// <summary>
        /// Checks if a received command is a transaction command
        /// and queues it until a FinishTransactionCommand is received.
        /// </summary>
        /// <param name="handler">The received command type.</param>
        /// <param name="cmd">The received command.</param>
        /// <returns>true, if the command is a transaction command</returns>
        public static bool CheckReceived(CommandHandler handler, CommandBase cmd)
        {
            if (!handler.TransactionCmd)
            {
                return false;
            }

            _receivedTransactions.Add(new Tuple<CommandHandler, CommandBase, int>(handler, cmd, cmd.SenderId));

            return true;
        }

        /// <summary>
        /// Called when the FinishTransactionCommand was received.
        /// </summary>
        /// <param name="sender">The sending player, -1 if it's the server.</param>
        public static void FinishReceived(int sender)
        {
            foreach (Tuple<CommandHandler, CommandBase, int> transaction in _receivedTransactions)
            {
                if (transaction.Var3 != sender)
                {
                    continue;
                }

                transaction.Var1.Parse(transaction.Var2);
            }

            ClearTransactions(sender);
        }

        /// <summary>
        /// Clears all transactions by the given sender.
        /// </summary>
        /// <param name="clientId">The sender's client id.</param>
        public static void ClearTransactions(int clientId)
        {
            _receivedTransactions.RemoveAll(tuple => tuple.Var3 == clientId);
        }

        /// <summary>
        /// Clears all transactions.
        /// </summary>
        public static void ClearTransactions()
        {
            _sendStarted = false;
            _receivedTransactions.Clear();
        }
    }
}
