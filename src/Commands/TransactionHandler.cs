using CSM.Commands.Data;
using CSM.Commands.Handler;
using CSM.Helpers;
using CSM.Networking;
using System.Collections.Generic;

namespace CSM.Commands
{
    public class TransactionHandler
    {
        private static readonly List<TransactionType> _sendStarted = new List<TransactionType>();
        private static readonly Dictionary<TransactionType, List<Tuple<CommandHandler, byte[], Player>>> _receivedTransactions = new Dictionary<TransactionType, List<Tuple<CommandHandler, byte[], Player>>>();

        /// <summary>
        /// Starts a transaction if the given command is a transaction command
        /// and the transaction was not yet started.
        /// </summary>
        /// <param name="command">A received command</param>
        public static void CheckSendTransaction(CommandBase command)
        {
            CommandHandler handler = Command.GetCommandHandler(command.GetType());
            TransactionType type = handler.Transaction;

            if (type != TransactionType.NONE)
            {
                if (!_sendStarted.Contains(type))
                {
                    _sendStarted.Add(type);
                }
            }
        }

        /// <summary>
        /// Finishes all transactions that were started before.
        /// </summary>
        public static void FinishSend()
        {
            foreach (TransactionType type in _sendStarted)
            {
                Command.SendToAll(new FinishTransactionCommand
                {
                    Type = type
                });
            }
            _sendStarted.Clear();
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
            if (handler.Transaction == TransactionType.NONE)
            {
                return false;
            }

            TransactionType type = handler.Transaction;

            if (!_receivedTransactions.ContainsKey(type))
            {
                _receivedTransactions[type] = new List<Tuple<CommandHandler, byte[], Player>>();
            }
            _receivedTransactions[type].Add(new Tuple<CommandHandler, byte[], Player>(handler, data, player));

            return true;
        }

        /// <summary>
        /// Called when the FinishTransactionCommand was received.
        /// </summary>
        /// <param name="type">The finished transaction type.</param>
        /// <param name="player">The sending player, may be null if it's the server.</param>
        public static void FinishReceived(TransactionType type, Player player)
        {
            if (_receivedTransactions.ContainsKey(type))
            {
                foreach (Tuple<CommandHandler, byte[], Player> transaction in _receivedTransactions[type])
                {
                    if (transaction.Var3 != player)
                    {
                        continue;
                    }

                    if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                    {
                        transaction.Var1.ParseOnClient(transaction.Var2);
                    }
                    else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                    {
                        transaction.Var1.ParseOnServer(transaction.Var2, transaction.Var3);
                    }
                }
                _receivedTransactions[type].RemoveAll((Tuple<CommandHandler, byte[], Player> tuple) => { return tuple.Var3 == player; });
            }
        }
    }

    public enum TransactionType
    {
        NONE,
        NODES
    }
}
