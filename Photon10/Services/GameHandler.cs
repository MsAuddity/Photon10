using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using Photon10.Data;
using Photon10.Network;

namespace Photon10.Services
{
    public delegate void GameEventProcessed(GameAttackData attackData);
    public struct PlayerGameData
    {
        public Models.Player Player { get; set; }
        public int Team { get; set; }
        public int Score { get; set; }
    }
    public class GameHandler : IDisposable
    {
        private const int scoreIncrement = 1;

        public List<string> EventsSinceLastUpdate { get; private set; } = new();

        private Dictionary<int, PlayerGameData> _playerData = new();
        private ConcurrentQueue<Data.GameAttackData> _pendingAttacks = new();
        public ConcurrentQueue<Data.GameAttackData> _eventHistory = new();

        GamePacketReceived packetReceivedEvent;
        public event GameEventProcessed EventProcessed;
        private string pythonExecutablePath = string.Empty;


        public void AddTrackedPlayer(Models.Player player, int team)
        {
            var playerData = new PlayerGameData
            {
                Player = player,
                Team = team,
                Score = 0
            };
            var id = player.Id;
            _playerData.Add(id, playerData);
        }
        public void StartListener()
        {

            //Subscribe to network event
            packetReceivedEvent = new(ProcessPacket);
            NetworkTrafficListener.EventReceived += packetReceivedEvent;

            //Start listening for game traffic
            NetworkTrafficListener.Start();
        }
        public void ProcessPacket(PacketReceivedEventArgs e)
        {

            string data = e.packetData;
            var targets = data.Normalize().Split(':');
            if (targets.Count() > 0)
            {
                GameAttackData action = new()
                {
                    attackerId = int.Parse(targets[0]),
                    receiverId = int.Parse(targets[1])
                };
                //EnqueueAttack(action);
                var attacker = _playerData[action.attackerId];
                attacker.Score += scoreIncrement;
                _playerData[action.attackerId] = attacker;
                FireEventProcessed(action);
            }
        }
        public PlayerGameData GetPlayerData(int playerId)
        {
            PlayerGameData data;
            _playerData.TryGetValue(playerId, out data);
            return data;
        }
        public void FireEventProcessed(GameAttackData attackData)
        {
            EventProcessed(attackData);
        }

        public string GetAttackEventString(GameAttackData attackEvent)
        {
            string attackerName = _playerData[attackEvent.attackerId].Player.Codename;
            string receiverName = _playerData[attackEvent.receiverId].Player.Codename;
            string eventString = $"{attackerName} hit {receiverName}!";
            return eventString;
        }
        public void StopGame()
        {
            NetworkTrafficListener.EventReceived -= packetReceivedEvent;
            NetworkTrafficListener.Stop();
        }
        public void Dispose()
        {
            _playerData.Clear();
            _eventHistory.Clear();
            NetworkTrafficListener.EventReceived -= packetReceivedEvent;
            NetworkTrafficListener.Stop();

        }
    }
}
