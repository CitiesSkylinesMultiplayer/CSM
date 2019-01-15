using CSM.Commands.Data;
using CSM.Commands.Handler;
using CSM.Helpers;
using CSM.Networking;
using System.Collections.Generic;

namespace CSM.Commands
{
    public class TransactionHandler
    {
        private static bool _sendStarted = false;
        private static readonly List<Tuple<CommandHandler, byte[], Player>> _receivedTransactions = new List<Tuple<CommandHandler, byte[], Player>>();

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
        /// <param name="data">The received data.</param>
        /// <param name="player">The sending player, may be null if it's the server.</param>
        /// <returns>true, if the command is a transaction command</returns>
        public static bool CheckReceived(CommandHandler handler, byte[] data, Player player)
        {
            if (!handler.TransactionCmd)
            {
                return false;
            }

            _receivedTransactions.Add(new Tuple<CommandHandler, byte[], Player>(handler, data, player));

            return true;
        }

        /// <summary>
        /// Called when the FinishTransactionCommand was received.
        /// </summary>
        /// <param name="player">The sending player, may be null if it's the server.</param>
        public static void FinishReceived(Player player)
        {
            foreach (Tuple<CommandHandler, byte[], Player> transaction in _receivedTransactions)
            {
                if (transaction.Var3 != player)
                {
                    continue;
                }

                if (MultiplayerManager.Instance.CurrentClient.Status == Networking.Status.ClientStatus.Connected)
                {
                    transaction.Var1.ParseOnClient(transaction.Var2);
                }
                else if (MultiplayerManager.Instance.CurrentServer.Status == Networking.Status.ServerStatus.Running)
                {
                    transaction.Var1.ParseOnServer(transaction.Var2, transaction.Var3);
                }
            }
            _receivedTransactions.RemoveAll((Tuple<CommandHandler, byte[], Player> tuple) => { return tuple.Var3 == player; });
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
